import { Component, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; 
import { GiftsService } from '../../services/gifts'; // וודאי שהנתיב נכון
import { Auth } from '../../services/auth'; // וודאי שהנתיב נכון

// הגדרת המנשק למתנה (אפשר גם בקובץ נפרד)
export interface Gift {
  id: number;
  name: string;
  description: string;
  category: string;
  cost: number;
  imageUrl: string;
  ticketCount?: number; // אופציונלי - למיון לפי פופולריות
}

@Component({
  selector: 'app-gift-list',
  standalone: true,
  imports: [CommonModule, FormsModule], 
  templateUrl: './gift-list.html',
  styleUrls: ['./gift-list.scss'],
})
export class GiftList implements OnInit {
  
  // המערך שמוצג למשתמש (מסונן)
  gifts: Gift[] = [];

  // המערך המקורי (גיבוי מלא מהשרת) - כדי שהחיפוש לא "יאבד" מתנות
  originalGifts: Gift[] = [];
  
  // משתנים לחיפוש ומיון
  searchTerm: string = '';
  sortBy: string = '';

  // אירוע לעדכון העגלה
  @Output() itemAddedToCart = new EventEmitter<void>();

  constructor(
    private giftsService: GiftsService,
    private authService: Auth
  ) {}

  ngOnInit() {
    this.loadGifts();
  }

  // --- לוגיקת טעינה, חיפוש ומיון (צד לקוח) ---
  
  loadGifts() {
    // טוענים את הכל מהשרת פעם אחת
    this.giftsService.getGifts().subscribe(data => {
      this.originalGifts = data; // שומרים בגיבוי
      this.gifts = data;         // מציגים הכל בהתחלה
    });
  }

  // הפונקציה הזו מופעלת בכל הקלדה או שינוי מיון
  applyFilter() {
    // 1. מתחילים תמיד מהרשימה המקורית המלאה
    let tempGifts = [...this.originalGifts];

    // 2. סינון לפי טקסט (חיפוש)
    if (this.searchTerm.trim() !== '') {
      const term = this.searchTerm.toLowerCase();
      tempGifts = tempGifts.filter(gift => 
        gift.name.toLowerCase().includes(term) || 
        (gift.description && gift.description.toLowerCase().includes(term))
      );
    }

    // 3. מיון
    if (this.sortBy === 'expensive') {
      // מהיקר לזול
      tempGifts.sort((a, b) => b.cost - a.cost); 
    } 
    else if (this.sortBy === 'popular') {
      // מהנמכר ביותר (אם אין שדה כזה, זה לא ישנה כלום)
      tempGifts.sort((a, b) => (b.ticketCount || 0) - (a.ticketCount || 0));
    }
    else if (this.sortBy === 'cheap') {
      // מהזול ליקר
      tempGifts.sort((a, b) => a.cost - b.cost);
    }

    // 4. עדכון התצוגה
    this.gifts = tempGifts;
  }

  onSortChange(event: any) {
    this.sortBy = event.target.value;
    this.applyFilter(); // הפעלת הסינון מחדש
  }

  // --- לוגיקת רכישה ---

  buyTickets(gift: Gift, quantityInput: HTMLInputElement) {
    // 1. המרה למספרים
    const qty = parseInt(quantityInput.value);
    
    // תיקון חשוב: שימוש ב-getCurrentUserId שמחזיר מספר
    const userId = this.authService.getCurrentUser(); 

    // 2. בדיקות
    if (!userId || userId <= 0) {
        alert("עליך להתחבר למערכת כדי לבצע רכישה!");
        return;
    }

    if (!qty || qty < 1 || isNaN(qty)) {
        alert("נא לבחור כמות תקינה");
        return;
    }

    // 3. בניית האובייקט
    const purchaseRequest = {
      userId: userId, 
      giftId: gift.id,   
      quantity: qty      
    };

    console.log("🚀 שולח לשרת:", purchaseRequest); 

    // 4. שליחה
    this.giftsService.addToCart(purchaseRequest).subscribe({
      next: () => {
        alert("הכרטיסים נוספו לסל בהצלחה!");
        quantityInput.value = '';
        this.itemAddedToCart.emit(); // עדכון העגלה בצד שמאל
      },
      error: (err) => {
        console.error("❌ שגיאה מהשרת:", err);
        const msg = err.error?.errors ? JSON.stringify(err.error.errors) : (err.error?.message || err.message);
        alert("שגיאה בהוספה: " + msg);
      }
    });
  }
}