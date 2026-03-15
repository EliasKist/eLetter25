import { HttpErrorResponse } from '@angular/common/http';

/**
 * Extracts a human-readable error message from an Angular HttpErrorResponse.
 * Falls back to the provided default message when no structured error body is available.
 */
export function getApiErrorMessage(err: HttpErrorResponse, defaultMessage: string): string {
  if (err.status === 401) {
    return 'Nicht autorisiert. Bitte melde dich erneut an.';
  }
  if (err.status >= 400 && err.status < 500) {
    return err.error?.detail ?? err.error?.error ?? defaultMessage;
  }
  return defaultMessage;
}

