import { Injectable } from '@angular/core';
import { ActiveState, EntityState, EntityStore, StoreConfig } from '@datorama/akita';
import { JobCategory, UserJobPost } from 'src/app/models/userJobPost';

export interface JobCategoryState extends EntityState<JobCategory, number>, ActiveState {}

@Injectable({ providedIn: 'root' })
@StoreConfig({ name: 'jobCategories' })
export class JobCategoryStore extends EntityStore<JobCategoryState> {
  constructor() {
    super() ;
  }
}