import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription, map, switchMap } from 'rxjs';
import { SubmitApplicationModalComponent } from 'src/app/modal/submit-application-modal/submit-application-modal.component';
import { User } from 'src/app/modal/user';
import { CompanyJobPost } from 'src/app/models/companyJobAd';
import { AdvertisementTypeEnum } from 'src/app/models/enums';
import { AdsPaginationParameters } from 'src/app/models/filterCriteria';
import { City } from 'src/app/models/location';
import { JobCategory, JobType, UserJobPost } from 'src/app/models/userJobPost';
import { AccountService } from 'src/app/services/account.service';
import { CompanyJobService } from 'src/app/services/company-job.service';
import { JobService } from 'src/app/services/job.service';
import { LocationService } from 'src/app/services/location.service';
import { UtilityService } from 'src/app/services/utility.service';
import { FiltersQuery } from 'src/app/store/filters/filters.query';
import { FiltersStore } from 'src/app/store/filters/filters.store';
import { AdsQuery } from 'src/app/store/jobs/ads.query';
import { AdsStore } from 'src/app/store/jobs/ads.store';
import { JobCategoryQuery } from 'src/app/store/jobsHelpers/job-category.query';
import { JobTypeQuery } from 'src/app/store/jobsHelpers/job-type.query';

@Component({
  selector: 'app-company-job-preview',
  templateUrl: './company-job-preview.component.html',
  styleUrls: ['./company-job-preview.component.css']
})
export class CompanyJobPreviewComponent implements OnInit, OnDestroy {
  job: CompanyJobPost;
  jobCategories: JobCategory[];
  jobTypes: JobType[];
  cities: City[];
  private subscription: Subscription;
  user: User;
  
  constructor(private jobService: JobService, private companyJobService: CompanyJobService, utility: UtilityService, private route: ActivatedRoute,
    private datePipe: DatePipe, private cdr: ChangeDetectorRef, private adsQuery: AdsQuery, 
    private jobCategoryQuery: JobCategoryQuery, private jobTypeQuery: JobTypeQuery,
    private filtersStore: FiltersStore,
    private filtersQuery: FiltersQuery,
    private router: Router, 
    private accountService: AccountService,
    public dialog: MatDialog,
    private locationService: LocationService,
    private adsStore: AdsStore) {
    utility.setTitle('Detalji oglasa');
    this.accountService.user$.subscribe((u) => (this.user = u));
  }

  ngOnInit(): void {
    this.loadJobTypes();
    this.loadJobCategories();
    this.loadCities();
    this.subscription = this.route.params
      .pipe(
        map(params => params['id']),
        switchMap(jobId => this.companyJobService.getJobById(jobId))
      )
      .subscribe({ next: (response) => {
        console.log("response " + JSON.stringify(response));
         this.job = response;
         this.cdr.detectChanges();
       }, error: (errorResponse) => {
         console.log('Error fetching job', errorResponse);
       }});
     }

    getFormattedDate(date: Date): string {
      return this.datePipe.transform(date, 'dd.MM.yyyy');
    }

    getCategoryName(jobCategoryId: number): string {
      return this.jobCategories?.find(r => r.id == jobCategoryId)?.name;
    }
  
    getJobType(jobTypeId: number): string {
      return this.jobTypes?.find(r => r.id == jobTypeId)?.name;
    }

    getCityName(cityId: number) : string {
      return this.cities?.find(r => r.id == cityId)?.name;
    }
  

    loadCities(): void {
      this.locationService.getCities()
        .subscribe(cities => {
          this.cities = cities;
          this.cdr.detectChanges();
        });
    }
  
    loadJobCategories(): void {
      this.jobService.getJobCategories()
        .subscribe(categories => {
          this.jobCategories = categories.filter(r => r.parentId == null);
          this.cdr.detectChanges();
        });
    }

    loadJobTypes(): void {
      this.jobService.getJobTypes()
        .subscribe(types => {
          this.jobTypes = types;
          this.cdr.detectChanges();
        });
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

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}

