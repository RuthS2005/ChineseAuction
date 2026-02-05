import { Component } from '@angular/core';
import { GiftList } from "./gift-list/gift-list";
import { CartComponent } from './cart/cart';

@Component({
  selector: 'app-tickets',
  imports: [GiftList, CartComponent],
  templateUrl: './tickets.html',
  styleUrl: './tickets.scss',
})
export class Tickets {

}
