import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { UserApiService, UserProfileDto } from '../../../api-services/user/user-api.service';
import { BaseFormComponent } from '../../../core/components/base-classes/base-form-component';
import { AppValidators } from '../../../core/validators/app-validators';
import { AppLang, LocaleService } from '../../../core/services/locale.service';
import { USER_CONSTRAINTS } from '../../auth/register/register.component';

/** Settings page: profile data (API) + regional setting (language / formatting). */
@Component({
  selector: 'app-profile-edit',
  standalone: false,
  templateUrl: './profile-edit.component.html',
  styleUrl: './profile-edit.component.scss'
})
export class ProfileEditComponent extends BaseFormComponent<UserProfileDto> implements OnInit {
  isSaving = false;
  isDeleting = false;
  saveError = '';
  saveSuccess = false;
  showDeleteConfirm = false;

  auth = inject(AuthFacadeService);
  locale = inject(LocaleService);
  private fb = inject(FormBuilder);
  private userApi = inject(UserApiService);
  router = inject(Router);

  override form = this.fb.nonNullable.group({
    name: ['', [Validators.required, AppValidators.notBlank, Validators.maxLength(USER_CONSTRAINTS.nameMaxLength)]],
    surname: ['', [Validators.required, AppValidators.notBlank, Validators.maxLength(USER_CONSTRAINTS.nameMaxLength)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(USER_CONSTRAINTS.emailMaxLength)]],
    gender: [0, [Validators.required]]
  });

  readonly languages: { code: AppLang; labelKey: string }[] = [
    { code: 'en', labelKey: 'SETTINGS.LANG_EN' },
    { code: 'bs', labelKey: 'SETTINGS.LANG_BS' }
  ];

  /** Shown as a formatting preview of the selected regional setting. */
  readonly sampleDate = new Date();

  get initials(): string {
    const { name, surname } = this.form.getRawValue();
    return ((name || '?')[0] + (surname || ' ')[0]).toUpperCase().trim();
  }

  ngOnInit(): void {
    this.initForm(true);
  }

  /** Loads the logged-in user's profile (GET api/User/me). */
  protected loadData(): void {
    this.startLoading();
    this.userApi.getMe().subscribe({
      next: (p) => {
        this.model = p;
        this.form.reset({
          name: p.name,
          surname: p.surname,
          email: p.email,
          gender: UserApiService.genderToNumber(p.gender)
        });
        this.stopLoading();
      },
      error: () => {
        this.stopLoading();
        this.router.navigate(['/']);
      }
    });
  }

  setGender(gender: number): void {
    this.form.controls.gender.setValue(gender);
    this.form.controls.gender.markAsDirty();
  }

  setLanguage(lang: AppLang): void {
    this.locale.setLang(lang);
  }

  /** Called by BaseFormComponent.onSubmit() once the form is valid. */
  protected save(): void {
    const user = this.auth.currentUser();
    if (!user || this.isSaving) return;
    this.isSaving = true;
    this.saveError = '';
    this.saveSuccess = false;

    const v = this.form.getRawValue();
    this.userApi.update(user.userId, {
      name: v.name.trim(),
      surname: v.surname.trim(),
      email: v.email.trim(),
      gender: v.gender
    }).subscribe({
      next: () => {
        this.isSaving = false;
        this.saveSuccess = true;
        this.form.markAsPristine();
        // refresh the signal so the navbar shows the updated name
        this.auth.refreshProfile();
      },
      error: (err) => {
        this.isSaving = false;
        this.saveError = err?.error?.message ?? this.locale.t('COMMON.SAVE_FAILED');
      }
    });
  }

  confirmDelete(): void {
    this.showDeleteConfirm = true;
  }

  cancelDelete(): void {
    this.showDeleteConfirm = false;
  }

  deleteAccount(): void {
    const user = this.auth.currentUser();
    if (!user) return;
    this.isDeleting = true;

    this.userApi.delete(user.userId).subscribe({
      next: () => {
        this.auth.logout().subscribe();
        this.router.navigate(['/']);
      },
      error: () => { this.isDeleting = false; }
    });
  }
}
