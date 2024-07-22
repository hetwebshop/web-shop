import { Component } from '@angular/core';
import { Observable, map } from 'rxjs';
import { AdsPaginationParameters } from 'src/app/models/filterCriteria';
import { PagedResponse } from 'src/app/models/pagedResponse';
import { JobCategory, JobType, UserJobPost } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { UtilityService } from 'src/app/services/utility.service';
import { FiltersQuery } from 'src/app/store/filters/filters.query';
import { FiltersStore } from 'src/app/store/filters/filters.store';
//import { getCategoryName, getEnumName, getEnumValue, getFormattedDate } from '../helpers/helpers';
import { DatePipe } from '@angular/common';
import { JobCategoryQuery } from 'src/app/store/jobsHelpers/job-category.query';
import { AdvertisementTypeEnum, JobPostStatus } from 'src/app/models/enums';
import { CompanyJobPost } from 'src/app/models/companyJobAd';
import { CompanyJobService } from 'src/app/services/company-job.service';
import { JobTypeQuery } from 'src/app/store/jobsHelpers/job-type.query';

@Component({
  selector: 'app-company-my-ads',
  templateUrl: './company-my-ads.component.html',
  styleUrls: ['./company-my-ads.component.css']
})
export class CompanyMyAdsComponent {
  allJobs$: Observable<CompanyJobPost[]>;
  completedJobs$: Observable<CompanyJobPost[]>;
  filters = this.filtersQuery.getAll();
  paginationResponse: PagedResponse<CompanyJobPost>;
  paginationParameters: AdsPaginationParameters;
  jobCategories: JobCategory[];
  jobCategories$ = this.jobCategoryQuery.selectAll();
  jobTypes: JobType[];
  jobTypes$ = this.jobTypeQuery.selectAll();
  selectedTabIndex = 0;//AKTIVNI OGLASI TAB

  constructor(private jobService: JobService, private companyJobService: CompanyJobService, utility: UtilityService, private filtersQuery: FiltersQuery,
    private filtersStore: FiltersStore, private datePipe: DatePipe, private jobCategoryQuery: JobCategoryQuery,
    private jobTypeQuery: JobTypeQuery
  ) {
    utility.setTitle('Objave kompanije');
  }

  ngOnInit(): void {
    this.jobCategories$.subscribe((jobCategories) => {
      this.jobCategories = jobCategories;
    });
    this.jobTypes$.subscribe((jobTypes) => {
      this.jobTypes = jobTypes;
    });
    if (this.filters && this.filters.length > 0) {
      this.fetchPaginatedItems(this.selectedTabIndex, this.filters[0]);
    }
    else {
      this.fetchPaginatedItems(this.selectedTabIndex);
    }
  }

  fetchPaginatedItems(selectedTab: number, filterCriteria?: AdsPaginationParameters): void {
    if (filterCriteria) {
      this.paginationParameters = filterCriteria;
      this.filtersStore.set([this.paginationParameters]);
    }
    else {
      this.paginationParameters = {
        pageNumber: 1,
        pageSize: 10,
        orderBy: "",
      };
    }
    if(selectedTab == 0)//Aktivni oglasi
      this.allJobs$ = this.companyJobService.getCompanyAds({...this.paginationParameters, adStatus: JobPostStatus.Active}).pipe(
        map(response => {
          this.paginationResponse = response;
          return response.items;
        })
      );
    else 
      this.completedJobs$ = this.companyJobService.getCompanyAds({...this.paginationParameters, adStatus: JobPostStatus.Closed}).pipe(
        map(response => {
          this.paginationResponse = response;
          return response.items;
        })
      );
  }

  onTabChange(index: number): void {
    this.selectedTabIndex = index;
    this.fetchPaginatedItems(this.selectedTabIndex);
  }

  getCategoryName(jobCategoryId: number): string | undefined {
    console.log("JOBCATEGORY", jobCategoryId);
    return this.jobCategories?.find(r => r.id === jobCategoryId)?.name;
  }
  getJobType(jobTypeId: number): string {
    return this.jobTypes?.find(r => r.id === jobTypeId)?.name;
  }

  getStatusEnumValue(value: number): string {
    return value == JobPostStatus.Active ? "Aktivan Oglas" : value == JobPostStatus.Closed ? "Istekao Oglas" : "Obrisan Oglas";
  }
  
  getFormattedDate(datePipe: DatePipe, date: Date): string {
    return this.datePipe.transform(date, 'dd.MM.yyyy');
  }

  onPageChange(pageNumber: number) {
    this.paginationParameters = { ...this.paginationParameters, pageNumber: pageNumber };
    this.fetchPaginatedItems(this.selectedTabIndex, this.paginationParameters);
  }

  onPageSizeChange(pageSize: number) {
    this.paginationParameters = { ...this.paginationParameters, pageSize: pageSize };
    this.fetchPaginatedItems(this.selectedTabIndex, this.paginationParameters);
  }

  deleteAd(event, jobId: number): void {
    event.preventDefault();
    this.companyJobService.deleteAd(jobId).subscribe((response) => {
      if(response == true) {
        this.fetchPaginatedItems(this.selectedTabIndex);
      }
    });
  }

  closeAd(event, jobId: number): void {
    event.preventDefault();
    this.companyJobService.closeAd(jobId).subscribe((response) => {
      if(response == true) {
        this.fetchPaginatedItems(this.selectedTabIndex);
      }
    });
  }

  reactivateAd(event, jobId): void {
    event.preventDefault();
    this.companyJobService.reactivateAd(jobId).subscribe((response) => {
      if(response == true) {
        this.fetchPaginatedItems(this.selectedTabIndex);
      }
    });
  }
}
