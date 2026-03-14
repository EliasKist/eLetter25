import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { ApiErrorResponse } from '../../core/models/api-error.models';
import { formGetErrorMessage, formHasError } from '../../core/utils/form-validation.utils';

function passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
  const password = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return password === confirm ? null : { passwordsMismatch: true };
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly form = this.fb.group(
    {
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
      enableNotifications: [false]
    },
    { validators: passwordsMatchValidator }
  );

  protected isSubmitting = false;
  protected apiErrors: string[] = [];

  protected submit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.apiErrors = [];

    const raw = this.form.getRawValue();

    this.authService.register({
      email: raw.email ?? '',
      password: raw.password ?? '',
      enableNotifications: raw.enableNotifications ?? false
    }).subscribe({
      next: () => this.router.navigate(['/login'], { queryParams: { registered: true } }),
      error: (err: HttpErrorResponse) => {
        this.apiErrors = this.extractErrors(err);
        this.isSubmitting = false;
      }
    });
  }

  private extractErrors(err: HttpErrorResponse): string[] {
    const body = err.error as ApiErrorResponse | null;
    if (body?.errors?.length) {
      return body.errors.map(e => e.message);
    }
    return ['An unexpected error occurred. Please try again.'];
  }

  protected hasError = (path: string) => formHasError(this.form, path);
  protected getErrorMessage = (path: string) => formGetErrorMessage(this.form, path);

  protected get passwordsMismatch(): boolean {
    return this.form.hasError('passwordsMismatch') &&
      !!this.form.get('confirmPassword')?.touched;
  }
}

