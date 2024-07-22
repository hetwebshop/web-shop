import { ChangeDetectorRef, Component } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { Subscription } from 'rxjs';
import { UserProfile } from 'src/app/modal/user';
import { CompanyJobPost } from 'src/app/models/companyJobAd';
import { JobPostStatus } from 'src/app/models/enums';
import { City, Country } from 'src/app/models/location';
import { JobCategory, JobType } from 'src/app/models/userJobPost';
import { AccountService } from 'src/app/services/account.service';
import { CompanyJobService } from 'src/app/services/company-job.service';
import { JobService } from 'src/app/services/job.service';
import { LocationService } from 'src/app/services/location.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-company-my-ad-manager',
  templateUrl: './company-my-ad-manager.component.html',
  styleUrls: ['./company-my-ad-manager.component.css']
})
export class CompanyMyAdManagerComponent {
  isEditMode: boolean = true;
  jobId: number;
  job: CompanyJobPost;
  private subscription: Subscription;
  form: FormGroup;
  adUpdateForm: UntypedFormGroup;
  countries: Country[] = [];
  cities: City[] = [];
  jobTypes: JobType[] = [];
  jobCategories: JobCategory[] = [];
  selectedCategory: any;
  user: UserProfile;
  isJobInStatusClosedOrDeleted: boolean = false;
  canJobBeReactivated: boolean = false;

  constructor(private jobService: JobService, private companyJobService: CompanyJobService, private locationService: LocationService, utility: UtilityService, 
    private route: ActivatedRoute, private cdr: ChangeDetectorRef, 
    private fb: FormBuilder, private accountService: AccountService, private router: Router) {
    utility.setTitle('Detalji oglasa');
  }

  ngOnInit(): void {
    this.accountService.getProfile().subscribe((user: UserProfile) => {
      this.user = user;
    });

    this.route.params.subscribe(params => {
      this.jobId = +params['id'];
        this.subscription = this.companyJobService.getCompanyJobById(this.jobId)
          .subscribe({
            next: (response) => {
              this.job = response;
              this.initializeForm();
            },
            error: (errorResponse) => {
              console.log('Error fetching job', errorResponse);
            }
          });
    });
  }

  private initializeForm() {
    this.adUpdateForm = this.fb.group({
      id: new FormControl(this.job?.id),
      jobDescription: new FormControl(this.job?.jobDescription, Validators.required),
      jobTypeId: new FormControl(this.job?.jobTypeId, Validators.required),
      cityId: new FormControl(this.job?.cityId, Validators.required),
      countryId: new FormControl({ value: this.job?.countryId, disabled: true}, Validators.required),
      jobCategoryId: new FormControl(this.job?.jobCategoryId, Validators.required),
      adDuration: new FormControl(this.job?.adDuration, Validators.required),
      position: new FormControl(this.job?.position, Validators.required),
      adName: new FormControl(this.job?.adName, Validators.required),
      emailForReceivingApplications: new FormControl(this.job?.emailForReceivingApplications, [Validators.email, Validators.required])
    });

    if(this.job?.isDeleted || this.job?.jobPostStatusId == JobPostStatus.Closed)
    {    
      this.isJobInStatusClosedOrDeleted = true;
      this.disableForm();

      if(this.job?.jobPostStatusId == JobPostStatus.Closed && moment(this.job.adEndDate) > moment()){
        this.canJobBeReactivated = true;
      }
    }

    this.loadCountries();
    this.loadCities();
    this.loadJobTypes();
    this.loadJobCategories();
  }

  prepareModel(data: any) : CompanyJobPost {
    let now = moment();
    const model: CompanyJobPost = {
      id: data.id,
      jobDescription: data.jobDescription || '',
      jobTypeId: data.jobTypeId,
      cityId: data.cityId,
      countryId: data.countryId,
      jobCategoryId: data.jobCategoryId,
      jobPostStatusId: JobPostStatus.Active,
      adDuration: data.adDuration,
      adStartDate: now,
      adEndDate: moment(now).add(data.adDuration, 'days'),
      position: data.position,
      adName: data.adName,
      emailForReceivingApplications: data.emailForReceivingApplications
    };
    
    return model;
  }

  onCategoryChange(categoryId: number): void {
    this.selectedCategory = this.jobCategories.find(category => category.id === categoryId);
  }


  onSubmit(): void {
    if (this.adUpdateForm.valid) {
      const formData = this.adUpdateForm.getRawValue();
      const model = this.prepareModel(formData);
      this.subscription = this.companyJobService.upsertJob(this.isEditMode, model).subscribe();
    } 
    else {
      Object.keys(this.adUpdateForm.controls).forEach(field => {
        const control = this.adUpdateForm.get(field);
        if (control.invalid) {
          control.markAsTouched();
        }
      });
    }
  }

  loadCountries(): void {
    this.locationService.getCountries()
      .subscribe(countries => {
        this.countries = countries;
        const defaultCountry = countries[0];
        this.adUpdateForm.get('countryId').setValue(defaultCountry.id);
      });
  }

  loadCities(): void {
    this.locationService.getCities()
      .subscribe(cities => {
        this.cities = cities;
      });
  }

  loadJobTypes(): void {
    this.jobService.getJobTypes()
      .subscribe(types => {
        this.jobTypes = types
      });
  }

  loadJobCategories(): void {
    this.jobService.getJobCategories()
      .subscribe(categories => {
        this.jobCategories = categories.filter(r => r.parentId == null);
      });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  deleteAd(event): void {
    event.preventDefault();
    this.companyJobService.deleteAd(this.job.id).subscribe((response) => {
      if(response == true) {
        this.job = { ...this.job, isDeleted: true, jobPostStatusId: JobPostStatus.Deleted };
        this.isJobInStatusClosedOrDeleted = true;
        this.disableForm();
      }
    });
  }

  closeAd(event): void {
    event.preventDefault();
    this.companyJobService.closeAd(this.job.id).subscribe((response) => {
      if(response == true) {
        this.job = { ...this.job, jobPostStatusId: JobPostStatus.Closed };
        this.isJobInStatusClosedOrDeleted = true;
        this.disableForm();
        if(this.job && moment(this.job.adEndDate) > moment()){
          this.canJobBeReactivated = true;
        }
      }
    });
  }

  reactivateAd(event): void {
    event.preventDefault();
    this.companyJobService.closeAd(this.job.id).subscribe((response) => {
      if(response == true) {
        this.job = { ...this.job, jobPostStatusId: JobPostStatus.Active };
        this.isJobInStatusClosedOrDeleted = false;
        this.canJobBeReactivated = false;
        this.enableForm();
      }
    });
  }

  getEnumStatusValue(value: number): string {
    return value == JobPostStatus.Active ? "Aktivan Oglas" : value == JobPostStatus.Closed ? "Istekao Oglas" : "Obrisan Oglas";
  }

  disableForm() {
    this.adUpdateForm.disable();
  }

  enableForm() {
    this.adUpdateForm.enable();
  }
}
