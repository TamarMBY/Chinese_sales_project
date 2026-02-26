import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// import { CardModule } from 'primeng/card';
// import { InputTextModule } from 'primeng/inputtext';
// import { ButtonModule } from 'primeng/button';
// import { MessageModule } from 'primeng/message';

import { BusketService } from '../../services/busket.service';
import { ActivatedRoute } from '@angular/router';
import { BusketModel } from '../../models/busket.model';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent {

  busketSrv: BusketService = inject(BusketService);
  route = inject(ActivatedRoute);

  busketId = this.route.snapshot.paramMap.get('busketId');

  success = false;
  submitted = false;

  payment = {
    cardHolder: '',
    cardNumber: '',
    expiry: '',
    cvv: ''
  };

  busket: BusketModel = {};

  // ---- validation helpers ----
  get isCardHolderValid() {
    return this.payment.cardHolder.trim().length >= 2;
  }

  get isCardNumberValid() {
    const digits = this.payment.cardNumber.replace(/\s/g, '');
    return /^\d{16}$/.test(digits);
  }
  get isExpiryValid() {
  // בדיקת פורמט בסיסית
  if (!/^(0[1-9]|1[0-2])\/\d{2}$/.test(this.payment.expiry)) {
    return false;
  }

  const [mm, yy] = this.payment.expiry.split('/');
  const expMonth = Number(mm);
  const expYear = Number('20' + yy);

  const today = new Date();
  const currentMonth = today.getMonth() + 1; // 1–12
  const currentYear = today.getFullYear();

  // אם השנה קטנה מהנוכחית – פסול
  if (expYear < currentYear) {
    return false;
  }

  // אם אותה שנה אבל חודש קטן מהנוכחי – פסול
  if (expYear === currentYear && expMonth < currentMonth) {
    return false;
  }

  return true;
}


  get isCvvValid() {
    return /^\d{3}$/.test(this.payment.cvv);
  }

  get isFormValid() {
    return (
      this.isCardHolderValid &&
      this.isCardNumberValid &&
      this.isExpiryValid &&
      this.isCvvValid
    );
  }

  pay() {
    this.submitted = true;

    if (!this.isFormValid) {
      return;
    }

    // דמו בלבד – אין שליחה אמיתית
    console.log('FAKE PAYMENT:', this.payment);
    this.success = true;

    this.busketSrv.getById(Number(this.busketId)).subscribe(b => {
      this.busket = b;
      this.busketSrv.completePurchase(this.busket).subscribe();
    });
  }
}
