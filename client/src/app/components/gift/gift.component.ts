import { Component, inject, Input, OnChanges, SimpleChanges, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

// NG-ZORRO Imports
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzModalModule } from 'ng-zorro-antd/modal'; // חובה למודאל

// Services & Models
import { GiftService } from '../../services/gift.service';
import { GiftModel } from '../../models/gift.model';
import { BusketService } from '../../services/busket.service';
import { BusketModel } from '../../models/busket.model';
import { DonorService } from '../../services/donor.service';
import { AuthService } from '../../auth/auth.service';
import { TicketModel } from '../../models/ticket.model';

@Component({
  selector: 'app-gift',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    NzCardModule, 
    NzButtonModule, 
    NzInputModule, 
    NzIconModule, 
    NzGridModule, 
    NzFormModule, 
    NzAlertModule, 
    NzDividerModule, 
    NzInputNumberModule,
    NzToolTipModule,
    NzModalModule
  ],
  templateUrl: './gift.component.html',
  styleUrl: './gift.component.css'
})
export class GiftComponent implements OnInit, OnChanges {
  // Dependency Injection
  private router = inject(Router);
  public giftSrv = inject(GiftService);
  private donorSrv = inject(DonorService);
  private busketSrv = inject(BusketService);
  public authSrv = inject(AuthService);

  // Data Observables & State
  list$ = this.giftSrv.getAll();
  busket: BusketModel = { purchasePackages: [], tickets: [] };
  errorMessage: string | null = null;
  
  // Modal State
  isModalVisible = false;
  isEditMode = false;

  @Input() categoryId: number = 0;

  // Filter Values
  filterValues = {
    name: '',
    categoryId: undefined as number | undefined,
    donorId: '',
    buyerCount: undefined as number | undefined
  };

  // Draft for Add/Edit
  draftGift: GiftModel = this.getEmptyGift();

  ngOnInit() {
    const userData = localStorage.getItem('user');
    if (userData) {
      const user = JSON.parse(userData);
      this.loadBasket(user.id);
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['categoryId']) {
      if (this.categoryId > 0) {
        this.filter(undefined, this.categoryId);
      } else {
        this.refreshList();
      }
    }
  }

  // --- Modal Logic ---

  openAddModal() {
    this.isEditMode = false;
    this.errorMessage = null;
    this.draftGift = this.getEmptyGift();
    this.isModalVisible = true;
  }

  openEdit(g: GiftModel) {
    this.isEditMode = true;
    this.errorMessage = null;
    // יצירת עותק כדי לא לשנות את המקור בזמן ההקלדה
    this.draftGift = { ...g };
    this.isModalVisible = true;
  }

  handleCancel() {
    this.isModalVisible = false;
    this.resetForm();
  }

  // --- Core Actions ---

  save() {
    if (!this.draftGift.name || !this.draftGift.donorId) {
      this.errorMessage = "נא למלא את כל שדות החובה (שם ותורם)";
      return;
    }

    const action$ = this.isEditMode 
      ? this.giftSrv.update(this.draftGift.id!, this.draftGift)
      : this.giftSrv.add(this.draftGift);

    action$.pipe(
      catchError(err => {
        this.errorMessage = this.isEditMode ? "עדכון נכשל" : "הוספה נכשלה";
        return of(null);
      })
    ).subscribe(res => {
      if (res) {
        this.isModalVisible = false;
        this.refreshList();
        this.resetForm();
      }
    });
  }

  delete(id: number) {
    this.giftSrv.delete(id).subscribe(() => this.refreshList());
  }

  filter(name?: string, categoryId?: number, donorId?: string, buyerCount?: number) {
    const n = name !== undefined ? name : this.filterValues.name;
    const c = categoryId !== undefined ? categoryId : this.filterValues.categoryId;
    const d = donorId !== undefined ? donorId : this.filterValues.donorId;
    const b = buyerCount !== undefined ? buyerCount : this.filterValues.buyerCount;
    
    this.list$ = this.giftSrv.filter(n || undefined, c, d || undefined, b);
  }

  // --- Basket & Lottery ---

  loadBasket(userId: string) {
    this.busketSrv.getByUserId(userId).subscribe(b => this.busket = b);
  }

  addTicket(giftId: number) {
    this.busketSrv.addTicket({ giftId, busketId: this.busket.id, quantity: 1 })
      .subscribe(res => this.busket = res);
  }

  deleteTicket(giftId: number) {
    this.busketSrv.deleteTicket(this.busket.id!, giftId)
      .subscribe(res => this.busket = res);
  }

  lottery(giftId: number) {
    this.giftSrv.lottery(giftId).pipe(
      catchError(() => {
        this.errorMessage = "ההגרלה נכשלה - ייתכן ואין רוכשים.";
        return of(null);
      })
    ).subscribe(() => this.refreshList());
  }

  lotteryAllGifts() {
    this.giftSrv.getAll().subscribe(gifts => {
      const pending = gifts.filter(g => !g.isDrawn).map(g => 
        this.giftSrv.lottery(g.id!).pipe(catchError(() => of(null)))
      );
      if (pending.length > 0) {
        forkJoin(pending).subscribe(() => this.refreshList());
      }
    });
  }

  // --- Helpers ---

  getById(id: number) {
    this.router.navigate([`gift/${id}`]);
  }

  refreshList() {
    this.list$ = this.giftSrv.getAll();
    this.filterValues = { name: '', categoryId: undefined, donorId: '', buyerCount: undefined };
  }

  resetForm() {
    this.isEditMode = false;
    this.draftGift = this.getEmptyGift();
  }

  private getEmptyGift(): GiftModel {
    return {
      id: 0, name: '', description: '', price: 0, imageUrl: '',
      categoryId: undefined, donorId: '', isDrawn: false
    };
  }
}