import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GiftsService } from '../../services/gifts'
@Component({
  selector: 'app-gift-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './gift-form.html',
  styleUrls: ['./gift-form.scss'],
})
export class GiftForm implements OnInit {
  
  // הרחבנו את אובייקט ברירת המחדל כדי שלא יהיו שגיאות ב-HTML
  @Input() gift: any = { 
    name: '', 
    description: '', 
    value: 0, 
    category: '', 
    donorId: null, 
    imageUrl: '' 
  };
  
  @Output() saveGift = new EventEmitter<any>();

  // 2. הנה המערכים שהיו חסרים לך ב-HTML
  donors: any[] = []; 
  categories: string[] = ['מוצרי חשמל', 'כלי בית', 'ריהוט', 'ספרים', 'נופש'];

  // 3. מזריקים את הסרוויס
  constructor(private giftsService: GiftsService) {}

  // 4. טוענים את התורמים מיד כשהטופס עולה
  ngOnInit() {
    this.giftsService.getDonors().subscribe(data => {
      this.donors = data;
    });
  }

  onImageSelected(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = () => {
      // עדכנתי ל-imageUrl כדי שיתאים לשרת
      this.gift.imageUrl = reader.result as string;
    };
    reader.readAsDataURL(file);
  }

  onSave() {
    // 5. סידור קטן לפני השליחה כדי שיתאים לשרת (Cost במקום Value)
    const giftDto = {
      name: this.gift.name,
      description: this.gift.description,
      category: this.gift.category,
      cost: this.gift.value, // המרה מ-value ל-cost
      imageUrl: this.gift.imageUrl || '',
      donorId: this.gift.donorId
    };

    this.saveGift.emit(giftDto);
  }
}