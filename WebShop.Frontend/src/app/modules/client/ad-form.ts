import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AD_CONSTRAINTS, AdDetailDto, CreateAdCommand } from '../../api-services/ads/ads-api.service';
import { AppValidators, splitOptions } from '../../core/validators/app-validators';

/** Locations offered in the add/edit ad forms. */
export const AD_LOCATIONS = ['Sarajevo', 'Mostar', 'Banja Luka'];

export type AdForm = FormGroup<{
  title: FormControl<string>;
  price: FormControl<number | null>;
  categoryId: FormControl<number | null>;
  location: FormControl<string>;
  description: FormControl<string>;
  colors: FormControl<string>;
  sizes: FormControl<string>;
}>;

/**
 * Reactive form shared by "Add ad" and "Edit ad".
 * The rules mirror Create/UpdateAdCommandValidator on the backend.
 */
export function buildAdForm(fb: FormBuilder): AdForm {
  const c = AD_CONSTRAINTS;
  return fb.group({
    title: fb.nonNullable.control('', [Validators.required, AppValidators.notBlank, Validators.maxLength(c.titleMaxLength)]),
    price: fb.control<number | null>(null, [Validators.required, Validators.min(0.01), Validators.max(c.priceMax), AppValidators.decimals(2)]),
    categoryId: fb.control<number | null>(null, [Validators.required]),
    location: fb.nonNullable.control(''),
    description: fb.nonNullable.control('', [Validators.required, AppValidators.notBlank, Validators.maxLength(c.descriptionMaxLength)]),
    colors: fb.nonNullable.control('', [AppValidators.optionList(c.maxOptions, c.optionMaxLength)]),
    sizes: fb.nonNullable.control('', [AppValidators.optionList(c.maxOptions, c.optionMaxLength)])
  });
}

/** Fills the form from an existing ad (edit mode). */
export function patchAdForm(form: AdForm, ad: AdDetailDto): void {
  form.patchValue({
    title: ad.title,
    price: ad.price,
    categoryId: ad.category?.id ?? null,
    location: ad.location ?? '',
    description: ad.description,
    colors: (ad.colors ?? []).join(', '),
    sizes: (ad.sizes ?? []).join(', ')
  });
}

/** Maps the form value to the API command. */
export function toAdCommand(form: AdForm, images: string[]): CreateAdCommand {
  const v = form.getRawValue();
  return {
    title: v.title.trim(),
    price: Number(v.price),
    description: v.description.trim(),
    categoryId: Number(v.categoryId),
    location: v.location || null,
    images,
    colors: splitOptions(v.colors),
    sizes: splitOptions(v.sizes)
  };
}
