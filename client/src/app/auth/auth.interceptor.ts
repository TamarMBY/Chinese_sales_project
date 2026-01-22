import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  // שליפת הטוקן מה-LocalStorage
  const token = localStorage.getItem('token');

  // אם קיים טוקן, נשכפל את הבקשה ונוסיף לה את ה-Header
  if (token) {
    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
    return next(cloned);
  }

  // אם אין טוקן, נמשיך בבקשה המקורית
  return next(req);
};