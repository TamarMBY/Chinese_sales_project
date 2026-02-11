import { Routes } from '@angular/router';
import { CategoryComponent } from './components/category/category.component';
import { DonorComponent } from './components/donor/donor.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { GetGiftComponent } from './components/gift/get-gift/get-gift.component';
import { PackageComponent } from './components/package/package.component';
import { PaymentComponent } from './components/payment/payment.component';
import { HomePageComponent } from './components/home-page/home-page.component';


export const routes: Routes = [
    {path:'', component:HomePageComponent},
    {path:'package',component:PackageComponent},
    {path:'category',component:CategoryComponent},
    {path:'gift/:id',component:GetGiftComponent},
    {path:'donors',component:DonorComponent},
    {path:'login',component:LoginComponent},
    {path:'register',component:RegisterComponent},
    {path: 'payment/:busketId', component: PaymentComponent}
    // {path:'logout', component:Login}
];
