import { QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { CityState, CityStore } from './cities.store';
@Injectable({
  providedIn: 'root',
})
export class CityQuery extends QueryEntity<CityState> {
  constructor(protected store: CityStore) {
    super(store);
  }
}