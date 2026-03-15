import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateLetterRequest, CreateLetterResult, GetLettersResult } from '../models/letter.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LetterService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/letters`;

  createLetter(request: CreateLetterRequest, file: File): Observable<CreateLetterResult> {
    const formData = new FormData();
    formData.append('metadata', JSON.stringify(request));
    formData.append('document', file, file.name);
    return this.http.post<CreateLetterResult>(this.baseUrl, formData);
  }

  getLetters(): Observable<GetLettersResult> {
    return this.http.get<GetLettersResult>(this.baseUrl);
  }
}
