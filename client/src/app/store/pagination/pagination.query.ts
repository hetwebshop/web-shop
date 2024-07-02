import { QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { PaginationState, PaginationStore } from './pagination.store';
@Injectable({
  providedIn: 'root',
})
export class PaginationQuery extends QueryEntity<PaginationState> {
  constructor(protected store: PaginationStore) {
    super(store);
  }
}