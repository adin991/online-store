import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { AdsApiService, ListAdDto, ListAdsRequest } from '../../../api-services/ads/ads-api.service';
import { BaseListPagedComponent } from '../../../core/components/base-classes/base-list-paged-component';

@Component({
  selector: 'app-public-home',
  standalone: false,
  templateUrl: './public-home.component.html',
  styleUrl: './public-home.component.scss'
})
export class PublicHomeComponent extends BaseListPagedComponent<ListAdDto, ListAdsRequest> implements OnInit {
  selectedCategory: string | null = null;
  selectedCategoryName: string | null = null;
  searchTerm: string | null = null;

  readonly locations = ['Sarajevo', 'Mostar', 'Banja Luka'];
  readonly pageSizes = [12, 24, 48];

  private readonly categoryMap: Record<string, string> = {
    'CATEGORIES.ELECTRONICS': 'Electronics',
    'CATEGORIES.CARS': 'Cars',
    'CATEGORIES.FASHION': 'Fashion',
    'CATEGORIES.HOME': 'Home',
    'CATEGORIES.GAMING': 'Gaming',
    'CATEGORIES.BOOKS': 'Books',
    'CATEGORIES.SPORTS': 'Sports',
    'CATEGORIES.BEAUTY': 'Beauty',
  };

  private fb = inject(FormBuilder);
  private adsApi = inject(AdsApiService);
  private route = inject(ActivatedRoute);
  private destroyRef = inject(DestroyRef);

  /** Filter form - bound to the ListAdsQuery parameters of the API. */
  filters = this.fb.group({
    minPrice: this.fb.control<number | null>(null, [Validators.min(0)]),
    maxPrice: this.fb.control<number | null>(null, [Validators.min(0)]),
    location: this.fb.nonNullable.control(''),
    sortBy: this.fb.nonNullable.control('default')
  });

  constructor() {
    super();
    this.request = new ListAdsRequest();
    this.request.paging.pageSize = this.pageSizes[0];
  }

  ngOnInit(): void {
    this.filters.valueChanges
      .pipe(debounceTime(400), distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b)),
            takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        if (this.filters.valid) this.applyFilters();
      });

    // ?search=... comes from the search bar in the navbar
    this.route.queryParamMap
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        this.searchTerm = params.get('search');
        this.applyFilters();
      });
  }

  get totalCount(): number {
    return this.totalItems;
  }

  selectCategory(key: string): void {
    if (this.selectedCategory === key) {
      this.selectedCategory = null;
      this.selectedCategoryName = null;
    } else {
      this.selectedCategory = key;
      this.selectedCategoryName = this.categoryMap[key] ?? null;
    }
    this.applyFilters();
  }

  resetFilters(): void {
    this.filters.reset({ minPrice: null, maxPrice: null, location: '', sortBy: 'default' });
  }

  clearCategory(): void {
    this.selectedCategory = null;
    this.selectedCategoryName = null;
    this.resetFilters();
    this.applyFilters();
  }

  protected loadPagedData(): void {
    const f = this.filters.getRawValue();
    const r = this.request;

    r.search = this.searchTerm || null;
    r.categoryName = this.selectedCategoryName;
    r.minPrice = f.minPrice != null && f.minPrice >= 0 ? f.minPrice : null;
    r.maxPrice = f.maxPrice != null && f.maxPrice >= 0 ? f.maxPrice : null;
    r.location = f.location || null;
    r.sortBy = f.sortBy !== 'default' ? f.sortBy : null;

    this.startLoading();
    this.adsApi.list(r).subscribe({
      next: (res) => {
        this.handlePageResult(res);
        this.stopLoading();
      },
      error: () => this.stopLoading()
    });
  }

  onPageChange(page: number): void {
    this.goToPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
