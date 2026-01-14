import { Component, inject } from '@angular/core';
import { CategoryService } from '../../services/category';
import { CategoryModel } from '../../models/category.model';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-category',
  imports: [CommonModule],
  templateUrl: './category.html',
  styleUrl: './category.css',
})
export class Category {
  crudSrv: CategoryService = inject(CategoryService);
  list$ = this.crudSrv.getAll();
  add(name: string, descriptoin: string| undefined){
    if(name)
      this.crudSrv.add({name: name, descriptoin: descriptoin}).subscribe(data =>{
    this.list$ = this.crudSrv.getAll();
  })
  }
  flagUpdate: boolean = false;
  currentId: number = 0;
  openUpdate(id: number){
  if(!this.flagUpdate)
    this.currentId = id;
    this.flagUpdate = !this.flagUpdate;
  }
  update(iname: string, descriptoin: string| undefined) {  
    item: CategoryModel = {} 
    item = this.crudSrv.getById(this.currentId!)
    this.crudSrv.update(item).subscribe(d => {
      this.list$ =  this.crudSrv.getAll();
    })
  }
  delete(id: number){
    this.crudSrv.delete(id).subscribe(d => {
      this.list$ = this.crudSrv.getAll();
    })
  }
}
