import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GiftsService } from '../services/gifts';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.html',
  styleUrls: ['./reports.scss']
})
export class ReportsComponent implements OnInit {
  
  winners: any[] = [];
  incomeDetails: any[] = [];
  grandTotal: number = 0;

  constructor(private gifts: GiftsService) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    // טעינת זוכים
    this.gifts.getWinnersReport().subscribe(data => {
      this.winners = data;
    });

    // טעינת הכנסות
    this.gifts.getIncomeReport().subscribe(data => {
      this.incomeDetails = data.details; // שים לב לאותיות קטנות/גדולות לפי השרת
      this.grandTotal = data.grandTotal;
    });
  }
}