import { Component, Inject, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AdvertisementTypeEnum } from 'src/app/models/enums';
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
  jobCategories$ = this.jobCategoryQuery.selectAll();
  isLargeScreen = true;
  isJobAd: boolean;
  selectedFilePath: string | null = null;
  @ViewChild('filePreviewModal') filePreviewModal!: TemplateRef<any>;
  fileUrl: string = ""
  
  constructor(private jobService: JobService, utility: UtilityService, private route: ActivatedRoute,
    private router: Router, private datePipe: DatePipe, public dialog: MatDialog,
    private accountService: AccountService, private filtersStore: FiltersStore,
    private filtersQuery: FiltersQuery, private http: HttpClient, private jobCategoryQuery: JobCategoryQuery, private breakpointObserver: BreakpointObserver) {
    utility.setTitle('Oglasi');
    this.accountService.user$.subscribe((u) => (this.user = u));
  }

  ngOnInit(): void {
    this.breakpointObserver.observe([
      "(min-width: 768px)"
    ]).subscribe(result => {
      if(result.matches)
        this.isLargeScreen = true;
      else 
        this.isLargeScreen = false;
    });
    this.jobCategories$.subscribe((jobCategories) => {
      this.jobCategories = jobCategories;
    })
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

  previewUserCV(fileName: string) {
    this.jobService.getCVFileByName(fileName).subscribe((fileBlob) => {
      const blobUrl = URL.createObjectURL(fileBlob);
      this.selectedFilePath = blobUrl;
      this.dialog.open(this.filePreviewModal, {
        width: '80%',
        height: 'auto',
      });
    })
    // this.selectedFilePath = this.jobService.getFileUrl(cvFilePath);
    // this.dialog.open(this.filePreviewModal, {
    //   width: '80%',
    //   height: 'auto',
    // });
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
