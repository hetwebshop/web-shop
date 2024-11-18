import { Component, Inject, OnDestroy } from '@angular/core';
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
import { Observable, Subject, map, switchMap, take, takeUntil, tap } from 'rxjs';
import { AdsPaginationParameters } from 'src/app/models/filterCriteria';
import { PagedResponse } from 'src/app/models/pagedResponse';
import { FiltersStore } from 'src/app/store/filters/filters.store';
import { FiltersQuery } from 'src/app/store/filters/filters.query';
import { JobCategoryQuery } from 'src/app/store/jobsHelpers/job-category.query';
import { CompanyJobPost } from 'src/app/models/companyJobAd';
import { CompanyJobService } from 'src/app/services/company-job.service';
import { JobTypeQuery } from 'src/app/store/jobsHelpers/job-type.query';
import { SubmitApplicationModalComponent } from 'src/app/modal/submit-application-modal/submit-application-modal.component';
import { BreakpointObserver } from '@angular/cdk/layout';
import { LocationService } from 'src/app/services/location.service';
import { City } from 'src/app/models/location';
import { PricingPlanService } from 'src/app/services/pricingPlan.service';
import { PricingPlan } from 'src/app/models/pricingPlan';

@Component({
  selector: 'app-company-job-ads',
  templateUrl: './company-job-ads.component.html',
  styleUrls: ['./company-job-ads.component.css']
})
export class CompanyJobAdsComponent implements OnDestroy {
  private destroy$ = new Subject<void>();
  allJobs$: Observable<CompanyJobPost[]>;
  adType: AdvertisementTypeEnum;
  user: User;
  paginationResponse: PagedResponse<CompanyJobPost>;
  paginationParameters: AdsPaginationParameters;
  filters = [];
  showFilters: boolean = false;
  isGridView: boolean = true;
  isGridViewUserSelection: boolean = this.isGridView;
  isLargeScreen = true;
  
  constructor(private jobService: JobService, private companyJobService: CompanyJobService, utility: UtilityService, private route: ActivatedRoute,
    private router: Router, private datePipe: DatePipe, public dialog: MatDialog,
    private accountService: AccountService, private filtersStore: FiltersStore, private breakpointObserver: BreakpointObserver,
    private filtersQuery: FiltersQuery, private jobCategoryQuery: JobCategoryQuery, private pricingPlanService: PricingPlanService, private locationService: LocationService, private jobTypeQuery: JobTypeQuery) {
    utility.setTitle('Oglasi kompanije');
    this.accountService.user$.pipe(takeUntil(this.destroy$)).subscribe((u) => (this.user = u));
  }

  toggleView() {
    this.isGridView = !this.isGridView;
    this.isGridViewUserSelection = this.isGridView;
  }

  ngOnInit(): void {
    this.breakpointObserver.observe([
      "(min-width: 768px)"
    ]).pipe(takeUntil(this.destroy$)).subscribe(result => {
      if(result.matches){
        this.isLargeScreen = true;
        this.isGridView = this.isGridViewUserSelection;
      }
      
      else 
        {this.isLargeScreen = false;
        this.isGridView = false;}
    });

    if(this.filters && this.filters.length > 0){
      this.fetchPaginatedItems(this.filters[0]);
    }
    else {
      this.fetchPaginatedItems();
    }
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

  openSubmitApplicationModal(toEmail: string, jobPosition: string) {
    console.log(toEmail);
    const fromEmail = this.user.email ?? '';
    const dialogRef = this.dialog.open(SubmitApplicationModalComponent, {
      width: '800px',
      data: { fromEmail, toEmail, jobPosition }
    });

    dialogRef.afterClosed().pipe(takeUntil(this.destroy$)).subscribe(result => {
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

  toggleFilters() {
    this.showFilters = !this.showFilters;
  }

  
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}