import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, throwIfEmpty } from 'rxjs';
import { CategoryModel } from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  BASE_URL = 'https://localhost:7280/api/Category';
  http: HttpClient = inject(HttpClient);
  constructor() { }
  getAll(): Observable<CategoryModel[]> {
    return this.http.get<CategoryModel[]>(this.BASE_URL);
  }
  getById(id: number): Observable<CategoryModel> {
    return this.http.get<CategoryModel>(this.BASE_URL + '/' + id);
  }
  add(item: CategoryModel): Observable<CategoryModel> {
    return this.http.post<CategoryModel>(this.BASE_URL, item);
  }
  update(id: number, item: CategoryModel): Observable<CategoryModel> {
    return this.http.put<CategoryModel>(this.BASE_URL + `/${id}`, item);
  }
  delete(id: number) {
    return this.http.delete(this.BASE_URL + `/${id}`);
  }
}
