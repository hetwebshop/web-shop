import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpService } from './http.service';
import { JobCategory, JobType, UserJobPost } from '../models/userJobPost';
import { Observable, map, of, tap } from 'rxjs';
import { AdvertisementTypeEnum } from '../models/enums';
import { AdsPaginationParameters } from '../models/filterCriteria';
import { PagedResponse } from '../models/pagedResponse';
import { DatePipe } from '@angular/common';
import { AdsStore } from '../store/jobs/ads.store';
import { AdsQuery } from '../store/jobs/ads.query';
import { setLoading } from '@datorama/akita';
import { JobCategoryStore } from '../store/jobsHelpers/job-category.store';
import { JobCategoryQuery } from '../store/jobsHelpers/job-category.query';
import { JobTypeStore } from '../store/jobsHelpers/job-type.store';
import { JobTypeQuery } from '../store/jobsHelpers/job-type.query';

@Injectable({
  providedIn: 'root',
})
export class JobService {
  private allJobs: UserJobPost[];
  private myAds: UserJobPost[];
  totalItems: number;
  baseUrl = environment.apiUrl + 'job/';

  constructor(private http: HttpService, private datePipe: DatePipe, private adsStore: AdsStore, private adsQuery: AdsQuery,
    private jobCategoryStore: JobCategoryStore, private jobCategoryQuery: JobCategoryQuery,
    private jobTypeStore: JobTypeStore, private jobTypeQuery: JobTypeQuery) {}

  getAds(adsFilter?: AdsPaginationParameters) {
    const queryParams: any = {
      pageNumber: adsFilter?.pageNumber ?? 1,
      pageSize: adsFilter?.pageSize ?? 10,
      orderBy: adsFilter?.orderBy ?? "",
      searchKeyword: adsFilter?.searchKeyword ?? "",
      advertisementTypeId: adsFilter?.advertisementTypeId ?? 1,
      fromDate: this.datePipe.transform(adsFilter?.fromDate, 'yyyy-MM-dd') ?? "",
      toDate: this.datePipe.transform(adsFilter?.toDate, 'yyyy-MM-dd') ?? ""
    };

    if (adsFilter?.cityIds && adsFilter.cityIds.length > 0) {
      queryParams.cityIds = adsFilter.cityIds;
    }

    if (adsFilter?.jobCategoryIds && adsFilter.jobCategoryIds.length > 0) {
      queryParams.jobCategoryIds = adsFilter.jobCategoryIds;
    }

    if (adsFilter?.jobTypeIds && adsFilter.jobTypeIds.length > 0) {
      queryParams.jobTypeIds = adsFilter.jobTypeIds;
    }

    return this.http.get<PagedResponse<UserJobPost>>(`${this.baseUrl}ads`, { params: queryParams }).pipe(
      map(response => {
        this.allJobs = response.items;
        this.adsStore.set(response.items);
        this.totalItems = response.totalCount;
        setLoading(this.adsStore);
        return response;
      })
    );
}


  getMyAds() {
    return this.http.get<UserJobPost[]>(this.baseUrl + "my-ads").pipe(
      tap(jobs => this.myAds = jobs)
    );
  }

  getJobById(id: number): Observable<UserJobPost> {
    console.log("Job by id ");
    var entity = this.adsQuery.getEntity(id);
    console.log(entity);
    if (entity) {
      return of(entity);
    } else {
      console.log("http request");
      return this.http.get<UserJobPost>(`${this.baseUrl}user-job/${id}`).pipe(
        tap((response) => {
          this.adsStore.add(response);
          this.adsStore.setActive(response.id);
          setLoading(this.adsStore);

          return response;
        })
      );
    }
  }

  upsertJob(isEditMode: boolean, jobData: UserJobPost): Observable<UserJobPost> {
    if (isEditMode) {
      return this.http.put<UserJobPost>(`${this.baseUrl}update/${jobData.id}`, jobData).pipe(
        tap((response) => {
          this.adsStore.update(jobData.id, response);
          this.adsStore.setActive(response.id);
          setLoading(this.adsStore);

          return response;
        })
      );
    } else {
      return this.http.post<UserJobPost>(`${this.baseUrl}create`, jobData).pipe(
        tap((response) => {
          this.adsStore.add(response);
          this.adsStore.setActive(response.id);
          setLoading(this.adsStore);

          return response;
        })
      );;
    }
  }

  getJobTypes(): Observable<JobType[]> {
    const jobTypes = this.jobTypeQuery.getAll();
    if(jobTypes && jobTypes.length > 0){
      return of(jobTypes);
    }
    return this.http.get<JobType[]>(this.baseUrl + "types").pipe(
      tap(response => this.jobTypeStore.set(response))
    );
  }

  getJobCategories(): Observable<JobCategory[]> {
    var jobCategories = this.jobCategoryQuery.getAll();
    if(jobCategories && jobCategories.length > 0){
      return of(jobCategories);
    }
    else 
      return this.http.get<JobCategory[]>(this.baseUrl + "categories").pipe(
        tap((response) => {
          this.jobCategoryStore.set(response);
        })
      );
  }

  getAdTypes(): Observable<JobCategory[]> {
    return this.http.get<JobCategory[]>(this.baseUrl + "adtypes");
  }
}