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
import { MatTabChangeEvent } from '@angular/material/tabs';
import { LocationService } from 'src/app/services/location.service';
import { City } from 'src/app/models/location';
import { ConfirmationModalComponent } from 'src/app/modal/confirmation-modal/confirmation-modal.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-company-my-ads',
  templateUrl: './company-my-ads.component.html',
  styleUrls: ['./company-my-ads.component.css']
})
export class CompanyMyAdsComponent {
  paginationResponse: PagedResponse<CompanyJobPost>;
  paginationParameters: AdsPaginationParameters;
  activeTabJobStatus: JobPostStatus = JobPostStatus.Active;
  activeJobs: CompanyJobPost[];
  closedJobs: CompanyJobPost[];
  deletedJobs: CompanyJobPost[];
  activeAdsPaginationResponse: PagedResponse<CompanyJobPost>;
  closedAdsPaginationResponse: PagedResponse<CompanyJobPost>;
  deletedAdsPaginationResponse: PagedResponse<CompanyJobPost>;

  constructor(private jobService: JobService, private companyJobService: CompanyJobService, private locationService: LocationService, utility: UtilityService, private datePipe: DatePipe, private jobCategoryQuery: JobCategoryQuery,
    private jobTypeQuery: JobTypeQuery, private dialog: MatDialog
  ) {
    utility.setTitle('Objave kompanije');
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
    if (isPaginationChangedByUserEvent) {
      this.fetchCompanyAds({ ...this.paginationParameters, adStatus: this.activeTabJobStatus });
    }
    else {
      this.fetchCompanyAds({ ...this.paginationParameters, adStatus: JobPostStatus.Active });
      this.fetchCompanyAds({ ...this.paginationParameters, adStatus: JobPostStatus.Closed });
      this.fetchCompanyAds({ ...this.paginationParameters, adStatus: JobPostStatus.Deleted });
    }
  }

  fetchCompanyAds(params: AdsPaginationParameters) {
    this.companyJobService.getCompanyAds(params).subscribe(
      (response) => {
        if (params.adStatus == JobPostStatus.Active) {
          this.activeAdsPaginationResponse = response;
          this.activeJobs = response.items;
        }
        else if (params.adStatus == JobPostStatus.Closed) {
          this.closedAdsPaginationResponse = response;
          this.closedJobs = response.items;
        }
        else if (params.adStatus == JobPostStatus.Deleted) {
          this.deletedAdsPaginationResponse = response;
          this.deletedJobs = response.items;
        }
      },
      (error) => {
        console.error('Error fetching jobs:', error);
      }
    );
  }

  onTabChange(event: MatTabChangeEvent): void {
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

  onPageChange(pageNumber: number) {
    this.paginationParameters = { ...this.paginationParameters, pageNumber: pageNumber };
    this.fetchPaginatedItems(this.paginationParameters, true);
  }

  onPageSizeChange(pageSize: number) {
    this.paginationParameters = { ...this.paginationParameters, pageNumber: 1, pageSize: pageSize };
    this.fetchPaginatedItems(this.paginationParameters, true);
  }

  deleteAd(event, jobId: number): void {
    event.preventDefault();
    const confirmationDialogRef = this.dialog.open(ConfirmationModalComponent,
      {
        data: {
          title: "Obriši oglas",
          message: "Da li ste sigurni da želite obrisati objavu? Obrisane objave možete naći poslije u sekciji 'Obrisani oglasi' te se obrisani oglas više ne može aktivirati!"
        }
      });
      confirmationDialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.companyJobService.deleteAd(jobId).subscribe((response) => {
          if (response == true)
            this.fetchPaginatedItems(this.paginationParameters);
        });
      }
    });
  }

  closeAd(event, jobId: number): void {
    event.preventDefault();
    const confirmationDialogRef = this.dialog.open(ConfirmationModalComponent,
      {
        data: {
          title: "Obriši oglas",
          message: "Da li ste sigurni da želite obrisati objavu? Obrisane objave možete naći poslije u sekciji 'Obrisani oglasi' te se obrisani oglas više ne može aktivirati!"
        }
      });
      confirmationDialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.companyJobService.closeAd(jobId).subscribe((response) => {
          if (response == true) {
            this.fetchPaginatedItems();
          }
        });
      }
    });
  }

  reactivateAd(event, jobId): void {
    event.preventDefault();
    const confirmationDialogRef = this.dialog.open(ConfirmationModalComponent,
      {
        data: {
          title: "Obriši oglas",
          message: "Da li ste sigurni da želite obrisati objavu? Obrisane objave možete naći poslije u sekciji 'Obrisani oglasi' te se obrisani oglas više ne može aktivirati!"
        }
      });
      confirmationDialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.companyJobService.reactivateAd(jobId).subscribe((response) => {
          if (response == true) {
            this.fetchPaginatedItems();
          }
        });
      }
    });
  }
}
