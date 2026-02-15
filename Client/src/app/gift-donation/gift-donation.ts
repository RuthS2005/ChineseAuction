import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GiftsList } from './gift-list/gift-list';
import { GiftForm } from './gift-form/gift-form';
import { GiftsService } from '../services/gifts';

@Component({
  selector: 'app-gifts-management',
  standalone: true,
  imports: [CommonModule, GiftsList, GiftForm],
  templateUrl: './gift-donation.html',
  styleUrls: ['./gift-donation.scss'],
})
export class GiftDonation {
  gifts: any[] = [];
  selectedGift: any = null;
  showForm = false;

  constructor(private giftsService: GiftsService) {
      this.giftsService.getGifts().subscribe(data => {
      this.gifts = data;
    });
  }

  saveGift(giftFromForm: any) {

    // בדיקה: האם במצב עריכה? (האם יש ID למתנה שנבחרה?)
    if (this.selectedGift && this.selectedGift.id) {

      // --- לוגיקה של עדכון (UPDATE) ---

      const giftToUpdate = { ...giftFromForm, id: this.selectedGift.id };

      this.giftsService.updateGift(giftToUpdate).subscribe({
        next: (response) => {
          // עדכון הרשימה המקומית בתצוגה (בלי לטעון מחדש מהשרת)
          const index = this.gifts.findIndex(g => g.id === this.selectedGift.id);
          if (index !== -1) {
            this.gifts[index] = giftToUpdate; // מחליפים את הישן בחדש
          }

          this.showForm = false;
          this.selectedGift = null;
          alert("המתנה עודכנה בהצלחה!");
        },
        error: (err) => alert("שגיאה בעדכון: " + err.message)
      });

    } else {

      // --- לוגיקה של הוספה (ADD) ---
      this.giftsService.addGift(giftFromForm).subscribe({
        next: (response: any) => {
          const newGift = { ...giftFromForm, id: response.id || response };
          this.gifts.push(newGift);

          this.showForm = false;
          this.selectedGift = null;
        },
        error: (err) => {
          console.error(err);
          alert("שגיאה בהוספת מתנה. בדוק אם נבחר תורם!");
        }
      });
    }
  }
  removeGift(giftId: number) {

    if (!confirm("האם אתה בטוח שברצונך למחוק מתנה זו?")) return;

    //  קריאה לשרת למחוק את המתנה
    this.giftsService.deleteGift(giftId).subscribe({
      next: () => {

        //  מציאת המיקום האמיתי של המתנה במערך לפי ה-ID
        const index = this.gifts.findIndex(g => g.id === giftId);

        // אם מצאנו את המתנה ברשימה - נמחק אותה ויזואלית
        if (index !== -1) {
          this.gifts.splice(index, 1);
        }
      },
      error: (err) => alert("שגיאה במחיקה: " + err.message)
    });
  }
  startEdit(gift: any) {
    this.selectedGift = gift;
    this.showForm = true;
  }
  raffle(giftId: number) {
    console.log(`מבצע הגרלה למתנה עם מזהה: ${giftId}`);
    this.giftsService.raffle(giftId).subscribe({
      next: (res) => {
        alert(`🎉 הזוכה המאושר הוא: ${res.winnerName}\nמייל: ${res.email}`);
      },
      error: (err) => {
        console.log(err); 

        if (err.status === 403) {
          alert("⛔ אין לך הרשאת מנהל לביצוע הגרלה!");
        }
        else {
          const msg = err.error?.Error || err.error?.message || err.message || "שגיאה לא ידועה";
          alert("שגיאה: " + msg);
        }
      }
    });
  }
}