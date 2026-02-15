import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-donor-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './donor-form.html',
  styleUrls: ['./donor-form.scss'],
})

export class DonorForm {
  @Input() donor: any = { name: '', phone: '' };

  @Output() saveDonor = new EventEmitter<any>();

  onSave() {
    console.log("הילד שולח:", this.donor);

    this.saveDonor.emit(this.donor);
  }
}
