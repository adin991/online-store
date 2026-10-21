// src/app/core/services/auth/auth-facade.service.ts
import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of, tap, catchError, map } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

import { AuthApiService } from '../../../api-services/auth/auth-api.service';
import {
  LoginCommand,
  LoginCommandDto,
  LogoutCommand,
  RefreshTokenCommand,
  RefreshTokenCommandDto,
} from '../../../api-services/auth/auth-api.model';

import { AuthStorageService } from './auth-storage.service';
import { CurrentUserDto } from './current-user.dto';
import { JwtPayloadDto } from './jwt-payload.dto';
import { UserApiService } from '../../../api-services/user/user-api.service';

/**
 * Main auth service (facade).
 * - talks to AuthApiService (HTTP)
 * - talks to AuthStorageService (localStorage)
 * - decodes the JWT and keeps the CurrentUser as a signal
 *
 * Used in:
 * - the interceptor (getAccessToken, refresh)
 * - guards (isAuthenticated, isAdmin)
 * - components (login, logout, navbar)
 */
@Injectable({ providedIn: 'root' })
export class AuthFacadeService {
  private api = inject(AuthApiService);
  private userApi = inject(UserApiService);
  private storage = inject(AuthStorageService);
  private router = inject(Router);

  // === REACTIVE STATE: current user ===

  private _currentUser = signal<CurrentUserDto | null>(null);

  /** read-only signal for the UI - read as auth.currentUser() */
  currentUser = this._currentUser.asReadonly();

  /** computed signals over the current user */
  isAuthenticated = computed(() => !!this._currentUser());
  isAdmin = computed(() => this._currentUser()?.isAdmin ?? false);
  isManager = computed(() => this._currentUser()?.isManager ?? false);
  isEmployee = computed(() => this._currentUser()?.isEmployee ?? false);

  constructor() {
    // try to restore the state from an existing access token
    this.initializeFromToken();
  }

  // =========================================================
  // PUBLIC API
  // =========================================================

  /**
   * Login: checks email/password. Tokens are saved to storage right away
   * and the current user state is filled.
   */
  login(payload: LoginCommand): Observable<void> {
    return this.api.login(payload).pipe(
      tap((response: LoginCommandDto) => {
        this.storage.saveLogin(response);
        this.decodeAndSetUser(response.accessToken);
      }),
      map(() => void 0)
    );
  }

  /**
   * Logout:
   * - clear the state and tokens locally
   * - try to invalidate the refresh token on the server (errors are ignored)
   */
  logout(): Observable<void> {
    const refreshToken = this.storage.getRefreshToken();

    // 1) clear locally (optimistic logout)
    this.clearUserState();

    // 2) no refresh token -> no API call
    if (!refreshToken) {
      return of(void 0);
    }

    const payload: LogoutCommand = { refreshToken };

    // 3) try the server-side logout, ignore errors
    return this.api.logout(payload).pipe(catchError(() => of(void 0)));
  }

  /**
   * Refreshes the access token using the refresh token.
   * Called by the interceptor on a 401.
   */
  refresh(payload: RefreshTokenCommand): Observable<RefreshTokenCommandDto> {
    return this.api.refresh(payload).pipe(
      tap((response: RefreshTokenCommandDto) => {
        this.storage.saveRefresh(response);           // store the new tokens
        this.decodeAndSetUser(response.accessToken);  // update the current user
      })
    );
  }

  /**
   * Utility for guards/interceptors - clears the auth state and navigates to /login.
   */
  redirectToLogin(): void {
    this.clearUserState();
    this.router.navigate(['/auth/login']);
  }

  /**
   * Reloads name/surname/gender of the logged-in user (GET api/User/me),
   * e.g. after the profile was edited.
   */
  refreshProfile(): void {
    if (!this._currentUser()) return;

    this.userApi.getMe().subscribe({
      next: (profile) => {
        const current = this._currentUser();
        if (current) {
          this._currentUser.set({
            ...current,
            name: profile.name,
            surname: profile.surname,
            gender: UserApiService.genderToNumber(profile.gender)
          });
        }
      },
      error: (err) => console.error('Failed to load user profile:', err)
    });
  }

  // =========================================================
  // GETTERS FOR THE INTERCEPTOR
  // =========================================================

  /**
   * Access token for the Authorization header.
   */
  getAccessToken(): string | null {
    return this.storage.getAccessToken();
  }

  /**
   * Refresh token for the refresh call.
   */
  getRefreshToken(): string | null {
    return this.storage.getRefreshToken();
  }

  // =========================================================
  // PRIVATE HELPERS
  // =========================================================

  /**
   * On application start (constructor) - try to restore the state from an existing token.
   */
  private initializeFromToken(): void {
    const token = this.storage.getAccessToken();
    if (token) {
      this.decodeAndSetUser(token);
    }
  }

  /**
   * Decodes the JWT and sets the current user state, then loads name/surname/gender.
   */
  private decodeAndSetUser(token: string): void {
    try {
      const payload = jwtDecode<JwtPayloadDto>(token);

      const user: CurrentUserDto = {
        userId: Number(payload.sub),
        email: payload.email,
        isAdmin: payload.is_admin === 'true',
        isManager: payload.is_manager === 'true',
        isEmployee: payload.is_employee === 'true',
        tokenVersion: Number(payload.ver),
      };

      this._currentUser.set(user);
      // deferred: the HTTP interceptor injects this service, so avoid a circular call during construction
      setTimeout(() => this.refreshProfile(), 0);
    } catch (error) {
      console.error('Failed to decode JWT token:', error);
      this._currentUser.set(null);
    }
  }

  /**
   * Clears the user state and all tokens from storage.
   */
  private clearUserState(): void {
    this._currentUser.set(null);
    this.storage.clear();
  }
}
