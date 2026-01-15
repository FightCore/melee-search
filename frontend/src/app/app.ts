import { Component, inject, OnDestroy, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Search } from './services/search/search';
import { SearchResultCard } from './models/search-result-card';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { FrameData } from './components/blocks/frame-data/frame-data';
import { LucideAngularModule, SearchIcon } from 'lucide-angular';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, InputTextModule, FormsModule, FrameData, LucideAngularModule],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App implements OnDestroy {
  protected readonly results = signal<SearchResultCard[]>([]);
  protected query = '';
  private readonly search = inject(Search);
  private readonly searchSubject = new Subject<string>();
  protected SearchIcon = SearchIcon;

  constructor() {
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        switchMap((query) => {
          if (query.trim()) {
            return this.search.execute(query);
          }
          this.results.set([]);
          return [];
        })
      )
      .subscribe({
        next: (results) => this.results.set(results),
        error: (err) => console.error('Search failed:', err),
      });
  }

  protected onQueryChange(query: string): void {
    this.searchSubject.next(query);
  }

  ngOnDestroy(): void {
    this.searchSubject.complete();
  }
}
