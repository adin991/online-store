import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

/**
 * Shows the first validation error of a reactive form control, translated.
 * Visible once the control was touched (or the form was submitted -> markAllAsTouched).
 *
 * Usage: <app-field-error [control]="form.controls.title"></app-field-error>
 */
@Component({
  selector: 'app-field-error',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  template: `
    <span class="field-error" *ngIf="message as m">{{ m.key | translate: m.params }}</span>
  `,
  styles: [`.field-error { display: block; margin-top: 4px; font-size: 12.5px; color: #e53935; }`]
})
export class FieldErrorComponent {
  @Input({ required: true }) control!: AbstractControl | null;

  get message(): { key: string; params?: Record<string, unknown> } | null {
    const c = this.control;
    if (!c || !c.invalid || !(c.touched || c.dirty)) return null;
    const e = c.errors ?? {};

    if (e['required'] || e['blank']) return { key: 'VALIDATION.REQUIRED' };
    if (e['email']) return { key: 'VALIDATION.EMAIL' };
    if (e['minlength']) return { key: 'VALIDATION.MIN_LENGTH', params: { min: e['minlength'].requiredLength } };
    if (e['maxlength']) return { key: 'VALIDATION.MAX_LENGTH', params: { max: e['maxlength'].requiredLength } };
    if (e['min']) return { key: 'VALIDATION.MIN', params: { min: e['min'].min } };
    if (e['max']) return { key: 'VALIDATION.MAX', params: { max: e['max'].max } };
    if (e['decimals']) return { key: 'VALIDATION.DECIMALS', params: { max: e['decimals'].max } };
    if (e['tooManyOptions']) return { key: 'VALIDATION.TOO_MANY_OPTIONS', params: { max: e['tooManyOptions'].max } };
    if (e['optionTooLong']) return { key: 'VALIDATION.OPTION_TOO_LONG', params: { max: e['optionTooLong'].max } };
    if (e['server']) return { key: e['server'] };
    return { key: 'VALIDATION.INVALID' };
  }
}
