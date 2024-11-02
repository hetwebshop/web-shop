import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { JobCategory, JobType } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { UtilityService } from 'src/app/services/utility.service';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { LocationService } from 'src/app/services/location.service';
import { City, Country } from 'src/app/models/location';
import { AccountService } from 'src/app/services/account.service';
import { CompanyJobPost } from 'src/app/models/companyJobAd';
import { CompanyJobService } from 'src/app/services/company-job.service';
import * as moment from 'moment';
import { JobPostStatus } from 'src/app/models/enums';
import { ToastrService } from 'src/app/services/toastr.service';

@Component({
  selector: 'app-company-create-job',
  templateUrl: './company-create-job.component.html',
  styleUrls: ['./company-create-job.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CompanyCreateJobComponent implements OnInit, OnDestroy {
  isEditMode: boolean = false;
  jobId: number;
  job: CompanyJobPost;
  private subscription: Subscription;
  form: FormGroup;
  countries: Country[] = [];
  cities: City[] = [];
  jobTypes: JobType[] = [];
  jobCategories: JobCategory[] = [];
  selectedCategory: any;

  constructor(private jobService: JobService, private companyJobService: CompanyJobService, private locationService: LocationService, utility: UtilityService, 
    private route: ActivatedRoute, private cdr: ChangeDetectorRef, 
    private fb: FormBuilder, private accountService: AccountService, private toastrService: ToastrService, private router: Router) {
    utility.setTitle('Detalji oglasa za posao');
  }

  ngOnInit(): void {
    this.form = this.createForm();
    this.loadCountries();
    this.loadCities();
    this.loadJobTypes();
    this.loadJobCategories();

    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.jobId = +params['id'];
        this.subscription = this.companyJobService.getJobById(this.jobId)
          .subscribe({
            next: (response) => {
              this.job = response;
              this.onCategoryChange(this.job.jobCategoryId);
              this.form.patchValue({
                id: this.job.id,
                jobDescription: this.job.jobDescription,
                jobTypeId: this.job.jobTypeId,
                jobCategoryId: this.job.jobCategoryId,
                cityId: this.job.cityId,
                countryId: this.job.countryId,
              });
              this.cdr.detectChanges();
            },
            error: (errorResponse) => {
              console.log('Error fetching job', errorResponse);
            }
          });
      } else {
        this.isEditMode = false;
      }
    });
  }

  private createForm(): FormGroup {
    return this.fb.group({
      id: new FormControl(0),
      jobDescription: new FormControl('', Validators.required),
      jobTypeId: new FormControl(0, Validators.required),
      cityId: new FormControl('', Validators.required),
      countryId: new FormControl({ value: null, disabled: true}, Validators.required),
      jobCategoryId: new FormControl('', Validators.required),
      adDuration: new FormControl(null, Validators.required),
      position: new FormControl('', Validators.required),
      adName: new FormControl('', Validators.required),
      emailForReceivingApplications: new FormControl('', [Validators.email, Validators.required])
    });
  }

  onCategoryChange(categoryId: number): void {
    this.selectedCategory = this.jobCategories.find(category => category.id === categoryId);
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

  onSubmit(): void {
    if (this.form.valid) {
      const formData = this.form.getRawValue();
      const model = this.prepareModel(formData);
      this.subscription = this.companyJobService.upsertJob(this.isEditMode, model).subscribe({
        next: () => {
          this.toastrService.success("Uspješno ste kreirali oglas za posao!");
          setTimeout(() => {
            window.location.reload();
          }, 2000);
        },
        error: () => {
          this.toastrService.error("Desila se greška prilikom kreiranja oglasa za posao!");
        }
      });
    } 
    else {
      Object.keys(this.form.controls).forEach(field => {
        const control = this.form.get(field);
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
        this.form.get('countryId').setValue(defaultCountry.id);
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
        console.log("Job categories" + JSON.stringify(this.jobCategories));
      });
  }
  
  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
