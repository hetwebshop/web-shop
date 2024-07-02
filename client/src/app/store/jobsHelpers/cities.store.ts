import { Injectable } from '@angular/core';
import { ActiveState, EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { City } from 'src/app/models/location';

export interface CityState extends EntityState<City, number>, ActiveState {}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'cities' })
export class CityStore extends EntityStore<CityState> {
  constructor() {
    super() ;
  }
}