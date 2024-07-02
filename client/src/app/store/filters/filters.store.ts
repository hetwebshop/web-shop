import { Injectable } from '@angular/core';
import { ActiveState, EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { AdsPaginationParameters } from 'src/app/models/filterCriteria';
import { UserJobPost } from 'src/app/models/userJobPost';

export interface FiltersState extends EntityState<AdsPaginationParameters, number>, ActiveState {}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'filters' })
export class FiltersStore extends EntityStore<FiltersState> {
  constructor() {
    super() ;
  }
}