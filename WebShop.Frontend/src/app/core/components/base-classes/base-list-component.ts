// src/app/core/components/base-list.component.ts

import { BaseComponent } from './base-component';

export abstract class BaseListComponent<TItem> extends BaseComponent {
  items: TItem[] = [];

  /**
   * The concrete data loading is left to the child class.
   */
  protected abstract loadData(): void;

  /**
   * Helper that can be called from the child's ngOnInit.
   */
  protected initList(): void {
    this.loadData();
  }
}
