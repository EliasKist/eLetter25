import {HttpInterceptorFn} from '@angular/common/http';
import {inject} from '@angular/core';
import {TokenStorageService} from '../services/token-storage.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(TokenStorageService).getToken();

  if (!token) {
    return next(req);
  }

  return next(req.clone({
    setHeaders: {Authorization: `Bearer ${token}`}
  }));
};
