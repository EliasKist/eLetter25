import { Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { LetterService } from '../services/letter.service';
import { LetterSummary } from '../models/letter.models';
import { getApiErrorMessage } from '../../core/utils/api-error.utils';

@Component({
  selector: 'app-letters-list',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './letters-list.component.html',
  styleUrl: './letters-list.component.scss'
})
export class LettersListComponent implements OnInit {
  private readonly letterService = inject(LetterService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly letters = signal<LetterSummary[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.letterService.getLetters()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: result => {
          this.letters.set(result.letters);
          this.isLoading.set(false);
        },
        error: err => {
          this.errorMessage.set(getApiErrorMessage(err, 'Die Briefe konnten nicht geladen werden.'));
          this.isLoading.set(false);
        }
      });
  }
}
