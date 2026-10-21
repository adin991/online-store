import { Pipe, PipeTransform, inject } from '@angular/core';
import { formatDate } from '@angular/common';
import { LocaleService } from '../services/locale.service';

/**
 * Formats a date with the selected regional settings using a locale-aware
 * format name ('short', 'medium', 'shortDate', ...) instead of a hard-coded pattern.
 * Impure so it re-renders when the language changes.
 */
@Pipe({ name: 'localDate', standalone: true, pure: false })
export class LocalDatePipe implements PipeTransform {
  private locale = inject(LocaleService);

  transform(value: string | number | Date | null | undefined, format = 'short'): string {
    if (value === null || value === undefined || value === '') return '';
    // The API returns UTC timestamps without a zone suffix - treat them as UTC.
    const date = typeof value === 'string' && !/[zZ]|[+-]\d{2}:?\d{2}$/.test(value) ? value + 'Z' : value;
    return formatDate(date, format, this.locale.localeId());
  }
}
