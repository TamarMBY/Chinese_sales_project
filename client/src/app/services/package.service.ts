import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { PackageModel } from '../models/package.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PackageService {
  BASE_URL = 'https://localhost:7280/api/Package';
  http: HttpClient = inject(HttpClient);
  constructor() { }
  getAll() : Observable<PackageModel[]> {
    return this.http.get<PackageModel[]>(this.BASE_URL);
  }
  getById(id: number) : Observable<PackageModel>{
    return this.http.get(this.BASE_URL + '/' + id);
  }
  add(item: PackageModel): Observable<PackageModel> {
    console.log(item);
    return this.http.post(this.BASE_URL, item)
  }
  update(id: number, item: PackageModel) : Observable<PackageModel>{
    return this.http.put(this.BASE_URL + `/${id}`, item);
  }
  delete(id: number) {
    return this.http.delete(this.BASE_URL + `/${id}`)
  }
  sort(sortBy?: string) : Observable<PackageModel[]>{
    let params = new HttpParams();
    if(sortBy) params = params.set('sortBy', sortBy);
    console.log("im in sort in service:" + sortBy);
    return this.http.get<PackageModel[]>(this.BASE_URL + `/sortPackages`, { params });
  }
}
