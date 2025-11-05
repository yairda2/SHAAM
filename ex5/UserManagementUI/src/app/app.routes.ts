import { Routes } from '@angular/router';
import { UserListComponent } from './components/user-list/user-list.component';

/**
 * Application routing configuration
 * Defines navigation routes for the application
 */
export const routes: Routes = [
  {
    path: '',
    component: UserListComponent
  },
  {
    path: '**',
    redirectTo: ''
  }
];
