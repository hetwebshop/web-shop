import { ChangeDetectorRef, Component, ElementRef, Inject, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdvertisementTypeEnum, Gender } from 'src/app/models/enums';
import { JobCategory, JobType, UserJobPost } from 'src/app/models/userJobPost';
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
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatDrawer } from '@angular/material/sidenav';
import { HttpClient } from '@angular/common/http';
import { City } from 'src/app/models/location';
import { LocationService } from 'src/app/services/location.service';
@Component({
  selector: 'app-user-jobs',
  templateUrl: './user-jobs-list.component.html',
  styleUrls: ['./user-jobs-list.component.css']
})
export class UserJobsListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  allJobs$: Observable<UserJobPost[]>;
  adType: AdvertisementTypeEnum;
  user: User;
  paginationResponse: PagedResponse<UserJobPost>;
  paginationParameters: AdsPaginationParameters;
  filters = this.filtersQuery.getAll();
  isLargeScreen = true;
  isJobAd: boolean;
  selectedFilePath: string | null = null;
  @ViewChild('filePreviewModal') filePreviewModal!: TemplateRef<any>;
  fileUrl: string = ""
  showFilters: boolean = false;
  isGridView: boolean = true;
  isGridViewUserSelection: boolean = this.isGridView;

  hasAnyDataToShow = true;

  constructor(private cdr: ChangeDetectorRef, private jobService: JobService, utility: UtilityService, private route: ActivatedRoute,
    private router: Router, private datePipe: DatePipe, public dialog: MatDialog,
    private accountService: AccountService, private filtersStore: FiltersStore,
    private filtersQuery: FiltersQuery, private http: HttpClient, private locationService: LocationService, private jobCategoryQuery: JobCategoryQuery, private breakpointObserver: BreakpointObserver) {
    utility.setTitle('Oglasi');
    this.accountService.user$.pipe(takeUntil(this.destroy$)).subscribe((u) => (this.user = u));
  }

  genderName(gender: Gender) {
    if(gender == Gender.Male){
      return "Muškarac";
    }
    else if(gender == Gender.Female){
      return "Žena";
    }
    else 
      return "Ostali";
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
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(params => {
      if (params['type']) {
        this.adType = this.getEnumValue(params['type']);
        if(this.adType != AdvertisementTypeEnum.JobAd && this.adType != AdvertisementTypeEnum.Service){
          this.router.navigateByUrl('/not-found');
        }
        if(this.adType == AdvertisementTypeEnum.JobAd)
          this.isJobAd = true;
        else
          this.isJobAd = false;
        if(this.filters && this.filters.length > 0){
          this.fetchPaginatedItems(this.filters[0]);
        }
        else {
          this.fetchPaginatedItems();
        }
      }
    });
  }

  toggleFilters() {
    this.showFilters = !this.showFilters;
  }

  getEnumValue(name: string): AdvertisementTypeEnum {
    return AdvertisementTypeEnum[name as keyof typeof AdvertisementTypeEnum];
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
    this.allJobs$ = this.jobService.getAds(this.paginationParameters).pipe(
      map(response => {
        this.paginationResponse = response;
        if(this.paginationParameters.pageNumber == 1 && (response.items == null || response.items.length == 0))
          this.hasAnyDataToShow = false;
        return response.items;
      })
    );
  }

  @ViewChild('itemListContainer') itemListContainer!: ElementRef;
    onPageChange(pageNumber: number) {
    this.paginationParameters = { ...this.paginationParameters, pageNumber: pageNumber };
    this.fetchPaginatedItems(this.paginationParameters);
    this.cdr.detectChanges();
    console.log("TE22");
    this.itemListContainer.nativeElement.scrollIntoView({ behavior: 'smooth' });
  }

  onPageSizeChange(pageSize: number) {
    this.paginationParameters = { ...this.paginationParameters, pageSize: pageSize, pageNumber: 1 };
    this.fetchPaginatedItems(this.paginationParameters);
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
