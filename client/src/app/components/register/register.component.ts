import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  authSrv: AuthService = inject(AuthService);
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
    this.authSrv.register(this.profileForm.value).subscribe();
  }
}
