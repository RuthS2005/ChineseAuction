import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Auth } from '../services/auth';  
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule], // עברנו ל-ReactiveFormsModule
  templateUrl: './login.html'
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  isLoginMode = true;

  constructor(private fb: FormBuilder, private auth: Auth, private router: Router) {}

  ngOnInit() {
    this.initForm();
  }

  initForm() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      // שדות נוספים להרשמה בלבד
      username: [''], 
      phone: ['', [Validators.pattern('^[0-9]{9,10}$')]]
    });
  }

  toggleMode() {
    this.isLoginMode = !this.isLoginMode;
    // איפוס ולידציות לשדות הרשמה אם חזרנו לכניסה
    if (this.isLoginMode) {
      this.loginForm.get('username')?.clearValidators();
    } else {
      this.loginForm.get('username')?.setValidators([Validators.required]);
    }
    this.loginForm.get('username')?.updateValueAndValidity();
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    const data = this.loginForm.value;
    const request = this.isLoginMode ? this.auth.login(data) : this.auth.register(data);

    request.subscribe({
      next: (res) => {
        if (this.isLoginMode) this.router.navigate(['/home']);
        else this.isLoginMode = true;
      },
      error: (err) => alert('שגיאה: ' + (err.error.message || 'פעולה נכשלה'))
    });
  }
}