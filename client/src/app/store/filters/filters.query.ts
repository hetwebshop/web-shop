import { QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { FiltersState, FiltersStore } from './filters.store';
@Injectable({
  providedIn: 'root',
})
export class FiltersQuery extends QueryEntity<FiltersState> {
  constructor(protected store: FiltersStore) {
    super(store);
  }
}