import { Component, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common'; // חובה בשביל *ngIf
import { Auth } from './services/auth'; // הייבוא של הסרוויס שלך
import { Router } from '@angular/router'; // 1. הוספת הייבוא הזה

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,RouterLink,CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('MeciraSinit');
  constructor(public auth: Auth, private router: Router) {}

  logout() {
    this.auth.logout();
    this.router.navigate(['/']); // 3. העברה לדף הלוגין

  }
}
