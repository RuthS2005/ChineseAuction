import { CommonModule } from '@angular/common';
import { Component, Input, Output,EventEmitter } from '@angular/core';

@Component({
  selector: 'app-donors-list',
  standalone:true,
  imports: [CommonModule],
  templateUrl: './donors-list.html',
  styleUrls: ['./donors-list.scss'],
})



export class DonorsList {
@Input()
donors:any[]=[];

@Output() deleteDonor = new EventEmitter<number>();
@Output() editDonor = new EventEmitter<any>();

onEdit(donor:any){
  this.editDonor.emit(donor)
}
onDelete(i:number){
  this.deleteDonor.emit(i)
}
}
