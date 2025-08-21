import { Routes } from '@angular/router';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ContactNewComponent } from './components/contact-new/contact-new.component';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'add-contact', component: ContactNewComponent },
  { path: '**', redirectTo: '/dashboard' }
];