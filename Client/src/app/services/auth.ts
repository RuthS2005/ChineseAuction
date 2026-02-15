import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';  

@Injectable({
  providedIn: 'root',
})
export class Auth {
   private apiUrl = 'http://localhost:5035/api/Users'; 
      constructor(private http: HttpClient, private router: Router) { }
      login(loginData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, loginData).pipe(
      tap((response: any) => {
        // אם הצלחנו להתחבר - נשמור את הפרטים בזיכרון הדפדפן
        if (response && response.token) {
          localStorage.setItem('user', JSON.stringify(response));
        }
      })
    );
  }
  register(user: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, user);
  }
  logout(): void {
    localStorage.removeItem('user');
  }
getCurrentUser(): number {
  const userStr = localStorage.getItem('user');
  
  if (userStr) {
    const user = JSON.parse(userStr);
    

    const id = user.id || user.Id || 0; 
    
    return Number(id); // מוודאים שזה מספר
  }
  
  return 0; // לא מחובר
}
  isLoggedIn(): boolean {
    return this.getCurrentUser() !== null;
  } 
  // בדיקה האם המשתמש המחובר הוא מנהל
isManager(): boolean {
  const userStr = localStorage.getItem('user');
  if (userStr) {
    const user = JSON.parse(userStr);
    console.log("בדיקת מנהל:", user.role); // לראות בעיניים
    
    return user.role === 'Manager' || user.role === 1 || user.Role === 'Manager';
  }
  return false;
}
}
