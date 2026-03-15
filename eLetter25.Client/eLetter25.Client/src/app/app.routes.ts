import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { CreateLetterComponent } from './letters/create-letter/create-letter.component';
import { LettersListComponent } from './letters/letters-list/letters-list.component';
import { authGuard } from './core/guards/auth.guard';
import { SettingsComponent } from './settings/settings.component';
import { ShellComponent } from './shell/shell.component';

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
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        component: DashboardComponent,
        title: 'Dashboard'
      },
      {
        path: 'letters/create',
        component: CreateLetterComponent,
        title: 'Create Letter'
      },
      {
        path: 'letters',
        component: LettersListComponent,
        title: 'Letters'
      },
      {
        path: 'settings',
        component: SettingsComponent,
        title: 'Settings'
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
