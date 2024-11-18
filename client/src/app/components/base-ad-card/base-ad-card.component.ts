import { DatePipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AfterContentInit, ChangeDetectorRef, Component, ContentChild, ElementRef, Input, OnDestroy, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subject, takeUntil } from 'rxjs';
import { EmailModalComponent } from 'src/app/modal/email-modal/email-modal.component';
import { SubmitApplicationModalComponent } from 'src/app/modal/submit-application-modal/submit-application-modal.component';
import { User } from 'src/app/modal/user';
import { CompanyJobPost } from 'src/app/models/companyJobAd';
import { AdvertisementTypeEnum, Gender } from 'src/app/models/enums';
import { City } from 'src/app/models/location';
import { JobCategory, JobType, UserJobPost } from 'src/app/models/userJobPost';
import { AccountService } from 'src/app/services/account.service';
import { JobService } from 'src/app/services/job.service';
import { LocationService } from 'src/app/services/location.service';

@Component({
  selector: 'app-base-ad-card',
  templateUrl: './base-ad-card.component.html',
  styleUrls: ['./base-ad-card.component.css']
})
export class BaseAdCardComponent implements OnDestroy {
  @Input() job: UserJobPost | CompanyJobPost;
  @Input() isParentMyAdsComponent?: boolean = false;

  private destroy$ = new Subject<void>();
  userJobPost?: UserJobPost;
  companyJobPost?: CompanyJobPost;
  user: User;
  isJobAd: boolean = true;
  cities: City[];
  jobTypes: JobType[];
  jobCategories: JobCategory[];
  selectedFilePath: string | null = null;
  @ViewChild('filePreviewModal') filePreviewModal!: TemplateRef<any>;

  constructor(private cdr: ChangeDetectorRef, private jobService: JobService, private route: ActivatedRoute,
    private router: Router, private datePipe: DatePipe, public dialog: MatDialog,
    private accountService: AccountService,
    private http: HttpClient, private locationService: LocationService) {
    this.accountService.user$.pipe(
      takeUntil(this.destroy$)
    ).subscribe((u) => (this.user = u));
  }

  ngOnInit(): void {
    if (this.isUserJobPost(this.job)) {
      this.userJobPost = this.job;
    } else {
      this.companyJobPost = this.job;
    }
    this.isJobAd = this.userJobPost?.advertisementTypeId == AdvertisementTypeEnum.JobAd ? true : false;
    this.loadCities();
    this.loadJobCategories();
    this.loadJobTypes();
  }

  loadJobCategories(): void {
    this.jobService.getJobCategories().pipe(
      takeUntil(this.destroy$)
    )
      .subscribe(categories => {
        this.jobCategories = categories.filter(r => r.parentId == null);
      });
  }



  loadCities(): void {
    this.locationService.getCities().pipe(
      takeUntil(this.destroy$)
    )
      .subscribe(cities => {
        this.cities = cities;
      });
  }

  loadJobTypes(): void {
    this.jobService.getJobTypes().pipe(
      takeUntil(this.destroy$)
    )
      .subscribe(types => {
        this.jobTypes = types
      });
  }

  getPricingPlanLabel(pricingPlanName: string): string {
    return pricingPlanName == "Base" ? "Osnovni paket" : pricingPlanName + ' paket';
  }

  getCityName(cityId: number): string {
    return this.cities?.find(r => r.id == cityId)?.name;
  }

  getJobType(jobTypeId: number): string {
    return this.jobTypes?.find(r => r.id == jobTypeId)?.name;
  }

  getCategoryName(jobCategoryId: number): string {
    return this.jobCategories?.find(r => r.id == jobCategoryId)?.name;
  }

  getEnumName(value: number): string {
    return value == AdvertisementTypeEnum.JobAd ? "Posao" : "Usluga";
  }

  getFormattedDate(date: Date): string {
    return this.datePipe.transform(date, 'dd.MM.yyyy');
  }

  genderName(gender: Gender) {
    if (gender == Gender.Male) {
      return "Muškarac";
    }
    else if (gender == Gender.Female) {
      return "Žena";
    }
    else
      return "Ostali";
  }

  openEmailModal(toEmail: string) {
    console.log(toEmail);
    const fromEmail = this.user.email ?? '';
    const dialogRef = this.dialog.open(EmailModalComponent, {
      width: '800px',
      data: { fromEmail, toEmail }
    });

    dialogRef.afterClosed().pipe(
      takeUntil(this.destroy$)
    ).subscribe(result => {
      if (result === true) {
        // Perform cancellation action here
        console.log('Changes canceled');
      }
    });
  }

  openSubmitApplicationModal(toEmail: string, jobPosition: string) {
    console.log(toEmail);
    const fromEmail = this.user.email ?? '';
    const dialogRef = this.dialog.open(SubmitApplicationModalComponent, {
      width: '800px',
      data: { fromEmail, toEmail, jobPosition }
    });

    dialogRef.afterClosed().pipe(
      takeUntil(this.destroy$)
    ).subscribe(result => {
      if (result === true) {
        // Perform cancellation action here
        console.log('Changes canceled');
      }
    });
  }

  previewUserCV(fileName: string, event: Event) {
    event.preventDefault();
    event.stopPropagation();
    this.jobService.getCVFileByName(fileName).pipe(
      takeUntil(this.destroy$)
    ).subscribe((fileBlob) => {
      const blobUrl = URL.createObjectURL(fileBlob);
      this.selectedFilePath = blobUrl;
      this.dialog.open(this.filePreviewModal, {
        width: '80%',
        height: 'auto',
      });
    })
  }

  isUserJobPost(job: UserJobPost | CompanyJobPost): job is UserJobPost {
    //ad type id is specific property for userJobPost
    return (job as UserJobPost).advertisementTypeId !== undefined;
  }

  ngOnDestroy() {
    // Trigger cleanup of all subscriptions
    this.destroy$.next();
    this.destroy$.complete();
  }
}
