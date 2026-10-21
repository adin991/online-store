import { Component, inject } from '@angular/core';
import { OrderModalService } from '../../services/order-modal.service';

@Component({
  selector: 'app-order-modal',
  standalone: false,
  templateUrl: './order-modal.component.html',
  styleUrl: './order-modal.component.scss'
})
export class OrderModalComponent {
  modal = inject(OrderModalService);
}
