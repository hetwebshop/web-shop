import { QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { JobCategoryState, JobCategoryStore } from './job-category.store';
@Injectable({
  providedIn: 'root',
})
export class JobCategoryQuery extends QueryEntity<JobCategoryState> {
  constructor(protected store: JobCategoryStore) {
    super(store);
  }
}