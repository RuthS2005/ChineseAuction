import { HttpInterceptorFn } from '@angular/common/http';

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  
  //   לשלוף את המשתמש מהזיכרון
  const userString = localStorage.getItem('user');
  
  if (userString) {
    const user = JSON.parse(userString);
    const token = user.token || user.Token; // גמישות לאותיות גדולות/קטנות

    // 2. אם יש טוקן - שכפל את הבקשה ונוסיף לה את הכותרת
    if (token) {
      const clonedRequest = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
      // משחררים את הבקשה המשוכפלת לדרך
      return next(clonedRequest);
    }
  }

  // 3. אם אין טוקן, משחררים את הבקשה המקורית כמו שהיא
  return next(req);
};