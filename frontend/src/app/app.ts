import {
  Component,
  inject,
  OnDestroy,
  signal,
  computed,
  ElementRef,
  viewChild,
  ChangeDetectionStrategy,
  OnInit,
} from '@angular/core';
import { Search } from './services/search/search';
import { SearchResultCard } from './models/search-result-card';
import { InputTextModule } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { Subject, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap, catchError } from 'rxjs/operators';
import { FrameData } from './components/blocks/frame-data/frame-data';
import { LoaderCircle, LucideAngularModule, SearchIcon } from 'lucide-angular';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [
    InputTextModule,
    FormsModule,
    FrameData,
    LucideAngularModule,
    IconFieldModule,
    InputIconModule,
  ],
  templateUrl: './app.html',
  styleUrl: './app.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App implements OnDestroy, OnInit {
  protected readonly results = signal<SearchResultCard[]>([]);
  protected readonly isLoading = signal(false);
  protected readonly hasSearched = signal(false);
  protected query = '';

  private readonly search = inject(Search);
  private readonly searchSubject = new Subject<string>();

  protected readonly searchInput = viewChild<ElementRef<HTMLInputElement>>('searchInput');

  protected readonly SearchIcon = SearchIcon;
  protected readonly SpinnerIcon = LoaderCircle;

  protected readonly showEmptyState = computed(
    () => this.hasSearched() && !this.isLoading() && this.results().length === 0,
  );

  constructor() {
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap((query) => {
          if (query.trim()) {
            this.isLoading.set(true);
          }
        }),
        switchMap((query) => {
          if (query.trim()) {
            return this.search.execute(query);
          }
          this.results.set([]);
          this.hasSearched.set(false);
          return of([]);
        }),
      )
      .subscribe({
        next: (results) => {
          if (environment.isProduction) {
            // @ts-ignore
            umami.track('search', { query: this.query, resultCount: results.length });
          }
          this.results.set(results);
          this.isLoading.set(false);
          if (this.query.trim()) {
            this.hasSearched.set(true);
          }
        },
      });
  }
  ngOnInit(): void {
    if (environment.isProduction) {
      // @ts-ignore
      umami.track();
    }
  }

  protected onQueryChange(query: string): void {
    this.searchSubject.next(query);
  }

  ngOnDestroy(): void {
    this.searchSubject.complete();
  }
}
