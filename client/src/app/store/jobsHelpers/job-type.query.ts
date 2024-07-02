import { QueryEntity } from '@datorama/akita';
import { Injectable } from '@angular/core';
import { JobTypeState, JobTypeStore } from './job-type.store';
@Injectable({
  providedIn: 'root',
})
export class JobTypeQuery extends QueryEntity<JobTypeState> {
  constructor(protected store: JobTypeStore) {
    super(store);
  }
}