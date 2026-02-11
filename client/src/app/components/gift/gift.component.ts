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
import { forkJoin } from 'rxjs'; // וודא שהוספת את ה-import הזה למעלה
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';
@Component({
  selector: 'app-gift',
  imports: [CommonModule, FormsModule, CardModule, ButtonModule, InputTextModule],
  templateUrl: './gift.component.html',
  styleUrl: './gift.component.css'
})
export class GiftComponent implements OnChanges {
  router = inject(Router)
  giftSrv: GiftService = inject(GiftService);
  busketSrv = inject(BusketService);
  donorSrv = inject(DonorService);
  list$ = this.giftSrv.getAll();
  busket: BusketModel = {};
  user: any = {};
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
    this.busketSrv.getByUserId(userId).subscribe(b => {
      this.busket = b;
      console.log(this.busket);
    });
  }
  isEditMode = false;
  @Input() categoryId: number = 0;
  openEdit(g: GiftModel) {
    this.isEditMode = true;
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
    if (!this.draftGift.name) return;
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
      this.giftSrv.update(id!, update).subscribe(() => {
        this.refreshList();
        this.resetForm();
      });
    } else {
      this.giftSrv.add(this.draftGift).subscribe(() => {
        this.donorSrv.getById(this.draftGift.donorId!).subscribe(d => {
          this.draftGift.donor = d;
          this.refreshList();
          this.resetForm();
        });
      });
    }
  }
  getById(id: number) {
    this.router.navigate([`gift/${id}`]);
  }

  delete(id: number) {
    this.giftSrv.delete(id).subscribe(g => {
      this.refreshList();
    })
  }
  ticket: TicketModel = {};
  addTicket(giftId: number) {
    const ticket = {
      giftId: giftId,
      purchaseId: this.busket.id,
      quantity: 1
    }
    return this.busketSrv.addTicket(ticket).subscribe((updateBusket: BusketModel) => {
      console.log(updateBusket);
      this.busket = updateBusket ?? { purchasePackages: [], tickets: [] };
    });
  }
  deleteTicket(giftId: number) {
    return this.busketSrv.deleteTicket(this.busket.id!, giftId).subscribe((updateBusket: BusketModel) => {
      this.busket = updateBusket ?? { purchasePackages: [], tickets: [] };
    });
  }
  filter(name?: string, categoryId?: number, donorId?: string, buyerCount?: number) {
    this.list$ = this.giftSrv.filter(name, categoryId, donorId, buyerCount);
  }
  lottery(giftId: number) {
    this.giftSrv.lottery(giftId).subscribe(() => {
      this.refreshList();
    });
  }
  lotteryAllGifts() {
  this.giftSrv.getAll().subscribe(gifts => {
    const lotteryRequests = gifts
      .filter(g => !g.isDrawn)
      .map(g => 
        this.giftSrv.lottery(g.id!).pipe(
          // טיפול בשגיאה ספציפית לכל מתנה כדי שלא תפיל את כל הלולאה
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
}
