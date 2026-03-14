import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { CreateLetterComponent } from './letters/create-letter/create-letter.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    title: 'Sign In'
  },
  {
    path: 'register',
    component: RegisterComponent,
    title: 'Create Account'
  },
  {
    path: 'letters/create',
    component: CreateLetterComponent,
    title: 'Create Letter',
    canActivate: [authGuard]
  },
  {
    path: '',
    redirectTo: 'letters/create',
    pathMatch: 'full'
  },
  {
    path: '**',
    redirectTo: 'letters/create'
  }
];
