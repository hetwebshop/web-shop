import { Injectable } from '@angular/core';
import { ActiveState, EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { Country } from 'src/app/models/location';

export interface CountryState extends EntityState<Country, number>, ActiveState {}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'countries' })
export class CountryStore extends EntityStore<CountryState> {
  constructor() {
    super() ;
  }
}