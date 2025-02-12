import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { Subject, Subscription, takeUntil } from 'rxjs';
import { BaseContext } from 'src/app/base/modal';
import { MediaService } from 'src/app/services/media.service';

@Component({
  selector: 'app-filter-list',
  templateUrl: './filter-list.component.html',
  styleUrls: ['./filter-list.component.css'],
})
export class FilterListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  @Input() context: BaseContext;
  @Output() pageChange = new EventEmitter<number>();
  lt_md = false;
  mediaSubscription: Subscription;

  constructor(private mediaObserver: MediaService) {}

  ngOnInit(): void {
    this.mediaSubscription = this.mediaObserver.mediaChange$.pipe(takeUntil(this.destroy$)).subscribe(
      (changes) => {
        this.lt_md = changes.includes('lt-md');
      }
    );
  }

  onPageChange(page: number) {
    window.scroll({
      top: 0,
      left: 0,
      behavior: 'smooth',
    });
    this.pageChange.emit(page);
  }

  ngOnDestroy() {
    // Trigger cleanup of all subscriptions
    this.destroy$.next();
    this.destroy$.complete();
  }
}
