import { AbstractControl } from '@angular/forms';

export function formHasError(form: AbstractControl, path: string): boolean {
  const control = form.get(path);
  return !!control && control.invalid && control.touched;
}

export function formGetErrorMessage(form: AbstractControl, path: string): string {
  const control = form.get(path);
  if (!control) {
    return '';
  }
  if (control.hasError('required')) {
    return 'This field is required.';
  }
  if (control.hasError('email')) {
    return 'Please enter a valid email address.';
  }
  if (control.hasError('minlength')) {
    const requiredLength = control.getError('minlength').requiredLength as number;
    return `Minimum length is ${requiredLength} characters.`;
  }
  if (control.hasError('maxlength')) {
    return 'Maximum length exceeded.';
  }
  return '';
}

