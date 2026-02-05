import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Auth } from '../../services/auth'; // הזרקת הסרוויס

@Component({
  selector: 'app-gifts-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './gift-list.html',
  styleUrls: ['./gift-list.scss'],
})
export class GiftsList {
  @Input() gifts: any[] = [];
  @Output() deleteGift = new EventEmitter<number>();
  @Output() editGift = new EventEmitter<any>();
  @Output() raffle = new EventEmitter<number>();

  // חובה להזריק את ה-Auth כדי להשתמש בו ב-HTML
  constructor(public auth: Auth) {} 

  onEdit(gift: any) { this.editGift.emit(gift); }
  onDelete(i: number) { this.deleteGift.emit(i); }
  onRaffle(giftId: number) { this.raffle.emit(giftId); }
}