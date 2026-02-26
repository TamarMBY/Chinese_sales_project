import { Component, inject } from '@angular/core'; // הוספנו OnInit
import { PackageService } from '../../services/package.service';
import { PackageModel } from '../../models/package.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BusketService } from '../../services/busket.service';
import { BusketModel } from '../../models/busket.model';
import { AuthService } from '../../auth/auth.service';
// import { CardModule } from 'primeng/card';
// import { ButtonModule } from 'primeng/button';
// import { SelectModule } from 'primeng/select';
@Component({
   selector: 'app-package',
  standalone: true, // וודאי שזה מוגדר אם את בגרסאות חדשות
  imports: [CommonModule, FormsModule],
  templateUrl: './package.component.html',
  styleUrl: './package.component.css'
})
export class PackageComponent {
  packageSrv = inject(PackageService);
  busketSrv = inject(BusketService);
  authSrv: AuthService = inject(AuthService);
  list$ = this.packageSrv.getAll();
  currentBusket: BusketModel = {};
  draftPackage = { id: 0, name: '', description: '', quantity: 0, price: 0 };
  isEditMode = false;
  busket: BusketModel = {};
  user: any = {};
  sortOptions = [
    { label: 'מיון לפי מחיר יורד', value: 'price_desc' },
    { label: 'הכי נקנה', value: 'most_purchased' }
  ];
  selectedSort: string = 'price_desc';
  ngOnInit() {
    const user = localStorage.getItem('user');
    if (user) {
      const parsedUser = JSON.parse(user);
      this.busketSrv.getByUserId(parsedUser.id).subscribe(b => {
        this.busket = b;
      });
    }
  }
  get totalItemInCart() {
    return this.busket.purchasePackages?.reduce((sum, pkg) => sum + pkg.quantity!, 0) || 0;
  }

  getQuentityInCart(packageId: number): number{
    if (!this.busket || !this.busket.purchasePackages) return 0;

    const item = this.busket.purchasePackages.find(p => p.packageId === packageId);
    console.log("quantity: " + item);
    
    return item ? (item.quantity || 0) : 0;
  }
  getByUserId(userId: string) {
    this.busketSrv.getByUserId(userId).subscribe((b: any) => {
      this.busket = b;
    });
  }
  addPackage(item: PackageModel) {
    return this.busketSrv.addPackage(this.busket.id!, item.id!).subscribe((b: BusketModel) => {
      this.busket = b;
    });
  }

  deletePackage(packageId: number) {
    return this.busketSrv.deletePackage(this.busket.id!, packageId).subscribe((b: BusketModel) => {
      this.busket = b;
      // window.location.reload();
    });
  }
  save() {
    if (!this.draftPackage.name) return;
    const id = this.draftPackage.id;
    if (this.isEditMode) {
      this.packageSrv.update(id!, this.draftPackage).subscribe(() => {
        this.refreshList();
        this.resetForm();
      });
    } else {
      this.packageSrv.add(this.draftPackage).subscribe(() => {
        this.refreshList();
        this.resetForm();
      });
    }
  }

  delete(id: number) {
    this.packageSrv.delete(id).subscribe(() => this.refreshList());
  }

  openEdit(p: PackageModel) {
    this.isEditMode = true;
    this.draftPackage = { ...p } as any; // העתקה מהירה של האובייקט
  }

  refreshList() {
    this.list$ = this.packageSrv.getAll();
  }

  resetForm() {
    this.isEditMode = false;
    this.draftPackage = { id: 0, name: '', description: '', quantity: 0, price: 0 };
  }
  sortBy(sortBy: string) {
    this.list$ = this.packageSrv.sort(sortBy);
  }
}