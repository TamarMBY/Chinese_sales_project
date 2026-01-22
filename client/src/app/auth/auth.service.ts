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
        }
      })
    )
  }
  logout() {
    localStorage.removeItem('token')
  }
  get token() {
    return localStorage.getItem('token')
  }
  isLoggedIn() {
    return !!this.token;
  }
}
