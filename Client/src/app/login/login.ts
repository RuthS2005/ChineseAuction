import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Auth } from '../services/auth'; // וודא נתיב
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrls: ['./login.scss'] // אם יצרת קובץ css/scss
})
export class LoginComponent {
  
  isLoginMode = true; // האם אנחנו במצב כניסה או הרשמה?

  // אובייקט שיחזיק את כל הנתונים מהטופס
  userData = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    phone: ''
  };

  constructor(private auth: Auth, private router: Router) {}

  // פונקציה להחלפה בין כניסה להרשמה
  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
  }

  onSubmit() {
    if (this.isLoginMode) {
      // --- לוגיקה של כניסה ---
      this.auth.login(this.userData).subscribe({
        next: (res) => {
          this.router.navigate(['/home']); // מעבר לדף הבית
        },
        error: (err) => alert('שגיאה בכניסה: ' + (err.error.message || err.message))
      });
    } else {
      // --- לוגיקה של הרשמה ---
      this.auth.register(this.userData).subscribe({
        next: () => {
          this.isLoginMode = true; // מעביר למצב כניסה
        },
        error: (err) => alert('שגיאה בהרשמה: ' + (err.error.message || err.message))
      });
    }
  }
}