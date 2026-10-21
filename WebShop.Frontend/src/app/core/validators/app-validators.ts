import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Custom validators for the reactive forms. Limits mirror the backend
 * FluentValidation rules, so the UI and a direct API call enforce the same rules.
 */
export class AppValidators {
  /** Required and not only whitespace. */
  static notBlank(control: AbstractControl): ValidationErrors | null {
    const v = control.value;
    return typeof v === 'string' && v.trim().length === 0 ? { blank: true } : null;
  }

  /** At most `max` decimal places. */
  static decimals(max: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const v = control.value;
      if (v === null || v === undefined || v === '') return null;
      const parts = String(v).split('.');
      return parts.length > 1 && parts[1].length > max ? { decimals: { max } } : null;
    };
  }

  /** Comma-separated option list (e.g. "Red, Blue"): max item count and max item length. */
  static optionList(maxItems: number, maxLength: number): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const items = splitOptions(control.value);
      if (items.length > maxItems) return { tooManyOptions: { max: maxItems } };
      if (items.some(i => i.length > maxLength)) return { optionTooLong: { max: maxLength } };
      return null;
    };
  }
}

/** "Red, blue ,,Green" -> ["Red", "blue", "Green"] (trimmed, empty and duplicate entries removed). */
export function splitOptions(value: string | null | undefined): string[] {
  const seen = new Set<string>();
  return (value ?? '')
    .split(',')
    .map(v => v.trim())
    .filter(v => {
      const key = v.toLowerCase();
      if (!v || seen.has(key)) return false;
      seen.add(key);
      return true;
    });
}
