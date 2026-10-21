import { LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { ImageCropperComponent } from 'ngx-image-cropper';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { CustomTranslateLoader } from './core/services/custom-translate-loader';
import { ToastComponent } from './core/components/toast/toast.component';
import { OrderModalComponent } from './core/components/order-modal/order-modal.component';
import { StripePaymentModalComponent } from './core/components/stripe-payment-modal/stripe-payment-modal.component';
import { ImageCropModalComponent } from './core/components/image-crop-modal/image-crop-modal.component';
import { ChatWidgetComponent } from './core/components/chat-widget/chat-widget.component';
import { authInterceptor } from './core/interceptors/auth-interceptor.service';
import { localeInterceptor } from './core/interceptors/locale-interceptor.service';
import { initialLocaleId } from './core/services/locale.service';
import { MoneyPipe } from './core/pipes/money.pipe';

@NgModule({
  declarations: [
    AppComponent,
    ToastComponent,
    OrderModalComponent,
    StripePaymentModalComponent,
    ImageCropModalComponent,
    ChatWidgetComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    ReactiveFormsModule,
    AppRoutingModule,
    RouterModule,
    ImageCropperComponent,
    MoneyPipe,
    TranslateModule.forRoot({
      defaultLanguage: 'en',
      loader: {
        provide: TranslateLoader,
        useClass: CustomTranslateLoader,
        deps: [HttpClient]
      }
    })
  ],
  providers: [
    // Locale of the saved regional setting, used by Angular's built-in pipes
    { provide: LOCALE_ID, useFactory: initialLocaleId },
    provideHttpClient(withInterceptors([localeInterceptor, authInterceptor]))
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
