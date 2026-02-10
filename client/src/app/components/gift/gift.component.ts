import { GiftService } from '../../services/gift.service';
import { GiftModel } from '../../models/gift.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Component, inject, Input, OnChanges, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { TicketModel } from '../../models/ticket.model';
import { BusketService } from '../../services/busket.service';
import { BusketModel } from '../../models/busket.model';


@Component({
  selector: 'app-gift',
  imports: [CommonModule, FormsModule],
  templateUrl: './gift.component.html',
  styleUrl: './gift.component.css'
})
export class GiftComponent implements OnChanges {
  router = inject(Router)
  giftSrv: GiftService = inject(GiftService);
  busketSrv = inject(BusketService);
  list$ = this.giftSrv.getAll();
   busket: BusketModel = {};
    user: any = {};
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
  draftGift: GiftModel = {
    id: 0,
    name: '',
    description: '',
    price: 0,
    imageUrl: '',

    categoryId: undefined,
    donorId: '',
    winnerId: '',
    isDrawn: false
  };
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
      winnerId: g.winnerId ?? null!,
      isDrawn: g.isDrawn ?? false
    };
  }
  save() {    
    if (!this.draftGift.name) return;
    const id = this.draftGift.id;
    if (this.isEditMode) {
      this.giftSrv.update(id!, this.draftGift).subscribe(() => {
        this.refreshList();
        this.resetForm();
      });
    } else {
      this.giftSrv.add(this.draftGift).subscribe(() => {
        this.refreshList();
        this.resetForm();
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
   addGift(giftId : number){
    
    const ticket = {
      giftId: giftId,
      purchaseId: this.busket.id,
      quantity:1
      
    }

    return this.busketSrv.addTicket(ticket).subscribe((updateBusket:BusketModel)=>{
      console.log(updateBusket);
      this.busket = updateBusket;
    });
  }
  deleteGift(giftId:number){
    return this.busketSrv.deleteTicket(this.busket.id!,giftId).subscribe((updateBusket:BusketModel)=>{
      this.busket = updateBusket;
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
  refreshList() {
    this.list$ = this.giftSrv.getAll();
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
