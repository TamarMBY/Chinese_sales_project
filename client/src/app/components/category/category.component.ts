import { Component, inject } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { CategoryModel } from '../../models/category.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { GiftComponent } from "../gift/gift.component";
@Component({
  selector: 'app-category',
  imports: [CommonModule, FormsModule, GiftComponent, GiftComponent],
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})
export class CategoryComponent {
  categorySrv: CategoryService = inject(CategoryService);
  list$ = this.categorySrv.getAll();
  draftCategory: CategoryModel = {
    id: 0,
    name: '',
    description: ''
  };
  isEditMode = false;
  categoryIdForFilter: number = 0;
  getById(id: number) {
    console.log(id);
    if (!id || id <= 0) this.categoryIdForFilter = id;
    else {
      this.categorySrv.getById(id).subscribe(c => {
        this.categoryIdForFilter = c.id!;
      });
    }
  }
  openEdit(c: CategoryModel) {
    this.isEditMode = true;
    this.draftCategory = {
      id: c.id ?? 0,
      name: c.name ?? '',
      description: c.description ?? ''
    };
  }
  save() {
    if (!this.draftCategory.name) return;
    const id = this.draftCategory.id;
    if (this.isEditMode) {
      this.categorySrv.update(id!, this.draftCategory).subscribe(() => {
        this.refreshList();
        this.resetForm();
      });
    } else {
      this.categorySrv.add(this.draftCategory).subscribe(() => {
        this.refreshList();
        this.resetForm();
      });
    }
  }

  delete(id: number) {
    this.categorySrv.delete(id).subscribe(c => {
      this.refreshList();
    })
  }

  refreshList() {
    this.list$ = this.categorySrv.getAll();
  }

  resetForm() {
    this.isEditMode = false;
    this.draftCategory = { id: 0, name: '', description: '' };
  }
}
