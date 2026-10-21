import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface PaymentIntentResultDto {
  transactionIds: number[];
  clientSecret: string;
  publishableKey: string;
  amountEur: number;
}

export interface ConfirmPaymentResultDto {
  success: boolean;
  status: string;
}

export interface CartCheckoutItem {
  adId: number;
  quantity: number;
}

interface CreatePaymentIntentResultDto {
  transactionId: number;
  clientSecret: string;
  publishableKey: string;
  amountEur: number;
}

@Injectable({ providedIn: 'root' })
export class PaymentsApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/Payments`;
  private http = inject(HttpClient);

  createIntent(adId: number): Observable<PaymentIntentResultDto> {
    return this.http.post<CreatePaymentIntentResultDto>(`${this.baseUrl}/create-intent`, { adId }).pipe(
      map(res => ({
        transactionIds: [res.transactionId],
        clientSecret: res.clientSecret,
        publishableKey: res.publishableKey,
        amountEur: res.amountEur
      }))
    );
  }

  createCartIntent(items: CartCheckoutItem[]): Observable<PaymentIntentResultDto> {
    return this.http.post<PaymentIntentResultDto>(`${this.baseUrl}/create-cart-intent`, { items });
  }

  confirm(transactionIds: number[]): Observable<ConfirmPaymentResultDto> {
    return this.http.post<ConfirmPaymentResultDto>(`${this.baseUrl}/confirm`, { transactionIds });
  }
}
