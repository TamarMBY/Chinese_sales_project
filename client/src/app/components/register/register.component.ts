import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  authSrv: AuthService = inject(AuthService);
  router = inject(Router);
  profileForm = new FormGroup({
    Id: new FormControl(''),
    FullName: new FormControl(''),
    UserName: new FormControl(''),
    Password: new FormControl(''),
    Email: new FormControl(''),
    PhoneNumber: new FormControl('')
  });
  register() {
    console.log(this.profileForm.value);
    this.authSrv.register(this.profileForm.value).subscribe(() => {
      console.log("aaaa");
      this.login();
      this.router.navigate(['/package']);
    });
  }
    login() {
      this.authSrv.login(this.profileForm.value).subscribe({
        next: (user: any) => {
          if(user){
            this.router.navigate(['/']);
          }
        },
      });
  }
}
