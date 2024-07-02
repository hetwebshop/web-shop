import { Injectable } from '@angular/core';
import { ActiveState, EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { JobType } from 'src/app/models/userJobPost';

export interface JobTypeState extends EntityState<JobType, number>, ActiveState {}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'jobTypes' })
export class JobTypeStore extends EntityStore<JobTypeState> {
  constructor() {
    super() ;
  }
}