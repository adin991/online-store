import { HttpInterceptorFn } from '@angular/common/http';
import { savedCulture } from '../services/locale.service';

/**
 * Sends the selected regional setting as Accept-Language, so the backend
 * (UseRequestLocalization) formats with the same culture as the UI.
 *
 * Reads the saved language directly instead of injecting LocaleService:
 * LocaleService -> TranslateService -> (loads en.json over HttpClient) -> this interceptor
 * would be a circular dependency, and the failed load of en.json is cached by ngx-translate,
 * so English would show raw keys (e.g. "CATEGORIES.SPORTS").
 */
export const localeInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req.clone({ setHeaders: { 'Accept-Language': savedCulture() } }));
};
