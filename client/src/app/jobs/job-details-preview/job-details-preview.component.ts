import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subscription, map, switchMap } from 'rxjs';
import { AdvertisementTypeEnum } from 'src/app/models/enums';
import { AdsPaginationParameters } from 'src/app/models/filterCriteria';
import { UserJobPost } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
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
  prevNextButtonVisible: boolean = true;

  constructor(private jobService: JobService, utility: UtilityService, private route: ActivatedRoute,
    private datePipe: DatePipe, private cdr: ChangeDetectorRef, private adsQuery: AdsQuery, 
    private filtersStore: FiltersStore,
    private filtersQuery: FiltersQuery,
    private router: Router, 
    private adsStore: AdsStore) {
    utility.setTitle('Detalji oglasa');
  }

  ngOnInit(): void {

    this.subscription = this.route.params
      .pipe(
        map(params => params['id']),
        switchMap(userId => this.jobService.getJobById(userId))
      )
      .subscribe({ next: (response) => {
        console.log("response " + JSON.stringify(response));
         this.job = response;
         this.cdr.detectChanges();
       }, error: (errorResponse) => {
         console.log('Error fetching job', errorResponse);
       }});

       if(this.allJobs.length <= 0){
        this.prevNextButtonVisible = false;
       }
     }

     getEnumName(value: number): string {
      return value == AdvertisementTypeEnum.JobAd ? "Posao" : "Servis";
    }
  
    getFormattedDate(date: Date): string {
      return this.datePipe.transform(date, 'dd.MM.yyyy');
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
