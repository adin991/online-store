import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { PublicRoutingModule } from './public-routing.module';
import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { PublicHomeComponent } from './public-home/public-home.component';
import { PublicEndComponent } from './public-end/public-end.component';
import { ProductDetailComponent } from './product-detail/product-detail.component';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { ClickOutsideDirective } from '../../core/directives/click-outside.directive';
import { FieldErrorComponent } from '../../core/components/field-error/field-error.component';
import { PaginatorComponent } from '../../core/components/paginator/paginator.component';
import { MoneyPipe } from '../../core/pipes/money.pipe';
import { LocalDatePipe } from '../../core/pipes/local-date.pipe';


@NgModule({
  declarations: [
    PublicLayoutComponent,
    PublicHomeComponent,
    PublicEndComponent,
    ProductDetailComponent,
    ClickOutsideDirective
  ],
  imports: [
    CommonModule,
    PublicRoutingModule,
    RouterModule,
    ReactiveFormsModule,
    TranslateModule,
    FieldErrorComponent,
    PaginatorComponent,
    MoneyPipe,
    LocalDatePipe
  ]
})
export class PublicModule { }
