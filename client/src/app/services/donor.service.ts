import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DonorModel } from '../models/donor.model';

@Injectable({
  providedIn: 'root'
})
export class DonorService {

  BASE_URL = 'https://localhost:7280/api/Donor';
  http: HttpClient = inject(HttpClient);
  constructor() { }
  getAll(): Observable<DonorModel[]> {
    return this.http.get<DonorModel[]>(this.BASE_URL);
  }
  getById(id: string): Observable<DonorModel> {
    return this.http.get<DonorModel>(this.BASE_URL + '/' + id);
  }
  add(item: DonorModel): Observable<DonorModel> {
    console.log(item);

    return this.http.post<DonorModel>(this.BASE_URL, item)
  }
  update(id: string, item: DonorModel): Observable<DonorModel> {
    return this.http.put<DonorModel>(this.BASE_URL + `/${id}`, item);
  }
  delete(id: string) {
    return this.http.delete(this.BASE_URL + `/${id}`)
  }

  filter(name?: string, email?: string, giftId?: number): Observable<DonorModel[]> {
    let params = new HttpParams();
    if (name) params = params.set('name', name);
    if (email) params = params.set('email', email);
    if (giftId !== undefined && giftId !== null) params = params.set('giftId', giftId.toString());
    return this.http.get<DonorModel[]>(`${this.BASE_URL}/FilterDonors`, { params });
  }
}
