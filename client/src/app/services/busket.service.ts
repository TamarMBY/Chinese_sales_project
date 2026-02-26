import { HttpClient } from '@angular/common/http';
import { EventEmitter, inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { BusketModel } from '../models/busket.model';
import { TicketModel } from '../models/ticket.model';

@Injectable({
  providedIn: 'root'
})
export class BusketService {
  // כתובת ה-API שלך
  BASE_URL = 'https://localhost:7280/api/Purchase';
  
  // הזרקת ה-HttpClient
  private http: HttpClient = inject(HttpClient);

  /**
   * איוונט שמופעל בכל פעם שיש שינוי בסל (הוספה/מחיקה).
   * קומפוננטות אחרות (כמו ה-Header או ה-Sidebar) יכולות להירשם אליו.
   */
  public busketUpdated: EventEmitter<void> = new EventEmitter<void>();

  constructor() { }

  getById(id: number): Observable<BusketModel> {
    return this.http.get<BusketModel>(`${this.BASE_URL}/${id}`);
  }

  getByUserId(userId: string): Observable<BusketModel> {
    return this.http.get<BusketModel>(`${this.BASE_URL}/by-user/${userId}`);
  }

  addPackage(purchaseId: number, item: number): Observable<BusketModel> {
    return this.http.post<BusketModel>(`${this.BASE_URL}/${purchaseId}/${item}/packages`, {}).pipe(
      tap(() => this.busketUpdated.emit())
    );
  }

  deletePackage(purchaseId: number, packageId: number): Observable<BusketModel> {
    return this.http.delete<BusketModel>(`${this.BASE_URL}/deletePackage/${purchaseId}/${packageId}`).pipe(
      tap(() => this.busketUpdated.emit())
    );
  }

  /**
   * הוספת כרטיס לסל. 
   * שימוש ב-tap מבטיח שה-emit יקרה רק אם ה-POST הצליח.
   */
  addTicket(ticket: TicketModel): Observable<BusketModel> {
    return this.http.post<BusketModel>(`${this.BASE_URL}/addTicket`, ticket).pipe(
      tap(() => {
        console.log('Ticket added, emitting update...');
        this.busketUpdated.emit();
      })
    );
  }

  /**
   * מחיקת כרטיס מהסל.
   */
  deleteTicket(purchaseId: number, ticketId: number): Observable<BusketModel> {
    return this.http.delete<BusketModel>(`${this.BASE_URL}/${purchaseId}/${ticketId}`).pipe(
      tap(() => {
        console.log('Ticket deleted, emitting update...');
        this.busketUpdated.emit();
      })
    );
  }

  completePurchase(purchase: BusketModel): Observable<any> {
    return this.http.put(`${this.BASE_URL}/completePurchase/${purchase.id}`, purchase).pipe(
      tap(() => this.busketUpdated.emit())
    );
  }
}