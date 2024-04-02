import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, map, switchMap } from 'rxjs';
import { AdvertisementTypeEnum } from 'src/app/models/enums';
import { UserJobPost } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-job-details-preview',
  templateUrl: './job-details-preview.component.html',
  styleUrls: ['./job-details-preview.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class JobDetailsPreviewComponent implements OnInit, OnDestroy {
  job: UserJobPost;
  private subscription: Subscription;
  
  constructor(private jobService: JobService, utility: UtilityService, private route: ActivatedRoute,
    private datePipe: DatePipe, private cdr: ChangeDetectorRef) {
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
     }
     getEnumName(value: number): string {
      return value == AdvertisementTypeEnum.JobAd ? "Posao" : "Servis";
    }
  
    getFormattedDate(date: Date): string {
      return this.datePipe.transform(date, 'dd.MM.yyyy');
    }
  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
