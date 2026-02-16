import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GiftsService } from '../../services/gifts';

@Component({
  selector: 'app-gift-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './gift-form.html'
})
export class GiftForm implements OnInit {
  @Input() gift: any;
  @Output() saveGift = new EventEmitter<any>();
  giftForm!: FormGroup;
  donors: any[] = [];
  categories: string[] = ['מוצרי חשמל', 'כלי בית', 'ריהוט', 'ספרים', 'נופש'];

  constructor(private fb: FormBuilder, private giftsService: GiftsService) {}

  ngOnInit() {
    // טעינת רשימת התורמים מהשרת
    this.giftsService.getDonors().subscribe(data => this.donors = data);

    // אתחול הטופס עם ולידציות
    this.giftForm = this.fb.group({
      id: [this.gift?.id || 0],
      name: [this.gift?.name || '', Validators.required],
      description: [this.gift?.description || '', Validators.required],
      category: [this.gift?.category || '', Validators.required],
      cost: [this.gift?.cost || 10, [Validators.required, Validators.min(1)]],
      donorId: [this.gift?.donorId || null, Validators.required],
      imageUrl: [this.gift?.imageUrl || '']
    });
  }

  // הפונקציה לטיפול בבחירת תמונה ועדכון הטופס
  onImageSelected(event: any) {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = () => {
      // עדכון שדה ה-imageUrl בתוך ה-FormGroup
      this.giftForm.patchValue({
        imageUrl: reader.result as string
      });
    };
    reader.readAsDataURL(file);
  }

  onSave() {
    if (this.giftForm.valid) {
      // שליחת הערכים המעודכנים של הטופס לקומפוננטת האב
      this.saveGift.emit(this.giftForm.value);
    }
  }
}