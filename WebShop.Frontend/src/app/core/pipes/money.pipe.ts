import { Pipe, PipeTransform, inject } from '@angular/core';
import { formatCurrency, getCurrencySymbol } from '@angular/common';
import { LocaleService } from '../services/locale.service';

/**
 * Formats a price in the app currency (BAM / KM) with the selected regional settings,
 * e.g. bs: "1.234,50 KM", en: "KM1,234.50".
 * Impure so it re-renders when the language changes.
 */
@Pipe({ name: 'money', standalone: true, pure: false })
export class MoneyPipe implements PipeTransform {
  private locale = inject(LocaleService);

  transform(value: number | null | undefined, digitsInfo = '1.2-2', currencyCode?: string): string {
    if (value === null || value === undefined || isNaN(value)) return '';
    const localeId = this.locale.localeId();
    const code = currencyCode ?? this.locale.currency;
    return formatCurrency(value, localeId, getCurrencySymbol(code, 'narrow', localeId), code, digitsInfo);
  }
}
