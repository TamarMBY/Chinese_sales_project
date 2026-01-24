import { inject, Injectable } from '@angular/core';
import { GiftModel } from '../models/gift.model';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class GiftService {
  BASE_URL = 'https://localhost:7280/api/Gift';
  http: HttpClient = inject(HttpClient);
  constructor() { }
  getAll(): Observable<GiftModel[]> {
    return this.http.get<GiftModel[]>(this.BASE_URL);
  }
  getById(id: number): Observable<GiftModel> {
    return this.http.get<GiftModel>(this.BASE_URL + '/' + id);
  }
  add(item: GiftModel): Observable<GiftModel> {
    console.log(item);
    return this.http.post<GiftModel>(this.BASE_URL, item)
  }
  update(id: number, item: GiftModel): Observable<GiftModel> {
    return this.http.put<GiftModel>(this.BASE_URL + `/${id}`, item);
  }
  delete(id: number) {
    return this.http.delete(this.BASE_URL + `/${id}`)
  }
  filter(name?:string, categoryId?:number, donorId?: string, buyerCount?:number): Observable<GiftModel[]> {
    let params = new HttpParams();
    if(name) params = params.set('name', name);
    if(categoryId) params = params.set('categoryId', categoryId.toString());
    if(donorId) params = params.set('donorId', donorId);
    if(buyerCount) params = params.set('buyerCount', buyerCount.toString());
    return this.http.get<GiftModel[]>(this.BASE_URL + '/filterGifts', {params});
  }
  lottery(giftId: number): Observable<void> {
    return this.http.post<void>(this.BASE_URL + `/lottery/${giftId}`, {});
  }
}
