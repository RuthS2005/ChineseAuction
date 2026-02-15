import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
// שינוי 1: מייבאים את Gift מהקובץ של הסרוויס במקום להגדיר אותו מחדש
import { GiftsService, Gift } from '../../services/gifts'; 
import { Auth } from '../../services/auth';  

// --- מחקתי מכאן את ה-interface Gift שהיה מיותר ---

@Component({
  selector: 'app-gift-list',
  standalone: true,
  imports: [CommonModule, FormsModule], 
  templateUrl: './gift-list.html',
  styleUrls: ['./gift-list.scss'],
})
export class GiftList implements OnInit {
  
  gifts: Gift[] = []; // עכשיו זה משתמש ב-Gift המקורי והנכון מהסרוויס
  originalGifts: Gift[] = [];
  
  searchTerm: string = '';
  sortBy: string = '';

  @Output() itemAddedToCart = new EventEmitter<void>();

  constructor(
    private giftsService: GiftsService,
    private authService: Auth
  ) {}

  ngOnInit() {
    this.loadGifts();
  }

  loadGifts() {
    this.giftsService.getGifts().subscribe(data => {
      this.originalGifts = data;
      this.gifts = data;
    });
  }

  applyFilter() {
    let tempGifts = [...this.originalGifts];

    if (this.searchTerm.trim() !== '') {
      const term = this.searchTerm.toLowerCase();
      tempGifts = tempGifts.filter(gift => 
        gift.name.toLowerCase().includes(term) || 
        (gift.description && gift.description.toLowerCase().includes(term))
      );
    }

    if (this.sortBy === 'expensive') {
      tempGifts.sort((a, b) => b.cost - a.cost); 
    } 
    else if (this.sortBy === 'popular') {
      // אם ticketCount לא קיים ב-Interface המקורי, זה עלול לצעוק
      // אם זה צועק, תוסיף ticketCount?: number ל-interface בסרוויס
      tempGifts.sort((a, b) => (b['ticketCount'] || 0) - (a['ticketCount'] || 0));
    }
    else if (this.sortBy === 'cheap') {
      tempGifts.sort((a, b) => a.cost - b.cost);
    }

    this.gifts = tempGifts;
  }

  onSortChange(event: any) {
    this.sortBy = event.target.value;
    this.applyFilter();
  }

  buyTickets(gift: Gift, quantityInput: HTMLInputElement) {
    const qty = parseInt(quantityInput.value);
    
    // הנחתי שזה שירות שקיים אצלך לפי הקוד
    const userId = this.authService.getCurrentUser(); // וודא שזה מחזיר ID או מספר

    // בדיקה פשוטה למקרה שאין משתמש (תלוי איך המערכת שלך עובדת)
    if (!userId) {
        alert("עליך להתחבר למערכת כדי לבצע רכישה!");
        return;
    }

    if (!qty || qty < 1 || isNaN(qty)) {
        alert("נא לבחור כמות תקינה");
        return;
    }

    const purchaseRequest = {
      userId: userId, // שים לב שזה מספר
      giftId: gift.id,   
      quantity: qty      
    };

    console.log("🚀 שולח לשרת:", purchaseRequest); 

    this.giftsService.addToCart(purchaseRequest).subscribe({
      next: () => {
        alert("הכרטיסים נוספו לסל בהצלחה!");
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