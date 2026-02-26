import { Component, inject } from '@angular/core';
import { BusketService } from '../../services/busket.service';
import { BusketModel } from '../../models/busket.model';
import { TicketModel } from '../../models/ticket.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-busket',
  imports: [FormsModule, CommonModule],
  templateUrl: './busket.component.html',
  styleUrl: './busket.component.css'
})
export class BusketComponent {
  busketSrv: BusketService = inject(BusketService);
  router = inject(Router);
  // busket: BusketModel = {};
  busket: BusketModel = {
    purchasePackages: [],
    tickets: []
  };
  user: any = {};
  ngOnInit() {
    this.user = localStorage.getItem('user');
    if (this.user) {
      console.log(this.user);
      this.user = JSON.parse(this.user);
      this.getBusketByUserId(this.user.id);
      console.log("busket:"+this.groupTickets);
      
      this.groupTickets!.forEach(t => {
        console.log("aaa" + t.gift);
        if (t.gift?.isDrawn) {
          console.log(t.gift);
          this.deleteGift(t.id!);
        }
      });
      this.getBusketByUserId(this.user.id);
    }
    this.busketSrv.busketUpdated.subscribe(() => {
      console.log('התקבל עדכון בסרוויס, מרענן נתונים...');
      if (this.user && this.user.id) {
        this.getBusketByUserId(this.user.id);
      }

    })

  }
  getBusketByUserId(userId: string) {
    this.busketSrv.getByUserId(userId).subscribe(b => {
      this.busket = b ?? { purchasePackages: [], tickets: [] };
      console.log(this.busket);
    });
  }
  get groupTickets(): any[] {
    if (!this.busket || !this.busket.tickets) return [];

    const gruops = this.busket.tickets.reduce((acc: any, ticket: TicketModel) => {
      const id = ticket.giftId || 'unknown';
      if (!acc[id]) {
        acc[id] = { ...ticket, totalTickets: 0 }
      }
      acc[id].totalTickets++;
      return acc;
    }, {});
    return Object.values(gruops);
  }
  addPackageToCart(packageId: number) {
    if (!this.busket.id) return;
    this.busketSrv.addPackage(this.busket.id, packageId).subscribe((updatedBasket: BusketModel) => {
      this.busket = updatedBasket ?? { purchasePackages: [], tickets: [] };
    });
  }

  removePackageFromCart(packageId: number) {
    if (!this.busket.id) return;
    this.busketSrv.deletePackage(this.busket.id, packageId).subscribe((b: BusketModel) => {
      this.busket = b;
    });
  }
  addGift(giftId: number) {
    const ticket = {
      giftId: giftId,
      purchaseId: this.busket.id,
      quantity: 1

    }
    this.busketSrv.addTicket(ticket).subscribe({
      next: (updatedBusket) => {
        this.busket = updatedBusket;
      },
      error: (err) => {
        console.error("שגיאה בהוספת כרטיס:", err);
        alert("לא ניתן להוסיף את הכרטיס: " + err.error.message);
      }
    });
  }
  deleteGift(giftId: number) {
    this.busketSrv.deleteTicket(this.busket.id!, giftId).subscribe({
      next: (updatedBusket) => {
        this.busket = updatedBusket;
      },
      error: (err) => {
        console.error("שגיאה בהוספת כרטיס:", err);
        alert("לא ניתן להוסיף את הכרטיס: " + err.error.message);
      }
    });
  }
  get totalPurchasedTickets(): number {
    if (!this.busket || !this.busket.purchasePackages) return 0;

    return this.busket.purchasePackages.reduce((sum, pp) => {
      const ticketsInPackage = pp.package?.quantity || 0;
      return sum + ((pp.quantity || 0) * ticketsInPackage);
    }, 0);
  }

  get usedTickets(): number {
    if (!this.busket || !this.busket.tickets) return 0;
    return this.busket.tickets.length;
  }

  get remainingTickets(): number {
    const remaining = this.totalPurchasedTickets - this.usedTickets;
    return remaining > 0 ? remaining : 0;
  }
  get totalAmount(): number {
    if (!this.busket || !this.busket.purchasePackages) return 0;
    return this.busket.purchasePackages.reduce((sum, pp) => {
      const pricePerPackage = pp.package?.price || 0;
      return sum + ((pp.quantity || 0) * pricePerPackage);
    }
      , 0);
  }
  // completePurchase() {
  //   this.router.navigate(['/payment']);

  // }
}

