export interface CartItem {
  /** Unique line key: the same ad with different options is a separate line. */
  key: string;
  productId: number;
  name: string;
  price: number;
  quantity: number;
  color?: string | null;
  size?: string | null;
}

export interface CartProduct {
  id: number;
  name: string;
  price: number;
  color?: string | null;
  size?: string | null;
}

export function cartKey(productId: number, color?: string | null, size?: string | null): string {
  return `${productId}|${color ?? ''}|${size ?? ''}`;
}
