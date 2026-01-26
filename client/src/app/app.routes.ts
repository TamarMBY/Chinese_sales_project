import { Routes } from '@angular/router';
import { CategoryComponent } from './components/category/category.component';
import { DonorComponent } from './components/donor/donor.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { GetGiftComponent } from './components/gift/get-gift/get-gift.component';
import { PackageComponent } from './components/package/package.component';


export const routes: Routes = [
    // {path:'', component:Home},
    {path: 'packages', component: PackageComponent},
    { path: 'categories', component: CategoryComponent },
    { path: 'gift/:id', component: GetGiftComponent },
    { path: 'donors', component: DonorComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    // {path:'logout', component:Login}
];
