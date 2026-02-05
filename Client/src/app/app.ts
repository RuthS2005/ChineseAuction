import { Component, signal } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common'; // חובה בשביל *ngIf
import { Auth } from './services/auth'; // הייבוא של הסרוויס שלך

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,RouterLink,CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('MeciraSinit');
  constructor(public auth: Auth) {}

  logout() {
    this.auth.logout();
    window.location.reload();
  }
}
