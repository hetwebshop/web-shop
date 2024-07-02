import { QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { CountryState, CountryStore } from './countries.store';
@Injectable({
  providedIn: 'root',
})
export class CountryQuery extends QueryEntity<CountryState> {
  constructor(protected store: CountryStore) {
    super(store);
  }
}