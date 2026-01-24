import { Routes } from '@angular/router';
import { CategoryComponent } from './components/category/category.component';
import { DonorComponent } from './components/donor/donor.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { GetGiftComponent } from './components/gift/get-gift/get-gift.component';


export const routes: Routes = [
    // {path:'', component:Home},
    { path: 'categories', component: CategoryComponent, children: [
            { path: 'gift/:id', component: GetGiftComponent },
    ]},
    { path: 'donors', component: DonorComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    // {path:'logout', component:Login}
];
