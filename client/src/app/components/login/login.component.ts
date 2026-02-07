import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

// ייבוא רכיבי PrimeNG
import { MessageModule } from 'primeng/message';
import { ToastModule } from 'primeng/toast';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MessageService } from 'primeng/api';

import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true, // וודא שזה מוגדר כ-Standalone
  imports: [
    ReactiveFormsModule, 
    MessageModule, 
    ToastModule, 
    ButtonModule, 
    InputTextModule
  ],
  providers: [MessageService], // חשוב מאוד כדי שה-Toast יעבוד
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private authSrv = inject(AuthService);
  private messageService = inject(MessageService);

  profileForm = new FormGroup({
    UserName: new FormControl('', [Validators.required]),
    Password: new FormControl('', [Validators.required]),
  });

  login() {
    if (this.profileForm.valid) {
      this.authSrv.login(this.profileForm.value).subscribe({
        next: (res) => {
          this.messageService.add({ severity: 'success', summary: 'התחברות', detail: 'ברוך הבא!' });
        },
        error: (err) => {
          this.messageService.add({ severity: 'error', summary: 'שגיאה', detail: 'פרטי התחברות שגויים' });
        }
      });
    }
  }

  logout() {
    this.authSrv.logout();
  }
}