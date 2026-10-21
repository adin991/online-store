import { Component, inject } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { CartService } from '../../../core/services/cart/cart.service';
import { StripePaymentModalService } from '../../../core/services/stripe-payment-modal.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { AppLang, LocaleService } from '../../../core/services/locale.service';

@Component({
  selector: 'app-public-layout',
  standalone: false,
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.scss'
})
export class PublicLayoutComponent {
  isCartOpen = false;
  isLangOpen = false;
  isProfileOpen = false;

  auth = inject(AuthFacadeService);
  cart = inject(CartService);
  locale = inject(LocaleService);
  private stripePaymentModal = inject(StripePaymentModalService);
  private toaster = inject(ToasterService);
  private router = inject(Router);

  /** Navbar search - navigates to the home page with ?search=... */
  search = new FormControl('', { nonNullable: true, validators: [Validators.maxLength(100)] });

  get currentLang(): AppLang {
    return this.locale.lang();
  }

  submitSearch(): void {
    if (this.search.invalid) return;
    const term = this.search.value.trim();
    this.router.navigate(['/'], { queryParams: term ? { search: term } : {} });
  }

  toggleCart() {
    this.isCartOpen = !this.isCartOpen;
  }

  removeFromCart(key: string) {
    this.cart.remove(key);
  }

  saveForLater(key: string) {
    this.cart.saveForLater(key);
  }

  moveToCart(key: string) {
    this.cart.moveToCart(key);
  }

  removeSaved(key: string) {
    this.cart.removeSaved(key);
  }

  checkout() {
    if (this.cart.items().length === 0) return;
    if (!this.auth.isAuthenticated()) {
      this.toaster.error(this.locale.t('CART.LOGIN_TO_CHECKOUT'));
      return;
    }

    this.stripePaymentModal.open({
      title: this.locale.t('CART.CHECKOUT_TITLE', { count: this.cart.itemCount() }),
      priceKm: this.cart.total(),
      cartItems: this.cart.items().map(item => ({ adId: item.productId, quantity: item.quantity })),
      onSuccess: () => {
        this.cart.clear();
        this.isCartOpen = false;
      }
    });
  }

  toggleLangMenu() {
    this.isLangOpen = !this.isLangOpen;
    this.isProfileOpen = false;
  }

  toggleProfile() {
    this.isProfileOpen = !this.isProfileOpen;
    this.isLangOpen = false;
  }

  logout() {
    this.auth.logout().subscribe();
    this.isProfileOpen = false;
    this.router.navigate(['/']);
  }

  setLang(lang: AppLang) {
    this.locale.setLang(lang);
    this.isLangOpen = false;
  }
}
