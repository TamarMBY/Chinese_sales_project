import { GiftService } from '../../services/gift.service';
import { GiftModel } from '../../models/gift.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Component, inject, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { TicketModel } from '../../models/ticket.model';
import { BusketService } from '../../services/busket.service';
import { BusketModel } from '../../models/busket.model';
import { DonorService } from '../../services/donor.service';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { MessageModule } from 'primeng/message'; // מומלץ להוסיף ב-imports
import { forkJoin } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { AuthService } from '../../auth/auth.service';


@Component({
  selector: 'app-gift',
  standalone: true,
  imports: [CommonModule, FormsModule, CardModule, ButtonModule, InputTextModule, MessageModule],
  templateUrl: './gift.component.html',
  styleUrl: './gift.component.css'
})
export class GiftComponent implements OnChanges {
  router = inject(Router)
  giftSrv: GiftService = inject(GiftService);
  donorSrv: DonorService = inject(DonorService)
  busketSrv = inject(BusketService);
  // donorSrv: DonorService = inject(DonorService);
  authSrv = inject(AuthService);
  list$ = this.giftSrv.getAll();
  
  busket: BusketModel = {
    purchasePackages: [],
    tickets: []
  };
  user: any = {};
  
  // משתנה חדש לניהול שגיאות
  errorMessage: string | null = null;

  draftGift: GiftModel = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    imageUrl: '',
    categoryId: undefined,
    donorId: '',
    donor: undefined,
    winnerId: '',
    isDrawn: false
  };

  ngOnInit() {
    this.user = localStorage.getItem('user');
    if (this.user) {
      console.log(this.user.id);
      this.user = JSON.parse(this.user);
      this.getByUserId(this.user.id);
    }
  }

  getByUserId(userId: string) {
    this.busketSrv.getByUserId(userId).pipe(
      catchError(err => {
        console.error("Error fetching basket", err);
        return of({ purchasePackages: [], tickets: [] } as BusketModel);
      })
    ).subscribe(b => {
      this.busket = b;
      console.log(this.busket);
    });
  }

  isEditMode = false;
  @Input() categoryId: number = 0;

  openEdit(g: GiftModel) {
    this.isEditMode = true;
    this.errorMessage = null; // איפוס שגיאות במעבר לעריכה
    this.draftGift = {
      id: g.id ?? 0,
      name: g.name ?? '',
      description: g.description ?? '',
      price: g.price ?? 0,
      imageUrl: g.imageUrl ?? '',
      categoryId: g.categoryId ?? 0,
      donorId: g.donorId ?? '',
      donor: g.donor ?? undefined,
      winnerId: g.winnerId ?? null!,
      isDrawn: g.isDrawn ?? false
    };
  }

  save() {
    // וולידציה בסיסית לפני שליחה
    if (!this.draftGift.name || !this.draftGift.donorId) {
      this.errorMessage = "נא למלא את כל שדות החובה (שם ותורם)";
      return;
    }

    this.errorMessage = null;
    const id = this.draftGift.id;

    if (this.isEditMode) {
      const update = {
        name: this.draftGift.name,
        description: this.draftGift.description,
        imageUrl: this.draftGift.imageUrl,
        categoryId: this.draftGift.categoryId,
        donorId: this.draftGift.donorId,
        winnerId: this.draftGift.winnerId,
        isDrawn: this.draftGift.isDrawn
      }
      this.giftSrv.update(id!, update).pipe(
        catchError(err => {
          this.errorMessage = "עדכון המתנה נכשל. נסה שוב.";
          return of(null);
        })
      ).subscribe((res) => {
        if(res) {
          this.refreshList();
          this.resetForm();
        }
      });
    } else {
      this.giftSrv.add(this.draftGift).pipe(
        catchError(err => {
          this.errorMessage = "הוספת המתנה נכשלה.";
          return of(null);
        })
      ).subscribe((res) => {
        if(res) {
          this.donorSrv.getById(this.draftGift.donorId!).subscribe(d => {
            this.draftGift.donor = d;
            this.refreshList();
            this.resetForm();
          });
        }
      });
    }
  }

  getById(id: number) {
    this.router.navigate([`gift/${id}`]);
  }

  delete(id: number) {
    // if(!confirm("האם למחוק את המתנה?")) return;

    this.giftSrv.delete(id).pipe(
      catchError(err => {
        this.errorMessage = "לא ניתן למחוק את המתנה.";
        return of(null);
      })
    ).subscribe(g => {
      if(g) this.refreshList();
    })
  }

  ticket: TicketModel = {};
  addTicket(giftId: number) {
    const ticket = {
      giftId: giftId,
      purchaseId: this.busket.id,
      quantity: 1
    }
    return this.busketSrv.addTicket(ticket).pipe(
      catchError(err => {
        this.errorMessage = "הוספת כרטיס נכשלה.";
        return of(null);
      })
    ).subscribe((updateBusket: any) => {
      console.log(updateBusket);
      this.busket = updateBusket ?? { purchasePackages: [], tickets: [] };
    });
  }

  deleteTicket(giftId: number) {
    console.log(this.busket);
    return this.busketSrv.deleteTicket(this.busket.id!, giftId).pipe(
      catchError(err => {
        this.errorMessage = "מחיקת כרטיס נכשלה.";
        return of(null);
      })
    ).subscribe((b: any) => {
      this.busket = b;
    });
  }

  filter(name?: string, categoryId?: number, donorId?: string, buyerCount?: number) {
    this.list$ = this.giftSrv.filter(name, categoryId, donorId, buyerCount);
  }

  lottery(giftId: number) {
    this.giftSrv.lottery(giftId).pipe(
      catchError(err => {
        this.errorMessage = "ההגרלה נכשלה - לא נמצאו קונים עבור המתנה .";
        return of(null);
      })
    ).subscribe(() => {
      this.refreshList();
    });
  }

  lotteryAllGifts() {
    this.giftSrv.getAll().subscribe(gifts => {
      const lotteryRequests = gifts
        .filter(g => !g.isDrawn)
        .map(g =>
          this.giftSrv.lottery(g.id!).pipe(
            catchError(error => {
              console.error(`ההגרלה נכשלה עבור מתנה ${g.id}`, error);
              return of(null); // מחזיר Observable "ריק" כדי שהלולאה תמשיך
            })
          )
        );

      if (lotteryRequests.length > 0) {
        forkJoin(lotteryRequests).subscribe({
          next: (results) => {
            console.log('תהליך ההגרלה הסתיים');
            this.refreshList();
          }
        });
      }
    });
  }

  refreshList() {
    this.list$ = this.giftSrv.getAll();
    console.log(this.draftGift.name);
  }

  resetForm() {
    this.isEditMode = false;
    this.errorMessage = null;
    this.draftGift = { id: 0, name: '', description: '', price: 0, imageUrl: '', categoryId: 0, donorId: '', winnerId: '', isDrawn: false };
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categoryId']) {
      if (this.categoryId && this.categoryId > 0) {
        this.filter(undefined, this.categoryId, undefined, undefined);
      } else {
        this.refreshList();
      }
    }
  }

  getWinner(id: string) {
    return this.authSrv.getById(id);
  }
}