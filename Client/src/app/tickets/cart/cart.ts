import { Component, OnInit } from '@angular/core';
import { GiftsService, CartItem } from '../../services/gifts';
import { Auth } from '../../services/auth';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart.html'
})
export class CartComponent implements OnInit {
  cartItems: any[] = [];
  totalPrice: number = 0;

  constructor(private giftsService: GiftsService, private auth: Auth) {}

  ngOnInit() {
    this.loadCart();
  }

  loadCart() {
    const userId = this.auth.getCurrentUser()?.id;
    if (userId) {
      this.giftsService.getCart(userId).subscribe(items => {
        this.cartItems = items;
        this.calculateTotal();
      });
    }
  }

  calculateTotal() {
    this.totalPrice = this.cartItems.reduce((sum, item) => sum + (item.price * (item.quantity || 1)), 0);
  }

  onCheckout() {
    const userId = this.auth.getCurrentUser()?.id;
    this.giftsService.checkout(userId).subscribe(() => {
      alert('התשלום בוצע בהצלחה.');
      this.cartItems = [];
      this.totalPrice = 0;
    });
  }
}