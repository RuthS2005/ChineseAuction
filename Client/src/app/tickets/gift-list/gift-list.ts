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
    // 1. המרה למספרים שלמים (Integer)
    const qty = parseInt(quantityInput.value);
    const rawUserId = this.authService.getCurrentUser()
    const userId = parseInt(rawUserId.toString()); // וידוא הריגה שזה מספר

    // 2. בדיקות
    if (!userId || userId <= 0 || isNaN(userId)) {
        alert("עליך להתחבר למערכת כדי לבצע רכישה!");
        return;
    }

    if (!qty || qty < 1 || isNaN(qty)) {
        alert("נא לבחור כמות תקינה");
        return;
    }

    // 3. בניית האובייקט - הכל מספרים!
    const purchaseRequest = {
      userId: userId, 
      giftId: gift.id,   
      quantity: qty      
    };

    // בדיקה בקונסול - תראה שהמספרים צבועים בכחול (מספר) ולא בשחור/אדום (טקסט)
    console.log("🚀 שולח לשרת:", purchaseRequest); 

    // 4. שליחה
    this.giftsService.addToCart(purchaseRequest).subscribe({
      next: () => {
        quantityInput.value = '';
        this.itemAddedToCart.emit();
      },
      error: (err) => {
        console.error("❌ שגיאה מהשרת:", err);
        const msg = err.error?.errors ? JSON.stringify(err.error.errors) : (err.error?.message || err.message);
        alert("שגיאה בהוספה: " + msg);
      }
    });
}
}
