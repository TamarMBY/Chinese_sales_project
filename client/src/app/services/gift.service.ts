import { inject, Injectable } from '@angular/core';
import { GiftModel } from '../models/gift.model';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';

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
}
