// src/app/core/guards/my-auth-guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, ActivatedRouteSnapshot, Router, RouterStateSnapshot } from '@angular/router';
import { CurrentUserService } from '../services/auth/current-user.service';

export const myAuthGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const currentUser = inject(CurrentUserService);
  const router = inject(Router);

  // Read auth data from route.data['auth'] (myAuthData returns {auth: ...})
  const auth = route.data['auth'] as MyAuthRouteData | undefined;

  const requireAuth = auth?.requireAuth === true;
  const requireAdmin = auth?.requireAdmin === true;
  const requireManager = auth?.requireManager === true;
  const requireEmployee = auth?.requireEmployee === true;

  // Public route -> let it through
  if (!requireAuth) {
    return true;
  }

  // 1) the route needs a logged-in user -> go to login and come back afterwards
  const user = currentUser.snapshot;
  if (!currentUser.isAuthenticated() || !user) {
    return router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } });
  }

  // 2) role check - admin > manager > employee
  if (requireAdmin && !user.isAdmin) {
    return router.parseUrl(currentUser.getDefaultRoute());
  }

  if (requireManager && !user.isManager) {
    return router.parseUrl(currentUser.getDefaultRoute());
  }

  if (requireEmployee && !user.isEmployee) {
    return router.parseUrl(currentUser.getDefaultRoute());
  }

  return true;
};

export interface MyAuthRouteData {
  requireAuth?: boolean;
  requireAdmin?: boolean;
  requireManager?: boolean;
  requireEmployee?: boolean;
}

export function myAuthData(data: MyAuthRouteData): { auth: MyAuthRouteData } {
  return { auth: data };
}
