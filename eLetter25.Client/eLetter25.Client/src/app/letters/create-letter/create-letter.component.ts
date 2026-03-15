import {Component, DestroyRef, inject, signal} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {NgxExtendedPdfViewerModule} from 'ngx-extended-pdf-viewer';
import {LetterService} from '../services/letter.service';
import {CreateLetterRequest} from '../models/letter.models';
import {formGetErrorMessage, formHasError} from '../../core/utils/form-validation.utils';

type PreviewType = 'pdf' | 'image' | null;

const ACCEPTED_TYPES: Record<string, PreviewType> = {
  'application/pdf': 'pdf',
  'image/png': 'image',
  'image/jpeg': 'image'
};

@Component({
  selector: 'app-create-letter',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, NgxExtendedPdfViewerModule],
  templateUrl: './create-letter.component.html',
  styleUrl: './create-letter.component.scss'
})
export class CreateLetterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly letterService = inject(LetterService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly form = this.fb.group({
    subject: ['', [Validators.required, Validators.maxLength(200)]],
    sentDate: ['', Validators.required],
    sender: this.fb.group({
      name: ['', Validators.required],
      address: this.fb.group({
        street: ['', Validators.required],
        postalCode: ['', Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required]
      }),
      email: ['', Validators.email],
      phone: ['']
    }),
    recipient: this.fb.group({
      name: ['', Validators.required],
      address: this.fb.group({
        street: ['', Validators.required],
        postalCode: ['', Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required]
      }),
      email: ['', Validators.email],
      phone: ['']
    }),
    tags: ['']
  });

  protected selectedFile = signal<File | null>(null);
  protected previewUrl = signal<string | null>(null);
  protected previewType = signal<PreviewType>(null);
  protected isSubmitting = signal(false);
  protected successLetterId = signal<string | null>(null);
  protected errorMessage = signal<string | null>(null);
  protected fileError = signal<string | null>(null);

  private rawObjectUrl: string | null = null;

  constructor() {
    this.destroyRef.onDestroy(() => {
      if (this.rawObjectUrl) {
        URL.revokeObjectURL(this.rawObjectUrl);
      }
    });
  }

  protected onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    this.setFile(file);
    input.value = '';
  }

  protected onDrop(event: DragEvent): void {
    event.preventDefault();
    const file = event.dataTransfer?.files?.[0] ?? null;
    this.setFile(file);
  }

  protected onDragOver(event: DragEvent): void {
    event.preventDefault();
  }

  protected clearFile(): void {
    this.setFile(null);
  }

  protected submit(): void {
    if (!this.selectedFile()) {
      this.fileError.set('Please select a document before submitting.');
      return;
    }

    if (this.form.invalid || this.isSubmitting()) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);
    this.successLetterId.set(null);

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

    this.letterService.createLetter(request, this.selectedFile()!).subscribe({
      next: result => {
        this.successLetterId.set(result.letterId);
        this.isSubmitting.set(false);
        this.form.reset();
        this.clearFile();
      },
      error: err => {
        const status = err.status as number;
        this.errorMessage.set(
          status === 401
            ? 'Unauthorized. Please log in first.'
            : status === 400
              ? (err.error?.error ?? 'Invalid request. Please check all fields.')
              : 'An unexpected error occurred. Please try again.'
        );
        this.isSubmitting.set(false);
      }
    });
  }

  protected hasError = (path: string) => formHasError(this.form, path);
  protected getErrorMessage = (path: string) => formGetErrorMessage(this.form, path);

  private setFile(file: File | null): void {
    if (this.rawObjectUrl) {
      URL.revokeObjectURL(this.rawObjectUrl);
      this.rawObjectUrl = null;
    }

    this.fileError.set(null);

    if (!file) {
      this.selectedFile.set(null);
      this.previewUrl.set(null);
      this.previewType.set(null);
      return;
    }

    const type = ACCEPTED_TYPES[file.type];
    if (!type) {
      this.selectedFile.set(null);
      this.previewUrl.set(null);
      this.previewType.set(null);
      this.fileError.set('Unsupported file type. Please upload a PDF, PNG, or JPEG.');
      return;
    }

    this.rawObjectUrl = URL.createObjectURL(file);
    this.selectedFile.set(file);
    this.previewType.set(type);
    this.previewUrl.set(this.rawObjectUrl);
  }
}
