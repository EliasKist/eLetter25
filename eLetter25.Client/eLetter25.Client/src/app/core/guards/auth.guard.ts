import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../auth/services/auth.service';

export const authGuard: CanActivateFn = () => {
  const isAuthenticated = inject(AuthService).isAuthenticated();
  if (isAuthenticated) {
    return true;
  }
  return inject(Router).createUrlTree(['/login']);
};

