import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';

// ייבוא רכיבי PrimeNG
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MessageService } from 'primeng/api';

import { AuthService } from '../../auth/auth.service';
import { DividerModule } from 'primeng/divider';
import { Route, Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true, // וודא שזה מוגדר כ-Standalone
  imports: [
    ReactiveFormsModule, 
    DividerModule,
    MessageModule, 
    ToastModule, 
    ButtonModule, 
    InputTextModule,
    FormsModule, 
    RouterLink
  ],
  providers: [MessageService], // חשוב מאוד כדי שה-Toast יעבוד
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  constructor(private router: Router) {}
  private authSrv = inject(AuthService);
  showUserNotFound = signal(false);
  private messageService = inject(MessageService);

  profileForm = new FormGroup({
    Username: new FormControl('', [Validators.required]),
    Password: new FormControl('', [Validators.required]),
  });

  login() {
      this.authSrv.login(this.profileForm.value).subscribe({
        next: (user: any) => {
          if(user){
            this.router.navigate(['/']);
          }
          else{
            this.showUserNotFound.set(true);
          }
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'פרטי התחברות שגויים' });
          this.showUserNotFound.set(true);
        }
      });
    
  }

  logout() {
    this.authSrv.logout();
  }

}