import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

import { ClientRoutingModule } from './client-routing.module';
import { ProductAddComponent } from './product-add/product-add.component';
import { ProfileEditComponent } from './profile-edit/profile-edit.component';
import { AdEditComponent } from './ad-edit/ad-edit.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { UsersListComponent } from './users-list/users-list.component';
import { FieldErrorComponent } from '../../core/components/field-error/field-error.component';
import { PaginatorComponent } from '../../core/components/paginator/paginator.component';
import { MoneyPipe } from '../../core/pipes/money.pipe';
import { LocalDatePipe } from '../../core/pipes/local-date.pipe';


@NgModule({
  declarations: [
    ProductAddComponent,
    ProfileEditComponent,
    AdEditComponent,
    DashboardComponent,
    UsersListComponent
  ],
  imports: [
    CommonModule,
    ClientRoutingModule,
    RouterModule,
    ReactiveFormsModule,
    TranslateModule,
    FieldErrorComponent,
    PaginatorComponent,
    MoneyPipe,
    LocalDatePipe
  ]
})
export class ClientModule { }
