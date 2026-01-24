import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { DonorService } from '../../services/donor.service';
import { DonorModel } from '../../models/donor.model';

@Component({
  selector: 'app-donor',
  imports: [CommonModule, FormsModule],
  templateUrl: './donor.component.html',
  styleUrl: './donor.component.css'
})
export class DonorComponent {
  donorSrv = inject(DonorService);
  list$ = this.donorSrv.getAll();
  draftDonor: DonorModel = {
    id: '',
    name: '',
    phoneNumber: '',
    email: '',
    logoUrl: ''
  };
  isEditMode = false;
  openEdit(d: DonorModel) {
    this.isEditMode = true;
    this.draftDonor = {
      id: d.id ?? '',
      name: d.name ?? '',
      phoneNumber: d.phoneNumber ?? '',
      email: d.email ?? '',
      logoUrl: d.logoUrl ?? ''
    };
  }

  save() {
    if (!this.draftDonor.name || !this.draftDonor.email) return;
    const id = this.draftDonor.id;
    if (this.isEditMode) {
      this.donorSrv.update(id!, this.draftDonor).subscribe(() => {
        this.refreshList();
        this.resetForm();
      });
    } else {
      this.donorSrv.add(this.draftDonor).subscribe(() => {
        this.refreshList();
        this.resetForm();
      });
    }
  }

  delete(id: string) {
    this.donorSrv.delete(id).subscribe(() => {
      this.refreshList();
    });
  }
  filter(name?: string, email?: string, giftId?: number) {
    this.list$ = this.donorSrv.filter(name, email, giftId);
  }

  refreshList() {
    this.list$ = this.donorSrv.getAll();
  }

  resetForm() {
    this.isEditMode = false;
    this.draftDonor = { id: '', name: '', phoneNumber: '', email: '', logoUrl: '' };
  }
}