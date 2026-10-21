import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductAddComponent } from './product-add/product-add.component';
import { ProfileEditComponent } from './profile-edit/profile-edit.component';
import { AdEditComponent } from './ad-edit/ad-edit.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { UsersListComponent } from './users-list/users-list.component';
import { myAuthData, myAuthGuard } from '../../core/guards/my-auth-guard';

// Every client screen is private: the guard redirects anonymous users to /auth/login?returnUrl=...
const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [myAuthGuard],
    data: myAuthData({ requireAuth: true })
  },
  {
    path: 'product-add',
    component: ProductAddComponent,
    canActivate: [myAuthGuard],
    data: myAuthData({ requireAuth: true })
  },
  {
    path: 'profile-edit',
    component: ProfileEditComponent,
    canActivate: [myAuthGuard],
    data: myAuthData({ requireAuth: true })
  },
  {
    path: 'ad-edit/:id',
    component: AdEditComponent,
    canActivate: [myAuthGuard],
    data: myAuthData({ requireAuth: true })
  },
  {
    path: 'users',
    component: UsersListComponent,
    canActivate: [myAuthGuard],
    data: myAuthData({ requireAuth: true, requireAdmin: true })
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ClientRoutingModule { }
