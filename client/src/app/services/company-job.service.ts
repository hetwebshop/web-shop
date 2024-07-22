import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpService } from './http.service';
import { Observable, map, of, tap } from 'rxjs';
import { AdvertisementTypeEnum } from '../models/enums';
import { AdsPaginationParameters } from '../models/filterCriteria';
import { PagedResponse } from '../models/pagedResponse';
import { DatePipe } from '@angular/common';
import { setLoading } from '@datorama/akita';
import { CompanyJobPost } from '../models/companyJobAd';

@Injectable({
  providedIn: 'root',
})
export class CompanyJobService {
  private allJobs: CompanyJobPost[];
  private companyAds: CompanyJobPost[];
  totalItems: number;
  baseUrl = environment.apiUrl + 'companyjob/';

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

    return this.http.get<PagedResponse<CompanyJobPost>>(`${this.baseUrl}ads`, { params: queryParams }).pipe(
      map(response => {
        this.allJobs = response.items;
        //this.adsStore.set(response.items);
        this.totalItems = response.totalCount;
        //setLoading(this.adsStore);
        return response;
      })
    );
}


  getCompanyAds(adsFilter?: AdsPaginationParameters) {
    const queryParams: any = {
      pageNumber: adsFilter?.pageNumber ?? 1,
      pageSize: adsFilter?.pageSize ?? 10,
      orderBy: adsFilter?.orderBy ?? "",
      adStatus: adsFilter?.adStatus ?? null,
      //advertisementTypeId: adsFilter?.advertisementTypeId,
      // fromDate: this.datePipe.transform(adsFilter?.fromDate, 'yyyy-MM-dd') ?? "",
      // toDate: this.datePipe.transform(adsFilter?.toDate, 'yyyy-MM-dd') ?? ""
    };
    return this.http.get<PagedResponse<CompanyJobPost>>(this.baseUrl + "company-ads", { params: queryParams }).pipe(
      tap(jobs => this.companyAds = jobs.items)
    );
  }

  getJobById(id: number): Observable<CompanyJobPost> {
    // var entity = this.adsQuery.getEntity(id);
    // if (entity) {
    //   return of(entity);
    // } else {
    //   return this.http.get<UserJobPost>(`${this.baseUrl}user-job/${id}`).pipe(
    //     tap((response) => {
    //       this.adsStore.add(response);
    //       this.adsStore.setActive(response.id);
    //       setLoading(this.adsStore);

    //       return response;
    //     })
    //   );
    // }
    return this.http.get<CompanyJobPost>(`${this.baseUrl}company-job/${id}`).pipe(
        tap((response) => {
        //   this.adsStore.add(response);
        //   this.adsStore.setActive(response.id);
        //   setLoading(this.adsStore);

          return response;
        })
      );
  }

  getCompanyJobById(id: number): Observable<CompanyJobPost> {
    // var entity = this.adsQuery.getEntity(id);
    // if (entity) {
    //   return of(entity);
    // } else {
    //   return this.http.get<UserJobPost>(`${this.baseUrl}my-ad/${id}`).pipe(
    //     tap((response) => {
    //       this.adsStore.add(response);
    //       this.adsStore.setActive(response.id);
    //       setLoading(this.adsStore);

    //       return response;
    //     })
    //   );
    // }
    return this.http.get<CompanyJobPost>(`${this.baseUrl}company-my-ad/${id}`).pipe(
        tap((response) => {
        //   this.adsStore.add(response);
        //   this.adsStore.setActive(response.id);
        //   setLoading(this.adsStore);

          return response;
        })
      );
  }

  upsertJob(isEditMode: boolean, jobData: CompanyJobPost): Observable<CompanyJobPost> {
    if (isEditMode) {
      return this.http.put<CompanyJobPost>(`${this.baseUrl}update/${jobData.id}`, jobData).pipe(
        tap((response) => {
        //   this.adsStore.update(response.id, response);
        //   this.adsStore.setActive(response.id);
        //   setLoading(this.adsStore);

          return response;
        })
      );
    } else {
      return this.http.post<CompanyJobPost>(`${this.baseUrl}create`, jobData).pipe(
        tap((response) => {
        //   this.adsStore.add(response);
        //   this.adsStore.setActive(response.id);
        //   setLoading(this.adsStore);

          return response;
        })
      );
    }
  }

  deleteAd(jobPostId: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.baseUrl}delete/${jobPostId}`);
  }

  closeAd(jobPostId: number): Observable<boolean> {
    return this.http.patch<boolean>(`${this.baseUrl}close/${jobPostId}`);
  }

  reactivateAd(jobPostId: number): Observable<boolean> {
    return this.http.patch<boolean>(`${this.baseUrl}reactivate/${jobPostId}`);
  }
}