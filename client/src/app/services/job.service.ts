import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpService } from './http.service';
import { JobCategory, JobType, UserJobPost } from '../models/userJobPost';
import { Observable, map, of, tap } from 'rxjs';
import { AdvertisementTypeEnum } from '../models/enums';
import { AdsPaginationParameters } from '../models/filterCriteria';
import { PagedResponse } from '../models/pagedResponse';
import { DatePipe } from '@angular/common';

@Injectable({
  providedIn: 'root',
})
export class JobService {
  private allJobs: UserJobPost[];
  private myAds: UserJobPost[];
  baseUrl = environment.apiUrl + 'job/';

  constructor(private http: HttpService, private datePipe: DatePipe) {}

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

    console.log("query params");
    console.log(queryParams);

    return this.http.get<PagedResponse<UserJobPost>>(`${this.baseUrl}ads`, { params: queryParams }).pipe(
      map(response => {
        this.allJobs = response.items;
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