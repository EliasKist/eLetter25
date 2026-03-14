import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { ApiErrorResponse } from '../../core/models/api-error.models';
import { formGetErrorMessage, formHasError } from '../../core/utils/form-validation.utils';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  protected isSubmitting = false;
  protected errorMessage: string | null = null;

  protected submit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;

    const { email, password } = this.form.getRawValue();

    this.authService.login({ email: email ?? '', password: password ?? '' }).subscribe({
      next: () => this.router.navigate(['/letters/create']),
      error: (err: HttpErrorResponse) => {
        this.errorMessage = this.extractErrorMessage(err);
        this.isSubmitting = false;
      }
    });
  }

  private extractErrorMessage(err: HttpErrorResponse): string {
    if (err.status === 401) {
      const body = err.error as ApiErrorResponse | null;
      return body?.errors?.[0]?.message ?? 'Invalid email or password.';
    }
    return 'An unexpected error occurred. Please try again.';
  }

  protected hasError = (path: string) => formHasError(this.form, path);
  protected getErrorMessage = (path: string) => formGetErrorMessage(this.form, path);
}

