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
  const userId = this.auth.getCurrentUser();
  console.log("🔄 טוען עגלה עבור משתמש:", userId); // <--- הוסף את זה!

  if (userId > 0) {
    this.giftsService.getCart(userId).subscribe(items => {
      console.log("📦 מוצרים שהגיעו מהשרת:", items); // <--- הוסף את זה!
      this.cartItems = items;
      this.calculateTotal();
    });
  }
}

  calculateTotal() {
    this.totalPrice = this.cartItems.reduce((sum, item) => sum + (item.price * (item.quantity || 1)), 0);
  }

onCheckout() {
    const userId = this.auth.getCurrentUser(); 
    
    console.log("🚀 מנסה לבצע תשלום עבור משתמש מספר:", userId);

    if (!userId || userId <= 0) {
        alert("עליך להתחבר מחדש כדי לבצע תשלום");
        return;
    }

    this.giftsService.checkout(userId).subscribe({
      next: (res) => {
        console.log("✅ תשלום עבר בהצלחה:", res);
        alert('התשלום בוצע וההזמנה נרשמה!');
        
        // ניקוי העגלה בצד לקוח
        this.cartItems = [];
        this.totalPrice = 0;
      },
      error: (err) => {
        console.error("❌ שגיאה בתשלום:", err);
        alert("אירעה שגיאה בביצוע התשלום. נסה שוב.");
      }
    });
  }
}