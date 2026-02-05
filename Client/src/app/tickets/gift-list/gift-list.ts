import { Component, Output, EventEmitter } from '@angular/core'; // 1. הוספת Output, EventEmitter
import { CommonModule } from '@angular/common';
import { GiftsService, Gift } from '../../services/gifts';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-gift-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './gift-list.html',
  styleUrls: ['./gift-list.scss'],
})
export class GiftList {
  
  gifts: Gift[] = [];

  // 2. יצירת האירוע
  @Output() itemAddedToCart = new EventEmitter<void>();

  constructor(
    private giftsService: GiftsService,
    private authService: Auth
  ) {
    this.giftsService.getGifts().subscribe(data => this.gifts = data);
  }

  buyTickets(gift: Gift, quantityInput: HTMLInputElement) {
    // ... (הבדיקות הרגילות של כמות ומשתמש מחובר) ...
    const qty = parseInt(quantityInput.value);
    const userId = this.authService.getCurrentUser()

    const purchaseRequest = {
      userId: Number(userId), // 👈 התיקון הקריטי: מכריחים אותו להיות מספר!
      giftId: gift.id,
      quantity: qty
    };
    this.giftsService.addToCart(purchaseRequest).subscribe({
      next: () => {
        alert("הכרטיסים נוספו לסל בהצלחה!");
        quantityInput.value = '';

        // 3. הנה השינוי! אנחנו מודיעים לאבא שסיימנו
        this.itemAddedToCart.emit(); 
      },
      error: (err) => alert("שגיאה: " + err.message)
    });
  }
}