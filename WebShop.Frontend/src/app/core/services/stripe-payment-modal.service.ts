import { Injectable, signal } from '@angular/core';
import { CartCheckoutItem } from '../../api-services/payments/payments-api.service';

export interface StripePaymentModalData {
  title: string;
  priceKm: number;
  /** Purchase of a single ad (e.g. "Buy now" on the ad details). */
  adId?: number;
  /** Checkout of the whole cart - all lines are charged with one Stripe payment. */
  cartItems?: CartCheckoutItem[];
  onSuccess?: () => void;
}

@Injectable({ providedIn: 'root' })
export class StripePaymentModalService {
  private _data = signal<StripePaymentModalData | null>(null);
  data = this._data.asReadonly();

  open(data: StripePaymentModalData): void {
    this._data.set(data);
  }

  close(): void {
    this._data.set(null);
  }
}
