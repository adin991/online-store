import { Component, ElementRef, ViewChild, effect, inject, signal } from '@angular/core';
import { loadStripe, Stripe, StripeElements, StripePaymentElement } from '@stripe/stripe-js';
import { StripePaymentModalData, StripePaymentModalService } from '../../services/stripe-payment-modal.service';
import { PaymentsApiService } from '../../../api-services/payments/payments-api.service';
import { LocaleService } from '../../services/locale.service';

@Component({
  selector: 'app-stripe-payment-modal',
  standalone: false,
  templateUrl: './stripe-payment-modal.component.html',
  styleUrl: './stripe-payment-modal.component.scss'
})
export class StripePaymentModalComponent {
  modal = inject(StripePaymentModalService);
  private paymentsApi = inject(PaymentsApiService);
  private locale = inject(LocaleService);

  @ViewChild('paymentElementContainer') paymentElementContainerRef?: ElementRef<HTMLDivElement>;

  loading = signal(true);
  submitting = signal(false);
  succeeded = signal(false);
  errorMsg = signal<string | null>(null);
  amountEur = signal<number | null>(null);

  private stripe: Stripe | null = null;
  private elements: StripeElements | null = null;
  private paymentElement: StripePaymentElement | null = null;
  private transactionIds: number[] = [];
  private onSuccess?: () => void;

  constructor() {
    effect(() => {
      const data = this.modal.data();
      if (data) {
        this.startPayment(data);
      } else {
        this.reset();
      }
    });
  }

  private reset(): void {
    this.paymentElement?.unmount();
    this.paymentElement = null;
    this.elements = null;
    this.stripe = null;
    this.transactionIds = [];
    this.onSuccess = undefined;
    this.loading.set(true);
    this.submitting.set(false);
    this.succeeded.set(false);
    this.errorMsg.set(null);
    this.amountEur.set(null);
  }

  private startPayment(data: StripePaymentModalData): void {
    this.loading.set(true);
    this.errorMsg.set(null);
    this.succeeded.set(false);
    this.onSuccess = data.onSuccess;

    const request$ = data.adId != null
      ? this.paymentsApi.createIntent(data.adId)
      : this.paymentsApi.createCartIntent(data.cartItems ?? []);

    request$.subscribe({
      next: async (res) => {
        this.transactionIds = res.transactionIds;
        this.amountEur.set(res.amountEur);

        this.stripe = await loadStripe(res.publishableKey);
        if (!this.stripe) {
          this.errorMsg.set(this.locale.t('PAYMENT.STRIPE_LOAD_FAILED'));
          this.loading.set(false);
          return;
        }

        this.elements = this.stripe.elements({ clientSecret: res.clientSecret });
        this.paymentElement = this.elements.create('payment');

        // The container must become visible first (the 'hidden' class removed) before mounting,
        // because the Stripe iframe computes a wrong layout inside a display:none container
        // (the field stays non-interactive - it does not accept focus/input).
        this.loading.set(false);
        setTimeout(() => {
          if (this.paymentElementContainerRef) {
            this.paymentElement?.mount(this.paymentElementContainerRef.nativeElement);
          }
        });
      },
      error: (err) => {
        this.errorMsg.set(err?.error?.message ?? this.locale.t('PAYMENT.START_FAILED'));
        this.loading.set(false);
      }
    });
  }

  async pay(): Promise<void> {
    if (!this.stripe || !this.elements || this.submitting()) return;

    this.submitting.set(true);
    this.errorMsg.set(null);

    const { error } = await this.stripe.confirmPayment({
      elements: this.elements,
      redirect: 'if_required'
    });

    if (error) {
      this.errorMsg.set(error.message ?? this.locale.t('PAYMENT.FAILED'));
      this.submitting.set(false);
      return;
    }

    this.paymentsApi.confirm(this.transactionIds).subscribe({
      next: (res) => {
        this.submitting.set(false);
        if (res.success) {
          this.succeeded.set(true);
          this.onSuccess?.();
        } else {
          this.errorMsg.set(this.locale.t('PAYMENT.NOT_COMPLETED', { status: res.status }));
        }
      },
      error: () => {
        this.submitting.set(false);
        this.errorMsg.set(this.locale.t('PAYMENT.CONFIRM_FAILED'));
      }
    });
  }

  close(): void {
    this.modal.close();
  }
}
