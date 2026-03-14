import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LetterService } from '../services/letter.service';
import { CreateLetterRequest } from '../models/letter.models';
import { formGetErrorMessage, formHasError } from '../../core/utils/form-validation.utils';

@Component({
  selector: 'app-create-letter',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './create-letter.component.html',
  styleUrl: './create-letter.component.scss'
})
export class CreateLetterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly letterService = inject(LetterService);

  private buildAddressGroup() {
    return this.fb.group({
      street: ['', Validators.required],
      postalCode: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required]
    });
  }

  private buildCorrespondentGroup() {
    return this.fb.group({
      name: ['', Validators.required],
      address: this.buildAddressGroup(),
      email: ['', Validators.email],
      phone: ['']
    });
  }

  protected readonly form = this.fb.group({
    subject: ['', [Validators.required, Validators.maxLength(200)]],
    sentDate: ['', Validators.required],
    sender: this.buildCorrespondentGroup(),
    recipient: this.buildCorrespondentGroup(),
    tags: ['']
  });

  protected isSubmitting = false;
  protected successLetterId: string | null = null;
  protected errorMessage: string | null = null;


  protected submit(): void {
    if (this.form.invalid || this.isSubmitting) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = null;
    this.successLetterId = null;

    const raw = this.form.getRawValue();
    const request: CreateLetterRequest = {
      subject: raw.subject ?? '',
      sentDate: new Date(raw.sentDate ?? '').toISOString(),
      sender: {
        name: raw.sender.name ?? '',
        address: {
          street: raw.sender.address.street ?? '',
          postalCode: raw.sender.address.postalCode ?? '',
          city: raw.sender.address.city ?? '',
          country: raw.sender.address.country ?? ''
        },
        email: raw.sender.email || null,
        phone: raw.sender.phone || null
      },
      recipient: {
        name: raw.recipient.name ?? '',
        address: {
          street: raw.recipient.address.street ?? '',
          postalCode: raw.recipient.address.postalCode ?? '',
          city: raw.recipient.address.city ?? '',
          country: raw.recipient.address.country ?? ''
        },
        email: raw.recipient.email || null,
        phone: raw.recipient.phone || null
      },
      tags: raw.tags
        ? raw.tags.split(',').map(t => t.trim()).filter(t => t.length > 0)
        : []
    };

    this.letterService.createLetter(request).subscribe({
      next: result => {
        this.successLetterId = result.letterId;
        this.isSubmitting = false;
        this.form.reset();
      },
      error: err => {
        this.errorMessage = err.status === 401
          ? 'Unauthorized. Please log in first.'
          : 'An error occurred while creating the letter. Please try again.';
        this.isSubmitting = false;
      }
    });
  }

  protected hasError = (path: string) => formHasError(this.form, path);
  protected getErrorMessage = (path: string) => formGetErrorMessage(this.form, path);
}


