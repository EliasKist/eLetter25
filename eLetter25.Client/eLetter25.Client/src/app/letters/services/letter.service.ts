import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateLetterRequest, CreateLetterResult } from '../models/letter.models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LetterService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/api/letters`;

  createLetter(request: CreateLetterRequest): Observable<CreateLetterResult> {
    return this.http.post<CreateLetterResult>(this.baseUrl, request);
  }
}

