import { Component } from '@angular/core';
import { Observable, first, map } from 'rxjs';
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
import { AdvertisementTypeEnum, Gender, JobPostStatus } from 'src/app/models/enums';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { City } from 'src/app/models/location';
import { LocationService } from 'src/app/services/location.service';

@Component({
  selector: 'app-my-ads',
  templateUrl: './my-ads.component.html',
  styleUrls: ['./my-ads.component.css']
})
export class MyAdsComponent {
  activeJobs: UserJobPost[];
  closedJobs: UserJobPost[];
  deletedJobs: UserJobPost[];
  activeAdsPaginationResponse: PagedResponse<UserJobPost>;
  closedAdsPaginationResponse: PagedResponse<UserJobPost>;
  deletedAdsPaginationResponse: PagedResponse<UserJobPost>;
  paginationParameters: AdsPaginationParameters;
  activeTabJobStatus: JobPostStatus = JobPostStatus.Active;

  
  constructor(private jobService: JobService, private locationService: LocationService, utility: UtilityService,
    private datePipe: DatePipe, private jobCategoryQuery: JobCategoryQuery
  ) {
    utility.setTitle('Moje objave');
  }

  ngOnInit(): void {
    this.fetchPaginatedItems();
  }
  

  fetchPaginatedItems(filterCriteria?: AdsPaginationParameters, isPaginationChangedByUserEvent: boolean = false): void {
    if (filterCriteria) {
      this.paginationParameters = filterCriteria;
    }
    else {
      this.paginationParameters = {
        pageNumber: 1,
        pageSize: 10,
        orderBy: "",
      };
    }
    if(isPaginationChangedByUserEvent){
      this.fetchMyAds({...this.paginationParameters, adStatus: this.activeTabJobStatus});
    }
    else {
      this.fetchMyAds({...this.paginationParameters, adStatus: JobPostStatus.Active});
      this.fetchMyAds({...this.paginationParameters, adStatus: JobPostStatus.Closed});
      this.fetchMyAds({...this.paginationParameters, adStatus: JobPostStatus.Deleted});
    }
  }

  fetchMyAds(params: AdsPaginationParameters) {
    this.jobService.getMyAds(params).subscribe(
      (response) => {
        if(params.adStatus == JobPostStatus.Active){
          this.activeAdsPaginationResponse = response;
          this.activeJobs = response.items;
        }
        else if(params.adStatus == JobPostStatus.Closed){
          this.closedAdsPaginationResponse = response;
          this.closedJobs = response.items;
        }
        else if(params.adStatus == JobPostStatus.Deleted){
          this.deletedAdsPaginationResponse = response;
          this.deletedJobs = response.items;
        }
      },
      (error) => {
        console.error('Error fetching jobs:', error);
      }
    );
  }

  onPageChange(pageNumber: number) {
    this.paginationParameters = { ...this.paginationParameters, pageNumber: pageNumber };
    this.fetchPaginatedItems(this.paginationParameters, true);
  }

  onPageSizeChange(pageSize: number) {
    this.paginationParameters = { ...this.paginationParameters, pageNumber: 1, pageSize: pageSize };
    this.fetchPaginatedItems(this.paginationParameters, true);
  }

  onTabChanged(event: MatTabChangeEvent) {
    const selectedIndex = event.index; //0-aktivni oglasi tab, 1-zatvoreni oglasi tab
    if (selectedIndex === 0) {
      this.activeTabJobStatus = JobPostStatus.Active;
    } else if (selectedIndex === 1) {
      this.activeTabJobStatus = JobPostStatus.Closed;
    }
    else if (selectedIndex === 2) {
      this.activeTabJobStatus = JobPostStatus.Deleted;
    }
    this.resetAdsResponses();
    this.paginationParameters = { ...this.paginationParameters, pageSize: 10, pageNumber: 1, adStatus: this.activeTabJobStatus }
    this.fetchPaginatedItems(this.paginationParameters, true);
  }

  resetAdsResponses() {
    this.activeAdsPaginationResponse = null;
    this.closedAdsPaginationResponse = null;
    this.deletedAdsPaginationResponse = null;
  }

  deleteAd(jobId: number): void {
    this.jobService.deleteAd(jobId).subscribe((response) => {
      if(response == true)
        this.fetchPaginatedItems(this.paginationParameters);
    });
  }
}
