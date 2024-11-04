import { ChangeDetectorRef, Component, ElementRef, Inject, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdvertisementTypeEnum, Gender } from 'src/app/models/enums';
import { JobCategory, UserJobPost } from 'src/app/models/userJobPost';
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
export class UserJobsListComponent {
  allJobs$: Observable<UserJobPost[]>;
  adType: AdvertisementTypeEnum;
  user: User;
  paginationResponse: PagedResponse<UserJobPost>;
  paginationParameters: AdsPaginationParameters;
  filters = this.filtersQuery.getAll();
  jobCategories: JobCategory[];
  isLargeScreen = true;
  isJobAd: boolean;
  selectedFilePath: string | null = null;
  @ViewChild('filePreviewModal') filePreviewModal!: TemplateRef<any>;
  fileUrl: string = ""
  showFilters: boolean = false;
  isGridView: boolean = true;
  isGridViewUserSelection: boolean = this.isGridView;
  cities: City[];

  constructor(private cdr: ChangeDetectorRef, private jobService: JobService, utility: UtilityService, private route: ActivatedRoute,
    private router: Router, private datePipe: DatePipe, public dialog: MatDialog,
    private accountService: AccountService, private filtersStore: FiltersStore,
    private filtersQuery: FiltersQuery, private http: HttpClient, private locationService: LocationService, private jobCategoryQuery: JobCategoryQuery, private breakpointObserver: BreakpointObserver) {
    utility.setTitle('Oglasi');
    this.accountService.user$.subscribe((u) => (this.user = u));
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
    this.loadCities();
    this.loadJobCategories();
    this.breakpointObserver.observe([
      "(min-width: 768px)"
    ]).subscribe(result => {
      if(result.matches){
        this.isLargeScreen = true;
        this.isGridView = this.isGridViewUserSelection;
      }
      
      else 
        {this.isLargeScreen = false;
        this.isGridView = false;}
    });
    this.route.queryParams.subscribe(params => {
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

  loadJobCategories(): void {
    this.jobService.getJobCategories()
      .subscribe(categories => {
        this.jobCategories = categories.filter(r => r.parentId == null);
      });
  }

  getCategoryName(jobCategoryId: number): string {
    return this.jobCategories?.find(r => r.id == jobCategoryId)?.name;
  }

  getEnumValue(name: string): AdvertisementTypeEnum {
    return AdvertisementTypeEnum[name as keyof typeof AdvertisementTypeEnum];
  }

  getEnumName(value: number): string {
    return value == AdvertisementTypeEnum.JobAd ? "Posao" : "Servis";
  }

  getFormattedDate(date: Date): string {
    return this.datePipe.transform(date, 'dd.MM.yyyy');
  }

  getCityName(cityId: number) : string {
    return this.cities.find(r => r.id == cityId)?.name;
  }

  loadCities(): void {
    this.locationService.getCities()
      .subscribe(cities => {
        this.cities = cities;
      });
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
        return response.items;
      })
    );
  }

  openEmailModal(toEmail: string) {
    console.log(toEmail);
    const fromEmail = this.user.email ?? '';
    const dialogRef = this.dialog.open(EmailModalComponent, {
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

  previewUserCV(fileName: string, event: Event) {
    event.preventDefault();
    event.stopPropagation();
    this.jobService.getCVFileByName(fileName).subscribe((fileBlob) => {
      const blobUrl = URL.createObjectURL(fileBlob);
      this.selectedFilePath = blobUrl;
      this.dialog.open(this.filePreviewModal, {
        width: '80%',
        height: 'auto',
      });
    })
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
}
