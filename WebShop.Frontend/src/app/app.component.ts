import { Component, inject } from '@angular/core';
import { LocaleService } from './core/services/locale.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'WebShop.Frontend';

  // Created at startup so the saved language / regional setting is applied on every route.
  private locale = inject(LocaleService);
}
