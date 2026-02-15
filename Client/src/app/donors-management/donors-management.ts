import { Component } from '@angular/core';
import { DonorForm } from './donor-form/donor-form';
import { DonorsList } from './donors-list/donors-list';
import { CommonModule } from '@angular/common';
import { GiftsService } from '../services/gifts';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-donors-management',
  standalone: true,
  imports: [CommonModule, FormsModule, DonorForm, DonorsList],
  templateUrl: './donors-management.html',
  styleUrls: ['./donors-management.scss'],
})
export class DonorsManagement {
  showForm = false;
  donors: any[] = [];
  searchTerm: string = ''; // המשתנה שיתחבר לתיבת החיפוש
  selectedDonor: any = null; // שומר את המקור (כדי לדעת אם זה עדכון)
  tempDonor: any = {};       // המשתנה שנשלח לטופס (מונע את הבאג של האיפוס)

  constructor(private giftService: GiftsService) {
    this.loadDonors(); // טוען את התורמים כשהקומפוננט נטען
  }


  // פונקציה ראשית לטעינה (עם או בלי חיפוש)
  loadDonors() {
    this.giftService.getDonors(this.searchTerm).subscribe(data => {
      this.donors = data;
    });
  }

  // פונקציה שתופעל כשהמשתמש מקליד
  onSearch() {
    this.loadDonors();
  }

  // --- פתיחת טופס להוספה חדשה ---
  addNewDonor() {
    this.selectedDonor = null;
    // מאתחלים אובייקט ריק לטופס
    this.tempDonor = { name: '', phone: '', email: '' };
    this.showForm = true;
  }

  // --- פתיחת טופס לעריכה ---
  startEdit(donor: any) {
    this.selectedDonor = donor; // שומרים רפרנס למקורי
    // מעתיקים את הנתונים לתוך המשתנה הזמני
    this.tempDonor = { ...donor };
    this.showForm = true;
  }

  // --- שמירה (גם הוספה וגם עדכון) ---
  saveDonor(donorFromForm: any) {
    console.log("האבא קיבל מהטופס:", donorFromForm);

    if (this.selectedDonor) {
      // ============ לוגיקת עדכון ============
      const originalId = this.selectedDonor.id;

      // מכינים אובייקט לעדכון עם ה-ID המקורי
      const donorToUpdate = { ...donorFromForm, id: originalId };

      this.giftService.updateDonor(donorToUpdate).subscribe({
        next: () => {
          // מציאת האינדקס ברשימה המקומית
          const index = this.donors.findIndex(d => d.id == originalId);

          if (index !== -1) {
            // עדכון הרשימה בתצוגה
            this.donors[index] = donorToUpdate;
            // רענון הטבלה באנגולר (יצירת רפרנס חדש למערך)
            this.donors = [...this.donors];
            console.log("עודכן בהצלחה ברשימה!");
          }

          this.closeForm();
        },
        error: (err) => alert("שגיאה בעדכון: " + err.message)
      });

    } else {
      // ============ לוגיקת הוספה ============
      this.giftService.addDonor(donorFromForm).subscribe({
        next: (response: any) => {
          // השרת מחזיר את ה-ID החדש
          const donorDisplay = {
            ...donorFromForm,
            id: response.id || response.Id // תמיכה גם באות גדולה וגם בקטנה
          };

          this.donors.push(donorDisplay);
          this.closeForm();
        },
        error: (err) => alert("שגיאה בהוספה: " + err.message)
      });
    }
  }

  // --- מחיקה ---
  removeDonor(id: number) {
    if (confirm("האם למחוק תורם זה?")) {
      this.giftService.deleteDonor(id).subscribe({
        next: () => {
          // הסרה מהרשימה המקומית
          this.donors = this.donors.filter(d => d.id !== id);
        },
        error: (err) => {
          // הצגת השגיאה מהשרת (למשל: לא ניתן למחוק כי יש מתנות)
          const msg = err.error?.Message || err.message || "שגיאה במחיקה";
          alert(msg);
        }
      });
    }
  }

  // --- סגירה ואיפוס ---
  closeForm() {
    this.showForm = false;
    this.selectedDonor = null;
    this.tempDonor = {};
  }
}