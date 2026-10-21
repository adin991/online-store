import { Injectable, computed, inject, signal } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeBs from '@angular/common/locales/bs';
import { TranslateService } from '@ngx-translate/core';

registerLocaleData(localeBs, 'bs');

export type AppLang = 'en' | 'bs';

/** UI language -> Angular locale id (formatting) + BCP-47 culture sent to the API. */
const REGIONS: Record<AppLang, { locale: string; culture: string }> = {
  en: { locale: 'en-US', culture: 'en-US' },
  bs: { locale: 'bs', culture: 'bs-BA' }
};

const STORAGE_KEY = 'lang';
const DEFAULT_LANG: AppLang = 'en';

/** Currency of all ad prices (ISO 4217, "KM"). */
export const APP_CURRENCY = 'BAM';

export function readSavedLang(): AppLang {
  const saved = localStorage.getItem(STORAGE_KEY);
  return saved === 'bs' || saved === 'en' ? saved : DEFAULT_LANG;
}

/** Angular locale id for the saved language - used for the LOCALE_ID provider at startup. */
export function initialLocaleId(): string {
  return REGIONS[readSavedLang()].locale;
}

/** Culture (Accept-Language) for the saved language. Has no DI dependencies, so the
 *  HTTP interceptor can use it while TranslateService is still loading translations. */
export function savedCulture(): string {
  return REGIONS[readSavedLang()].culture;
}

/**
 * Regional settings of the app: the selected language drives the UI texts (ngx-translate),
 * the formatting of dates / numbers / currency (money + localDate pipes) and the
 * Accept-Language header, so the backend uses the same culture.
 */
@Injectable({ providedIn: 'root' })
export class LocaleService {
  private translate = inject(TranslateService);

  private _lang = signal<AppLang>(readSavedLang());
  readonly lang = this._lang.asReadonly();

  /** Angular locale id used by formatDate / formatCurrency / formatNumber. */
  readonly localeId = computed(() => REGIONS[this._lang()].locale);

  /** Culture sent to the backend (Accept-Language). */
  readonly culture = computed(() => REGIONS[this._lang()].culture);

  readonly currency = APP_CURRENCY;

  constructor() {
    this.apply(this._lang());
  }

  setLang(lang: AppLang): void {
    this._lang.set(lang);
    localStorage.setItem(STORAGE_KEY, lang);
    this.apply(lang);
  }

  /** Translates a key right away (for toasts, confirm dialogs...). */
  t(key: string, params?: Record<string, unknown>): string {
    return this.translate.instant(key, params);
  }

  private apply(lang: AppLang): void {
    this.translate.use(lang);
    document.documentElement.lang = REGIONS[lang].culture;
  }
}
