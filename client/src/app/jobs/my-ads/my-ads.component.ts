import { Component } from '@angular/core';
import { Observable, map } from 'rxjs';
import { AdsPaginationParameters } from 'src/app/models/filterCriteria';
import { PagedResponse } from 'src/app/models/pagedResponse';
import { JobCategory, UserJobPost } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { UtilityService } from 'src/app/services/utility.service';
import { FiltersQuery } from 'src/app/store/filters/filters.query';
import { FiltersStore } from 'src/app/store/filters/filters.store';
//import { getCategoryName, getEnumName, getEnumValue, getFormattedDate } from '../helpers/helpers';
import { DatePipe } from '@angular/common';
import { JobCategoryQuery } from 'src/app/store/jobsHelpers/job-category.query';
import { AdvertisementTypeEnum } from 'src/app/models/enums';

@Component({
  selector: 'app-my-ads',
  templateUrl: './my-ads.component.html',
  styleUrls: ['./my-ads.component.css']
})
export class MyAdsComponent {
  allJobs$: Observable<UserJobPost[]>;
  filters = this.filtersQuery.getAll();
  paginationResponse: PagedResponse<UserJobPost>;
  paginationParameters: AdsPaginationParameters;
  jobCategories: JobCategory[];
  jobCategories$ = this.jobCategoryQuery.selectAll();

  constructor(private jobService: JobService, utility: UtilityService, private filtersQuery: FiltersQuery,
    private filtersStore: FiltersStore, private datePipe: DatePipe, private jobCategoryQuery: JobCategoryQuery
  ) {
    utility.setTitle('Moje objave');
  }

  ngOnInit(): void {
    this.jobCategories$.subscribe((jobCategories) => {
      this.jobCategories = jobCategories;
    })
    if (this.filters && this.filters.length > 0) {
      this.fetchPaginatedItems(this.filters[0]);
    }
    else {
      this.fetchPaginatedItems();
    }
  }

  fetchPaginatedItems(filterCriteria?: AdsPaginationParameters): void {
    if (filterCriteria) {
      this.paginationParameters = filterCriteria;
      this.filtersStore.set([this.paginationParameters]);
    }
    else {
      this.paginationParameters = {
        pageNumber: 1,
        pageSize: 10,
        orderBy: "",
        //advertisementTypeId: this.adType
      };
    }
    this.allJobs$ = this.jobService.getMyAds(this.paginationParameters).pipe(
      map(response => {
        this.paginationResponse = response;
        console.log("RESPONESEEE", response)
        return response.items;
      })
    );
  }

  getCategoryName(jobCategories: JobCategory[], jobCategoryId: number): string | undefined {
    return jobCategories?.find(r => r.id === jobCategoryId)?.name;
  }
  getEnumValue(name: string): AdvertisementTypeEnum {
    return AdvertisementTypeEnum[name as keyof typeof AdvertisementTypeEnum];
  }

  getEnumName(value: number): string {
    return value === AdvertisementTypeEnum.JobAd ? "Posao" : "Servis";
  }

  getFormattedDate(datePipe: DatePipe, date: Date): string {
    return this.datePipe.transform(date, 'dd.MM.yyyy');
  }
}
