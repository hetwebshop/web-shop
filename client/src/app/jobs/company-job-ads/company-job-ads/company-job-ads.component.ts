import { Component, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdvertisementTypeEnum } from 'src/app/models/enums';
import { JobCategory, JobType } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { UtilityService } from 'src/app/services/utility.service';
import { DatePipe } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { EmailModalComponent } from 'src/app/modal/email-modal/email-modal.component';
import { User } from 'src/app/modal/user';
import { AccountService } from 'src/app/services/account.service';
import { Observable, map, switchMap, take, tap } from 'rxjs';
import { AdsPaginationParameters } from 'src/app/models/filterCriteria';
import { PagedResponse } from 'src/app/models/pagedResponse';
import { FiltersStore } from 'src/app/store/filters/filters.store';
import { FiltersQuery } from 'src/app/store/filters/filters.query';
import { JobCategoryQuery } from 'src/app/store/jobsHelpers/job-category.query';
import { CompanyJobPost } from 'src/app/models/companyJobAd';
import { CompanyJobService } from 'src/app/services/company-job.service';
import { JobTypeQuery } from 'src/app/store/jobsHelpers/job-type.query';
import { SubmitApplicationModalComponent } from 'src/app/modal/submit-application-modal/submit-application-modal.component';

@Component({
  selector: 'app-company-job-ads',
  templateUrl: './company-job-ads.component.html',
  styleUrls: ['./company-job-ads.component.css']
})
export class CompanyJobAdsComponent {
  allJobs$: Observable<CompanyJobPost[]>;
  adType: AdvertisementTypeEnum;
  user: User;
  paginationResponse: PagedResponse<CompanyJobPost>;
  paginationParameters: AdsPaginationParameters;
  filters = this.filtersQuery.getAll();
  jobCategories: JobCategory[];
  jobCategories$ = this.jobCategoryQuery.selectAll();
  jobTypes: JobType[];
  jobTypes$ = this.jobTypeQuery.selectAll();

  constructor(private jobService: JobService, private companyJobService: CompanyJobService, utility: UtilityService, private route: ActivatedRoute,
    private router: Router, private datePipe: DatePipe, public dialog: MatDialog,
    private accountService: AccountService, private filtersStore: FiltersStore,
    private filtersQuery: FiltersQuery, private jobCategoryQuery: JobCategoryQuery, private jobTypeQuery: JobTypeQuery) {
    utility.setTitle('Oglasi kompanije');
    this.accountService.user$.subscribe((u) => (this.user = u));
  }

  ngOnInit(): void {
    this.jobCategories$.subscribe((jobCategories) => {
      this.jobCategories = jobCategories;
    });
    this.jobTypes$.subscribe((jobTypes) => {
      this.jobTypes = jobTypes;
    });
    if(this.filters && this.filters.length > 0){
      this.fetchPaginatedItems(this.filters[0]);
    }
    else {
      this.fetchPaginatedItems();
    }
  }

  getCategoryName(jobCategoryId: number): string {
    return this.jobCategories?.find(r => r.id == jobCategoryId)?.name;
  }

  getJobType(jobTypeId: number): string {
    return this.jobTypes?.find(r => r.id == jobTypeId)?.name;
  }

  getFormattedDate(date: Date): string {
    return this.datePipe.transform(date, 'dd.MM.yyyy');
  }

  fetchPaginatedItems(filterCriteria?: AdsPaginationParameters): void {  
    if(filterCriteria) {
      this.paginationParameters = filterCriteria;
      this.filtersStore.set([this.paginationParameters]);
    }
    else {
      this.paginationParameters = {
        pageNumber: 1,
        pageSize: 10,
        orderBy: "",
        advertisementTypeId: this.adType
      };
    }
    this.allJobs$ = this.companyJobService.getAds(this.paginationParameters).pipe(
      map(response => {
        this.paginationResponse = response;
        return response.items;
      })
    );
  }

  openSubmitApplicationModal(toEmail: string) {
    console.log(toEmail);
    const fromEmail = this.user.email ?? '';
    const dialogRef = this.dialog.open(SubmitApplicationModalComponent, {
      width: '800px',
      data: { fromEmail, toEmail }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        // Perform cancellation action here
        console.log('Changes canceled');
      }
    });
  }

  onPageChange(pageNumber: number) {
    this.paginationParameters = { ...this.paginationParameters, pageNumber: pageNumber };
    this.fetchPaginatedItems(this.paginationParameters);
  }

  onPageSizeChange(pageSize: number) {
    this.paginationParameters = { ...this.paginationParameters, pageSize: pageSize, pageNumber: 1 };
    this.fetchPaginatedItems(this.paginationParameters);
  }
}