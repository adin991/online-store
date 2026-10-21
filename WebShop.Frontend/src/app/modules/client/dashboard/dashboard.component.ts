import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder } from '@angular/forms';
import { debounceTime, distinctUntilChanged, forkJoin } from 'rxjs';
import { AdStatusType, AdsApiService, ListAdDto, ListAdsRequest } from '../../../api-services/ads/ads-api.service';
import { BaseListPagedComponent } from '../../../core/components/base-classes/base-list-paged-component';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { CartService } from '../../../core/services/cart/cart.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { LocaleService } from '../../../core/services/locale.service';

interface DashboardStats {
  total: number;
  live: number;
  closed: number;
}

/**
 * Seller dashboard: summary of the user's ads and a paged, filterable list of them
 * (GET /Ads?mine=true) with status actions (owner rule enforced by the API).
 */
@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent extends BaseListPagedComponent<ListAdDto, ListAdsRequest> implements OnInit {
  auth = inject(AuthFacadeService);
  cart = inject(CartService);
  private adsApi = inject(AdsApiService);
  private toaster = inject(ToasterService);
  private locale = inject(LocaleService);
  private fb = inject(FormBuilder);
  private destroyRef = inject(DestroyRef);

  stats: DashboardStats = { total: 0, live: 0, closed: 0 };
  busyAdId: number | null = null;

  readonly statusOptions: AdStatusType[] = ['Live', 'Closed', 'AwaitingReview', 'Removed', 'ShadowBanned'];

  filters = this.fb.nonNullable.group({
    search: [''],
    status: ['' as AdStatusType | ''],
    sortBy: ['newest']
  });

  constructor() {
    super();
    this.request = new ListAdsRequest();
    this.request.mine = true;
    this.request.paging.pageSize = 5;
  }

  ngOnInit(): void {
    this.filters.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b)),
            takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applyFilters());

    this.loadStats();
    this.initList();
  }

  protected loadPagedData(): void {
    const f = this.filters.getRawValue();
    this.request.search = f.search.trim() || null;
    this.request.status = f.status || null;
    this.request.sortBy = f.sortBy || null;

    this.startLoading();
    this.adsApi.list(this.request).subscribe({
      next: (res) => {
        this.handlePageResult(res);
        this.stopLoading();
      },
      error: () => this.stopLoading(this.locale.t('DASHBOARD.LOAD_FAILED'))
    });
  }

  private loadStats(): void {
    const count = (status: AdStatusType | null) => {
      const r = new ListAdsRequest();
      r.mine = true;
      r.status = status;
      r.paging.pageSize = 1;
      return this.adsApi.list(r);
    };

    forkJoin([count(null), count('Live'), count('Closed')]).subscribe({
      next: ([all, live, closed]) => this.stats = { total: all.total, live: live.total, closed: closed.total }
    });
  }

  resetFilters(): void {
    this.filters.reset({ search: '', status: '', sortBy: 'newest' });
  }

  /** Owner actions: close a live ad or put a closed (unsold) ad back on sale. */
  setStatus(ad: ListAdDto, status: AdStatusType): void {
    this.busyAdId = ad.id;
    this.adsApi.changeStatus(ad.id, status).subscribe({
      next: () => {
        this.busyAdId = null;
        this.toaster.success(this.locale.t('DASHBOARD.STATUS_CHANGED'));
        this.loadStats();
        this.loadPagedData();
      },
      error: (err) => {
        this.busyAdId = null;
        this.toaster.error(err?.error?.message ?? this.locale.t('DASHBOARD.STATUS_CHANGE_FAILED'));
      }
    });
  }

  deleteAd(ad: ListAdDto): void {
    if (!confirm(this.locale.t('AD_DETAIL.DELETE_CONFIRM'))) return;
    this.busyAdId = ad.id;
    this.adsApi.delete(ad.id).subscribe({
      next: () => {
        this.busyAdId = null;
        this.loadStats();
        // step back if the last item of the page was deleted
        if (this.items.length === 1 && this.paging.page > 1) this.paging.page--;
        this.loadPagedData();
      },
      error: (err) => {
        this.busyAdId = null;
        this.toaster.error(err?.error?.message ?? this.locale.t('COMMON.DELETE_FAILED'));
      }
    });
  }
}
