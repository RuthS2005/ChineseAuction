import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class Auth {
   private apiUrl = 'http://localhost:5035/api/Users'; 
      constructor(private http: HttpClient) { }
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
  getCurrentUser(): any {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }
  isLoggedIn(): boolean {
    return this.getCurrentUser() !== null;
  } 
  // בדיקה האם המשתמש המחובר הוא מנהל
isManager(): boolean {
  const userStr = localStorage.getItem('user');
  if (userStr) {
    const user = JSON.parse(userStr);
    console.log("User Data from Storage:", user);

    // 2. נרמול: קח את הערך בין אם הוא כתוב ב-role או ב-Role
    const actualRole = user.role !== undefined ? user.role : user.Role;

    console.log("Actual Role Value:", actualRole);

    // 3. בדיקה רחבה:
    // האם זה המספר 1? (משתמשים ב-== כדי לתפוס גם את "1" כמחרוזת)
    // האם זו המילה 'Manager'?
    return actualRole == 1 || actualRole === 'Manager';
  }
  
  return false;
}
}
