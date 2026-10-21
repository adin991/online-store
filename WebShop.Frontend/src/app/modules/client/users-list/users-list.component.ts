import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { ListUserDto, ListUsersRequest, UserApiService } from '../../../api-services/user/user-api.service';
import { BaseListPagedComponent } from '../../../core/components/base-classes/base-list-paged-component';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { LocaleService } from '../../../core/services/locale.service';

/** User administration (admin only - route guard + API check): search, role filter, sort, paging, delete. */
@Component({
  selector: 'app-users-list',
  standalone: false,
  templateUrl: './users-list.component.html',
  styleUrl: '../dashboard/dashboard.component.scss'
})
export class UsersListComponent extends BaseListPagedComponent<ListUserDto, ListUsersRequest> implements OnInit {
  auth = inject(AuthFacadeService);
  private userApi = inject(UserApiService);
  private toaster = inject(ToasterService);
  private locale = inject(LocaleService);
  private fb = inject(FormBuilder);
  private destroyRef = inject(DestroyRef);

  busyUserId: number | null = null;

  filters = this.fb.nonNullable.group({
    search: [''],
    role: [''],
    sortBy: ['name']
  });

  constructor() {
    super();
    this.request = new ListUsersRequest();
    this.request.paging.pageSize = 10;
  }

  ngOnInit(): void {
    this.filters.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged((a, b) => JSON.stringify(a) === JSON.stringify(b)),
            takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applyFilters());

    this.initList();
  }

  protected loadPagedData(): void {
    const f = this.filters.getRawValue();
    this.request.search = f.search.trim() || null;
    this.request.isAdmin = f.role === 'admin' ? true : f.role === 'user' ? false : null;
    this.request.sortBy = f.sortBy;

    this.startLoading();
    this.userApi.list(this.request).subscribe({
      next: (res) => {
        this.handlePageResult(res);
        this.stopLoading();
      },
      error: (err) => this.stopLoading(err?.error?.message ?? this.locale.t('USERS.LOAD_FAILED'))
    });
  }

  resetFilters(): void {
    this.filters.reset({ search: '', role: '', sortBy: 'name' });
  }

  deleteUser(user: ListUserDto): void {
    if (user.id === this.auth.currentUser()?.userId) return;
    if (!confirm(this.locale.t('USERS.DELETE_CONFIRM', { name: `${user.name} ${user.surname}` }))) return;

    this.busyUserId = user.id;
    this.userApi.delete(user.id).subscribe({
      next: () => {
        this.busyUserId = null;
        this.toaster.success(this.locale.t('USERS.DELETED'));
        if (this.items.length === 1 && this.paging.page > 1) this.paging.page--;
        this.loadPagedData();
      },
      error: (err) => {
        this.busyUserId = null;
        this.toaster.error(err?.error?.message ?? this.locale.t('COMMON.DELETE_FAILED'));
      }
    });
  }
}
