import { QueryEntity } from '@datorama/akita';
import { AdsStore, AdsState } from './ads.store';
import { Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root',
})
export class AdsQuery extends QueryEntity<AdsState> {
  constructor(protected store: AdsStore) {
    super(store);
  }
}