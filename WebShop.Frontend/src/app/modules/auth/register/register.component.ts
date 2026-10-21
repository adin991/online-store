import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthApiService } from '../../../api-services/auth/auth-api.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { LocaleService } from '../../../core/services/locale.service';
import { AppValidators } from '../../../core/validators/app-validators';

/** Limits mirror CreateUserCommandValidator on the backend. */
export const USER_CONSTRAINTS = {
  nameMaxLength: 50,
  emailMaxLength: 200,
  passwordMinLength: 4,
  passwordMaxLength: 100
} as const;

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  showPassword = false;
  isLoading = false;

  private fb = inject(FormBuilder);
  private api = inject(AuthApiService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private locale = inject(LocaleService);

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required, AppValidators.notBlank, Validators.maxLength(USER_CONSTRAINTS.nameMaxLength)]],
    surname: ['', [Validators.required, AppValidators.notBlank, Validators.maxLength(USER_CONSTRAINTS.nameMaxLength)]],
    gender: [0, [Validators.required]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(USER_CONSTRAINTS.emailMaxLength)]],
    password: ['', [
      Validators.required,
      Validators.minLength(USER_CONSTRAINTS.passwordMinLength),
      Validators.maxLength(USER_CONSTRAINTS.passwordMaxLength)
    ]]
  });

  get password(): string {
    return this.form.controls.password.value;
  }

  get passwordStrength(): number {
    const p = this.password;
    if (p.length === 0) return -1;
    if (p.length < 6) return 0;
    let score = 0;
    if (/[a-z]/.test(p)) score++;
    if (/[A-Z]/.test(p)) score++;
    if (/[0-9]/.test(p)) score++;
    if (/[^a-zA-Z0-9]/.test(p)) score++;
    if (score <= 1) return 1;
    if (score === 2) return 2;
    if (score === 3) return 3;
    return 4;
  }

  get strengthLabelKey(): string {
    return ['AUTH.STRENGTH_TOO_SHORT', 'AUTH.STRENGTH_WEAK', 'AUTH.STRENGTH_FAIR', 'AUTH.STRENGTH_GOOD', 'AUTH.STRENGTH_STRONG'][this.passwordStrength] ?? '';
  }

  get strengthColor(): string {
    return ['#111111', '#e53935', '#fdd835', '#29b6f6', '#43a047'][this.passwordStrength] ?? '';
  }

  get strengthBars(): number {
    return [1, 1, 2, 3, 4][this.passwordStrength] ?? 0;
  }

  signUp(): void {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.isLoading) return;

    this.isLoading = true;
    const v = this.form.getRawValue();

    this.api.register({
      name: v.name.trim(),
      surname: v.surname.trim(),
      email: v.email.trim(),
      password: v.password,
      gender: Number(v.gender)
    }).subscribe({
      next: () => {
        this.isLoading = false;
        this.toaster.success(this.locale.t('AUTH.REGISTER_SUCCESS'));
        this.router.navigate(['/auth/login']);
      },
      error: (err) => {
        this.isLoading = false;
        this.toaster.error(err?.error?.message ?? this.locale.t('AUTH.REGISTER_FAILED'));
      }
    });
  }
}
