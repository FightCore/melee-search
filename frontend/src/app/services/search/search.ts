import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchResultCard } from '../../models/search-result-card';

@Injectable({
  providedIn: 'root',
})
export class Search {
  private readonly httpClient = inject(HttpClient);

  execute(search: string): Observable<SearchResultCard[]> {
    return this.httpClient.get<SearchResultCard[]>(
      'https://localhost:7058/api/search?q=' + encodeURIComponent(search)
    );
  }
}
