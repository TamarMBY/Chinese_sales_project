import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { tap } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  BASE_URL = 'https://localhost:7280/api/Auth';
  constructor(private http: HttpClient) { }
  register(body: any) {
    alert("here")
    return this.http.post(`${this.BASE_URL}/register`, body);
  }

  login(body: any) {
    return this.http.post<any>(`${this.BASE_URL}/login`, body).pipe(
      tap(res => {
        if (res && res.token) {
          localStorage.setItem('token', res.token);
          localStorage.setItem('user', JSON.stringify(res.user));
        }
      })
    )
  }
  logout() {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }
  get token() {
    return localStorage.getItem('token')
  }
  get user() {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }
  isLoggedIn() {
    return !!this.token;
  }
  isAdmin():boolean{
    const user = this.user;
    console.log("current user"+ user.role);
    
    console.log(user && user.role ==='Admin');
     return user && user.role ==='Admin'

  }
}

