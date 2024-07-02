import { Injectable } from '@angular/core';
import { ActiveState, EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { PaginationParameters } from 'src/app/models/filterCriteria';

export interface PaginationState extends EntityState<PaginationParameters, number>, ActiveState {}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'pagination' })
export class PaginationStore extends EntityStore<PaginationState> {
  constructor() {
    super() ;
  }
}