import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdDetailDto, AdsApiService } from '../../../api-services/ads/ads-api.service';
import { ProductCategoriesApiService } from '../../../api-services/product-categories/product-categories-api.service';
import { ListProductCategoriesQueryDto, ListProductCategoriesRequest } from '../../../api-services/product-categories/product-categories-api.model';
import { FilesApiService } from '../../../api-services/files/files-api.service';
import { ImageCropModalService } from '../../../core/services/image-crop-modal.service';
import { BaseFormComponent } from '../../../core/components/base-classes/base-form-component';
import { ToasterService } from '../../../core/services/toaster.service';
import { LocaleService } from '../../../core/services/locale.service';
import { AD_LOCATIONS, AdForm, buildAdForm, patchAdForm, toAdCommand } from '../ad-form';

@Component({
  selector: 'app-ad-edit',
  standalone: false,
  templateUrl: './ad-edit.component.html',
  styleUrl: './ad-edit.component.scss'
})
export class AdEditComponent extends BaseFormComponent<AdDetailDto> implements OnInit {
  adId = 0;
  previewUrl: string | null = null;
  uploadedImageUrl: string | null = null;
  isUploadingImage = false;
  imageUploadError = '';
  categories: ListProductCategoriesQueryDto[] = [];

  readonly locations = AD_LOCATIONS;

  isSaving = false;
  saveError = '';

  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  router = inject(Router);
  private adsApi = inject(AdsApiService);
  private categoriesApi = inject(ProductCategoriesApiService);
  private filesApi = inject(FilesApiService);
  private cropModal = inject(ImageCropModalService);
  private toaster = inject(ToasterService);
  private locale = inject(LocaleService);

  override form: AdForm = buildAdForm(this.fb);

  ngOnInit(): void {
    this.adId = Number(this.route.snapshot.paramMap.get('id'));
    if (!this.adId) { this.router.navigate(['/']); return; }

    const req = new ListProductCategoriesRequest();
    req.onlyEnabled = true;
    req.paging.pageSize = 100;
    this.categoriesApi.list(req).subscribe({
      next: (res) => this.categories = res.items ?? []
    });

    this.initForm(true);
  }

  /** Edit mode: load the ad and fill the form. */
  protected loadData(): void {
    this.startLoading();
    this.adsApi.getById(this.adId).subscribe({
      next: (ad) => {
        this.model = ad;
        patchAdForm(this.form, ad);
        this.previewUrl = ad.images?.[0] ?? null;
        this.stopLoading();
      },
      error: () => {
        this.stopLoading();
        this.router.navigate(['/']);
      }
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

  removeImage(): void {
    this.previewUrl = null;
    this.uploadedImageUrl = null;
  }

  /** Called by BaseFormComponent.onSubmit() once the form is valid. */
  protected save(): void {
    if (this.isUploadingImage || this.isSaving) return;
    this.isSaving = true;
    this.saveError = '';

    const images = this.uploadedImageUrl ? [this.uploadedImageUrl] : (this.previewUrl ? [this.previewUrl] : []);

    this.adsApi.update(this.adId, toAdCommand(this.form, images)).subscribe({
      next: () => {
        this.toaster.success(this.locale.t('AD_FORM.SAVED'));
        this.router.navigate(['/ad', this.adId]);
      },
      error: (err) => {
        this.isSaving = false;
        this.saveError = err?.error?.message ?? this.locale.t('AD_FORM.SAVE_FAILED');
      }
    });
  }
}
