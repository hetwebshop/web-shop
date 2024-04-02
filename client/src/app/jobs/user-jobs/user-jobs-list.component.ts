import { Component } from '@angular/core';
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
import { map } from 'rxjs';
import { AdsPaginationParameters } from 'src/app/models/filterCriteria';
import { PagedResponse } from 'src/app/models/pagedResponse';

@Component({
  selector: 'app-user-jobs',
  templateUrl: './user-jobs-list.component.html',
  styleUrls: ['./user-jobs-list.component.css']
})
export class UserJobsListComponent {
  allJobs: UserJobPost[];
  adType: AdvertisementTypeEnum;
  user: User;
  paginationResponse: PagedResponse<UserJobPost>;
  paginationParameters: AdsPaginationParameters;
  jobCategories: JobCategory[];
  // pageNumber: number;
  // pageSize: number;
  // orderBy: string;

  constructor(private jobService: JobService, utility: UtilityService, private route: ActivatedRoute, 
    private router: Router, private datePipe: DatePipe, public dialog: MatDialog, private accountService: AccountService) {
    utility.setTitle('Oglasi');
    this.accountService.user$.subscribe((u) => (this.user = u));
  }

  ngOnInit(): void {
    this.loadJobCategories();
    console.log("ON INIT");
    this.route.queryParams.subscribe(params => {
      if (params['type']) {
        this.adType = this.getEnumValue(params['type']);
        if(this.adType != AdvertisementTypeEnum.JobAd && this.adType != AdvertisementTypeEnum.Service){
          this.router.navigateByUrl('/not-found');
        }

        this.paginationParameters = {
          pageNumber: 1,
          pageSize: 10,
          orderBy: "",
          advertisementTypeId: this.adType
        }
        this.fetchPaginatedItems(this.paginationParameters);
      }
    });

  }

  loadJobCategories(): void {
    this.jobService.getJobCategories()
      .subscribe(categories => {
        this.jobCategories = categories;
      });
  }

  getCategoryName(jobCategoryId: number): string {
    return this.jobCategories.find(r => r.id == jobCategoryId).name;
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

  fetchPaginatedItems(filterCriteria: AdsPaginationParameters): void {
    filterCriteria = {...filterCriteria, pageSize: this.paginationParameters.pageSize, pageNumber: this.paginationParameters.pageNumber};
    console.log("FILTER CRITERIA");
    console.log(filterCriteria);
    this.paginationParameters = filterCriteria;
    this.jobService.getAds(this.paginationParameters).subscribe(filteredItems => {
      this.allJobs = filteredItems.items;
      this.paginationResponse = filteredItems;
      // Update items on the second component
      // filteredItems: Array of items fetched from the backend
    });
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

  onPageChange(pageNumber: number) {
    this.paginationParameters = { ...this.paginationParameters, pageNumber: pageNumber };
    this.fetchPaginatedItems(this.paginationParameters);
  }

  onPageSizeChange(pageSize: number) {
    this.paginationParameters = { ...this.paginationParameters, pageSize: pageSize };
    this.fetchPaginatedItems(this.paginationParameters);
  }
}
