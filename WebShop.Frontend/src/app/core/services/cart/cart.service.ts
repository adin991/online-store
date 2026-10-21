import { Injectable, signal, computed } from '@angular/core';
import { CartItem, CartProduct, cartKey } from './cart.models';

const MAX_QTY = 999;

/**
 * Shopping cart kept in localStorage, with two separate lists:
 * - items: what will be paid on checkout
 * - saved: "save for later" - kept aside, not part of the total or the checkout
 */
@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly STORAGE_KEY = 'market_cart_v2';
  private readonly SAVED_KEY = 'market_cart_saved_v1';

  private cartItems = signal<CartItem[]>([]);
  private savedItems = signal<CartItem[]>([]);

  // Readonly signals for components
  items = this.cartItems.asReadonly();
  saved = this.savedItems.asReadonly();

  itemCount = computed(() => this.cartItems().length);
  savedCount = computed(() => this.savedItems().length);

  total = computed(() =>
    this.cartItems().reduce((sum, item) => sum + (item.price * item.quantity), 0)
  );

  constructor() {
    this.cartItems.set(this.load(this.STORAGE_KEY));
    this.savedItems.set(this.load(this.SAVED_KEY));
  }

  /**
   * Add a product (with the selected options) to the cart. If the same line exists, increment quantity.
   */
  add(product: CartProduct, qty = 1): void {
    const key = cartKey(product.id, product.color, product.size);
    const items = [...this.cartItems()];
    const index = items.findIndex(i => i.key === key);

    if (index >= 0) {
      items[index] = { ...items[index], quantity: clamp(items[index].quantity + qty) };
    } else {
      items.push({
        key,
        productId: product.id,
        name: product.name,
        price: product.price,
        quantity: clamp(qty),
        color: product.color ?? null,
        size: product.size ?? null
      });
    }

    this.setItems(items);
  }

  /**
   * Set exact quantity for a cart line (clamp 1-999)
   */
  setQuantity(key: string, qty: number): void {
    this.setItems(this.cartItems().map(i => i.key === key ? { ...i, quantity: clamp(qty) } : i));
  }

  /**
   * Remove a line from the cart
   */
  remove(key: string): void {
    this.setItems(this.cartItems().filter(i => i.key !== key));
  }

  /**
   * Clear the cart (the "saved for later" list is kept)
   */
  clear(): void {
    this.setItems([]);
  }

  /** Move a cart line to the "saved for later" list. */
  saveForLater(key: string): void {
    const item = this.cartItems().find(i => i.key === key);
    if (!item) return;
    this.setItems(this.cartItems().filter(i => i.key !== key));
    this.setSaved([...this.savedItems().filter(i => i.key !== key), item]);
  }

  /** Move a saved line back to the cart (merges with an existing line). */
  moveToCart(key: string): void {
    const item = this.savedItems().find(i => i.key === key);
    if (!item) return;
    this.setSaved(this.savedItems().filter(i => i.key !== key));
    this.add({ id: item.productId, name: item.name, price: item.price, color: item.color, size: item.size }, item.quantity);
  }

  removeSaved(key: string): void {
    this.setSaved(this.savedItems().filter(i => i.key !== key));
  }

  private setItems(items: CartItem[]): void {
    this.cartItems.set(items);
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(items));
  }

  private setSaved(items: CartItem[]): void {
    this.savedItems.set(items);
    localStorage.setItem(this.SAVED_KEY, JSON.stringify(items));
  }

  private load(storageKey: string): CartItem[] {
    const data = localStorage.getItem(storageKey);
    if (!data) return [];
    try {
      const items = JSON.parse(data) as CartItem[];
      return items.map(i => ({ ...i, key: i.key ?? cartKey(i.productId, i.color, i.size) }));
    } catch (error) {
      console.error('Failed to load cart from storage', error);
      return [];
    }
  }
}

function clamp(qty: number): number {
  return Math.max(1, Math.min(Math.floor(qty) || 1, MAX_QTY));
}
