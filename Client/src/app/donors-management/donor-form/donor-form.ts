import { Component, EventEmitter, Output, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-donor-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './donor-form.html'
})
export class DonorForm implements OnInit {
  @Input() donor: any;
  @Output() saveDonor = new EventEmitter<any>();
  donorForm!: FormGroup;

  constructor(private fb: FormBuilder) {}

  ngOnInit() {
    this.donorForm = this.fb.group({
      id: [this.donor?.id || 0],
      name: [this.donor?.name || '', [Validators.required, Validators.minLength(2)]],
      email: [this.donor?.email || '', [Validators.required, Validators.email]],
      phone: [this.donor?.phone || '', [Validators.required, Validators.pattern('^[0-9]{9,10}$')]]
    });
  }

  onSave() {
    if (this.donorForm.valid) {
      this.saveDonor.emit(this.donorForm.value);
    }
  }
}