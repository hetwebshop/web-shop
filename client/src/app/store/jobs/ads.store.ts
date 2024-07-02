import { Injectable } from '@angular/core';
import { ActiveState, EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { UserJobPost } from 'src/app/models/userJobPost';

export interface AdsState extends EntityState<UserJobPost, number>, ActiveState {}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'ads' })
export class AdsStore extends EntityStore<AdsState> {
  constructor() {
    super() ;
  }
}