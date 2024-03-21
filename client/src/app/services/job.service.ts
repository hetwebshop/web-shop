import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpService } from './http.service';
import { JobCategory, JobType, UserJobPost } from '../models/userJobPost';
import { Observable, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class JobService {
  private allJobs: UserJobPost[];
  private myAds: UserJobPost[];
  baseUrl = environment.apiUrl + 'job/';

  constructor(private http: HttpService) {}

  getAllUserJobs() {
    return this.http.get<UserJobPost[]>(this.baseUrl + "alljobposts").pipe(
      tap(jobs => this.allJobs = jobs)
    );
  }

  getMyAds() {
    return this.http.get<UserJobPost[]>(this.baseUrl + "my-ads").pipe(
      tap(jobs => this.myAds = jobs)
    );
  }

  getJobById(id: number): Observable<UserJobPost> {
    if (this.allJobs) {
      console.log("all jobs " + JSON.stringify(this.allJobs));
      console.log("id " + JSON.stringify(id));
      return of(this.allJobs.find(job => job.id == id));
    } else {
      return this.http.get<UserJobPost>(`${this.baseUrl}user-job/${id}`);
    }
  }

  upsertJob(isEditMode: boolean, jobData: UserJobPost): Observable<UserJobPost> {
    console.log("Job data service");
    console.log(jobData);
    if (isEditMode) {
      return this.http.put<UserJobPost>(`${this.baseUrl}update/${jobData.id}`, jobData);
    } else {
      return this.http.post<UserJobPost>(`${this.baseUrl}create`, jobData);
    }
  }

  getJobTypes(): Observable<JobType[]> {
    return this.http.get<JobType[]>(this.baseUrl + "types");
  }

  getJobCategories(): Observable<JobCategory[]> {
    return this.http.get<JobCategory[]>(this.baseUrl + "categories");
  }

  getAdTypes(): Observable<JobCategory[]> {
    return this.http.get<JobCategory[]>(this.baseUrl + "adtypes");
  }
}