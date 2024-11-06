import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription, map, switchMap } from 'rxjs';
import { EmailModalComponent } from 'src/app/modal/email-modal/email-modal.component';
import { AdvertisementTypeEnum, Gender } from 'src/app/models/enums';
import { AdsPaginationParameters } from 'src/app/models/filterCriteria';
import { City } from 'src/app/models/location';
import { JobCategory, JobType, UserJobPost } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { LocationService } from 'src/app/services/location.service';
import { UtilityService } from 'src/app/services/utility.service';
import { FiltersQuery } from 'src/app/store/filters/filters.query';
import { FiltersStore } from 'src/app/store/filters/filters.store';
import { AdsQuery } from 'src/app/store/jobs/ads.query';
import { AdsStore } from 'src/app/store/jobs/ads.store';

@Component({
  selector: 'app-job-details-preview',
  templateUrl: './job-details-preview.component.html',
  styleUrls: ['./job-details-preview.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class JobDetailsPreviewComponent implements OnInit, OnDestroy {
  job: UserJobPost;
  private subscription: Subscription;
  currentItemIndex: number = 5;
  totalItems: number = this.jobService.totalItems;
  ads = this.adsQuery.getAll().length;
  filters = this.filtersQuery.getAll();
  allJobs$: Observable<UserJobPost[]> = this.adsQuery.selectAll();
  allJobs: UserJobPost[] = this.adsQuery.getAll();
  jobCategories: JobCategory[];
  cities: City[];
  prevNextButtonVisible: boolean = true;
  jobTypes: JobType[];
  isJobAd: boolean = true;

  constructor(private jobService: JobService, utility: UtilityService, private route: ActivatedRoute,
    private datePipe: DatePipe, private cdr: ChangeDetectorRef, private adsQuery: AdsQuery, 
    private filtersStore: FiltersStore,
    private filtersQuery: FiltersQuery,
    public dialog: MatDialog,
    private router: Router, 
    private adsStore: AdsStore,
  private locationService: LocationService) {
    utility.setTitle('Detalji oglasa');
  }

  ngOnInit(): void {

    this.loadJobCategories();
    this.loadCities();
    this.loadJobTypes();
    this.subscription = this.route.params
      .pipe(
        map(params => params['id']),
        switchMap(userId => this.jobService.getJobById(userId))
      )
      .subscribe({ next: (response) => {
        console.log("response " + JSON.stringify(response));
         this.job = response;
         this.isJobAd = this.job.advertisementTypeId == AdvertisementTypeEnum.JobAd ? true : false;
         this.cdr.detectChanges();
       }, error: (errorResponse) => {
         console.log('Error fetching job', errorResponse);
       }});

       if(this.allJobs.length <= 0){
        this.prevNextButtonVisible = false;
       }
     }

     getEnumName(value: number): string {
      return value == AdvertisementTypeEnum.JobAd ? "Posao" : "Usluga";
    }

    getPricingPlanLabel(pricingPlanName: string) : string {
      return pricingPlanName == "Base" ? "Bazni" : pricingPlanName;
    }

    getCityName(cityId: number) : string {
      return this.cities?.find(r => r.id == cityId)?.name;
    }
  
  
    getFormattedDate(date: Date): string {
      return this.datePipe.transform(date, 'dd.MM.yyyy');
    }

    getCategoryName(jobCategoryId: number): string {
      return this.jobCategories?.find(r => r.id == jobCategoryId)?.name;
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
  
    getJobType(jobTypeId: number): string {
      return this.jobTypes?.find(r => r.id == jobTypeId)?.name;
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
  

    moveToPrevious(): void {
      console.log("move to previous");
      const currentIndex = this.allJobs.findIndex(job => job.id === this.job.id);
      const previousIndex = currentIndex - 1;
    
      if (previousIndex >= 0) {
        const previousItemId = this.allJobs[previousIndex].id;
        this.router.navigate(['/ads', previousItemId]);
      } else {
        let filterCriteria = { ...this.filtersQuery.getAll()[0] };
        if (Object.keys(filterCriteria).length > 0) {
          if (filterCriteria.pageNumber > 1) {
            filterCriteria.pageNumber = filterCriteria.pageNumber - 1;
            this.jobService.getAds(filterCriteria).subscribe(response => {
              this.allJobs = response.items;
              this.filtersStore.set([filterCriteria]);
              if (response.items.length > 0) {
                const lastItemId = response.items[response.items.length - 1].id;
                this.router.navigate(['/ads', lastItemId]);
              }
            });
          }
        }
        //we enter details page by typing in url
        else {
          this.prevNextButtonVisible = false;
        }
      }
    }
    
    openEmailModal(toEmail: string) {
      console.log(toEmail);
      const fromEmail = this.job.applicantEmail ?? '';
      const dialogRef = this.dialog.open(EmailModalComponent, {
        width: '800px',
        data: { fromEmail, toEmail },
      });
  
      dialogRef.afterClosed().subscribe(result => {
        if (result === true) {
          // Perform cancellation action here
          console.log('Changes canceled');
        }
      });
    }
    moveToNext(): void {
      const currentIndex = this.allJobs.findIndex(job => job.id === this.job.id);
      const nextIndex = currentIndex + 1;
    
      if (nextIndex < this.allJobs.length) {
        console.log("from if");
        const nextItemId = this.allJobs[nextIndex].id;
        this.router.navigate(['/ads', nextItemId]);
      } else {
        let filterCriteria = { ...this.filtersQuery.getAll()[0] };
        if (Object.keys(filterCriteria).length > 0) {
          filterCriteria.pageNumber = filterCriteria.pageNumber + 1;
          this.jobService.getAds(filterCriteria).subscribe(response => {
            this.allJobs = response.items;
            this.filtersStore.set([filterCriteria]);
            if (response.items.length > 0) {
              const firstItemId = response.items[0].id;
              this.router.navigate(['/ads', firstItemId]);
            }
          });
        }
        //we enter details page by typing in url
        else {
          this.prevNextButtonVisible = false;
        }
      }
    }
  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
