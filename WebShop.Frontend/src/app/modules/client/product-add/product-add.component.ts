import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { ProductCategoriesApiService } from '../../../api-services/product-categories/product-categories-api.service';
import { ListProductCategoriesQueryDto, ListProductCategoriesRequest } from '../../../api-services/product-categories/product-categories-api.model';
import { AdsApiService, CreateAdCommand } from '../../../api-services/ads/ads-api.service';
import { FilesApiService } from '../../../api-services/files/files-api.service';
import { ImageCropModalService } from '../../../core/services/image-crop-modal.service';
import { BaseFormComponent } from '../../../core/components/base-classes/base-form-component';
import { ToasterService } from '../../../core/services/toaster.service';
import { LocaleService } from '../../../core/services/locale.service';
import { AD_LOCATIONS, AdForm, buildAdForm, toAdCommand } from '../ad-form';

@Component({
  selector: 'app-product-add',
  standalone: false,
  templateUrl: './product-add.component.html',
  styleUrl: './product-add.component.scss'
})
export class ProductAddComponent extends BaseFormComponent<CreateAdCommand> implements OnInit {
  previewUrl: string | null = null;
  uploadedImageUrl: string | null = null;
  isUploadingImage = false;
  imageUploadError = '';
  categories: ListProductCategoriesQueryDto[] = [];
  isSubmitting = false;

  readonly locations = AD_LOCATIONS;

  private fb = inject(FormBuilder);
  private categoriesApi = inject(ProductCategoriesApiService);
  private adsApi = inject(AdsApiService);
  private filesApi = inject(FilesApiService);
  private cropModal = inject(ImageCropModalService);
  private toaster = inject(ToasterService);
  private locale = inject(LocaleService);
  private router = inject(Router);

  override form: AdForm = buildAdForm(this.fb);

  ngOnInit(): void {
    this.initForm(false);

    const req = new ListProductCategoriesRequest();
    req.onlyEnabled = true;
    req.paging.pageSize = 100;
    this.categoriesApi.list(req).subscribe({
      next: (res) => this.categories = res.items ?? [],
      error: () => this.toaster.error(this.locale.t('AD_FORM.CATEGORIES_LOAD_FAILED'))
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';
    if (!file) return;

    this.cropModal.open({
      file,
      onCropped: (croppedFile) => this.uploadImage(croppedFile)
    });
  }

  private uploadImage(file: File): void {
    const reader = new FileReader();
    reader.onload = () => { this.previewUrl = reader.result as string; };
    reader.readAsDataURL(file);

    this.uploadedImageUrl = null;
    this.imageUploadError = '';
    this.isUploadingImage = true;

    this.filesApi.upload(file).subscribe({
      next: (res) => {
        this.uploadedImageUrl = res.url;
        this.isUploadingImage = false;
      },
      error: () => {
        this.imageUploadError = 'AD_FORM.IMAGE_UPLOAD_FAILED';
        this.isUploadingImage = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }

  /** Add mode only - nothing to load. */
  protected loadData(): void { }

  /** Called by BaseFormComponent.onSubmit() once the form is valid. */
  protected save(): void {
    if (this.isUploadingImage || this.isSubmitting) return;

    this.isSubmitting = true;
    const payload = toAdCommand(this.form, this.uploadedImageUrl ? [this.uploadedImageUrl] : []);

    this.adsApi.create(payload).subscribe({
      next: () => {
        this.toaster.success(this.locale.t('AD_FORM.CREATED'));
        this.router.navigate(['/client/dashboard']);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.toaster.error(err?.error?.message ?? this.locale.t('AD_FORM.SAVE_FAILED'));
      }
    });
  }
}
