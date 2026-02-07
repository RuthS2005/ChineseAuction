import { Routes } from '@angular/router';
import { HomePage } from './home-page/home-page';
import { DonorsManagement } from './donors-management/donors-management';
import { DonorForm } from './donors-management/donor-form/donor-form';
import { DonorsList } from './donors-management/donors-list/donors-list';
import { GiftDonation } from './gift-donation/gift-donation';
import { Tickets } from './tickets/tickets';
import { LoginComponent } from './login/login';
import { CartComponent } from './tickets/cart/cart';

export const routes: Routes = [
      { path: 'home', component: HomePage},
        { path: 'login', component: LoginComponent }, // <--- הנתיב החדש!

      { path: 'donors', component: DonorsManagement },
      {
  path: 'donors',
  component: DonorsManagement,
  children: [
    { path: 'add', component: DonorForm },
    { path:'list',component:DonorsList}
  ]
},
{path:'gifts',component:GiftDonation},
{path:'tickets', component:Tickets},
{path:'cart', component:CartComponent}


];
