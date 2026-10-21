import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import {PublicLayoutComponent} from './public-layout/public-layout.component';
import { PublicHomeComponent } from './public-home/public-home.component';
import { ProductDetailComponent } from './product-detail/product-detail.component';


const routes: Routes = [
  {
    path:'',
    component: PublicLayoutComponent,
    children: [
      {
        path: '',
        component: PublicHomeComponent
      },
      {
        path: 'ad/:id',
        component: ProductDetailComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicRoutingModule { }
