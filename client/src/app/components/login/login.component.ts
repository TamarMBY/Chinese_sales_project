import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  authSrv: AuthService = inject(AuthService);
  profileForm = new FormGroup({
    UserName: new FormControl(''),
    Password: new FormControl(''),
  });
  login() {
    this.authSrv.login(this.profileForm.value).subscribe();
  }
  logout() {
    this.authSrv.logout();
  }
}
