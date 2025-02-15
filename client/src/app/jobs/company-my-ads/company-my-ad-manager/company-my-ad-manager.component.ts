import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, UntypedFormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { Subject, Subscription, takeUntil } from 'rxjs';
import { ConfirmationModalComponent } from 'src/app/modal/confirmation-modal/confirmation-modal.component';
import { UserProfile } from 'src/app/modal/user';
import { CompanyJobPost } from 'src/app/models/companyJobAd';
import { JobPostStatus } from 'src/app/models/enums';
import { City, Country } from 'src/app/models/location';
import { JobCategory, JobType } from 'src/app/models/userJobPost';
import { AccountService } from 'src/app/services/account.service';
import { CompanyJobService } from 'src/app/services/company-job.service';
import { JobService } from 'src/app/services/job.service';
import { LocationService } from 'src/app/services/location.service';
import { ToastrService } from 'src/app/services/toastr.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-company-my-ad-manager',
  templateUrl: './company-my-ad-manager.component.html',
  styleUrls: ['./company-my-ad-manager.component.css']
})
export class CompanyMyAdManagerComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
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

  filteredCities = []; 
  filteredCategories = []; 
  filteredJobTypes = []; 
  citieSearchKeyword = "";
  jobTypesSearchKeyword = "";
  jobCategoriesSearchKeyword = "";

  constructor(private jobService: JobService, private companyJobService: CompanyJobService, private locationService: LocationService, utility: UtilityService, 
    private route: ActivatedRoute, private cdr: ChangeDetectorRef,  private dialog: MatDialog,
    private fb: FormBuilder, private accountService: AccountService, private toastr: ToastrService, private router: Router) {
    utility.setTitle('Detalji oglasa');
  }

  ngOnInit(): void {
    this.accountService.getProfile().pipe(takeUntil(this.destroy$)).subscribe((user: UserProfile) => {
      this.user = user;
    });

    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      this.jobId = +params['id'];
        this.subscription = this.companyJobService.getCompanyJobById(this.jobId).pipe(takeUntil(this.destroy$))
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
      emailForReceivingApplications: data.emailForReceivingApplications,
      pricingPlanName: this.job.pricingPlanName
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
      this.subscription = this.companyJobService.upsertJob(this.isEditMode, model).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.toastr.success('Uspješno ste uredili oglas!');
        },
        error: () => {
          this.toastr.error('Desila se greška prilikom uređivanja objave!');
        }
      });
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
    this.locationService.getCountries().pipe(takeUntil(this.destroy$))
      .subscribe(countries => {
        this.countries = countries;
        const defaultCountry = countries[0];
        this.adUpdateForm.get('countryId').setValue(defaultCountry.id);
      });
  }

  loadCities(): void {
    this.locationService.getCities().pipe(takeUntil(this.destroy$))
      .subscribe(cities => {
        this.cities = cities;
        this.filteredCities = cities;
      });
  }

  loadJobTypes(): void {
    this.jobService.getJobTypes().pipe(takeUntil(this.destroy$))
      .subscribe(types => {
        this.jobTypes = types;
        this.filteredJobTypes = types;
      });
  }

  loadJobCategories(): void {
    this.jobService.getJobCategories().pipe(takeUntil(this.destroy$))
      .subscribe(categories => {
        this.jobCategories = categories.filter(r => r.parentId == null);
        this.filteredCategories = this.jobCategories;
      });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }

    this.destroy$.next();
    this.destroy$.complete();
  }

  deleteAd(event): void {
    event.preventDefault();
    const confirmationDialogRef = this.dialog.open(ConfirmationModalComponent,
      {
        data: {
          title: "Obriši oglas",
          message: "Da li ste sigurni da želite obrisati objavu? Obrisane objave možete naći poslije u sekciji 'Obrisani oglasi' te se obrisani oglas više ne može aktivirati!"
        }
      });
      confirmationDialogRef.afterClosed().pipe(takeUntil(this.destroy$)).subscribe(result => {
      if (result === true) {
        this.companyJobService.deleteAd(this.job.id).pipe(takeUntil(this.destroy$)).subscribe({
          next: (response) => {
          if(response == true) {
            this.job = { ...this.job, isDeleted: true, jobPostStatusId: JobPostStatus.Deleted };
            this.isJobInStatusClosedOrDeleted = true;
            this.disableForm();
            this.toastr.success("Uspješno obrisana objava!");
          }
          else
          this.toastr.error("Desila se greška prilikom brisanja objave!");
        },
        error: () => {
          this.toastr.error("Desila se greška prilikom brisanja objave!");
        }
        });
      }
    });
  }

  closeAd(event): void {
    event.preventDefault();
    const confirmationDialogRef = this.dialog.open(ConfirmationModalComponent,
      {
        data: {
          title: "Obriši oglas",
          message: "Da li ste sigurni da želite obrisati objavu? Obrisane objave možete naći poslije u sekciji 'Obrisani oglasi' te se obrisani oglas više ne može aktivirati!"
        }
      });
      confirmationDialogRef.afterClosed().pipe(takeUntil(this.destroy$)).subscribe(result => {
      if (result === true) {
        this.companyJobService.closeAd(this.job.id).pipe(takeUntil(this.destroy$)).subscribe({
          next: (response) => {
          if(response == true) {
            this.job = { ...this.job, jobPostStatusId: JobPostStatus.Closed };
            this.isJobInStatusClosedOrDeleted = true;
            this.disableForm();
            if(this.job && moment(this.job.adEndDate) > moment()){
              this.canJobBeReactivated = true;
            }
            this.toastr.success("Uspješno ste zatvorili oglas!");
          }
          else
            this.toastr.error("Desila se greška prilikom zatvaranja oglasa!");
        },
        error: () => {
          this.toastr.error("Desila se greška prilikom zatvaranja oglasa!");
        }
        });
      }
    });
  }

  reactivateAd(event): void {
    event.preventDefault();
    const confirmationDialogRef = this.dialog.open(ConfirmationModalComponent,
      {
        data: {
          title: "Obriši oglas",
          message: "Da li ste sigurni da želite obrisati objavu? Obrisane objave možete naći poslije u sekciji 'Obrisani oglasi' te se obrisani oglas više ne može aktivirati!"
        }
      });
      confirmationDialogRef.afterClosed().pipe(takeUntil(this.destroy$)).subscribe(result => {
      if (result === true) {
        this.companyJobService.closeAd(this.job.id).pipe(takeUntil(this.destroy$)).subscribe({
          next: (response) => {
          if(response == true) {
            this.job = { ...this.job, jobPostStatusId: JobPostStatus.Active };
            this.isJobInStatusClosedOrDeleted = false;
            this.canJobBeReactivated = false;
            this.enableForm();
            this.toastr.success("Uspješno ste aktivirali oglas!");
          }
          else
            this.toastr.error("Desila se greška prilikom ponovnog aktiviranja oglasa!");
        },
        error: () => {
          this.toastr.error("Desila se greška prilikom ponovnog aktiviranja oglasa!");
        }
        });
      }
    });
  }

  getEnumStatusValue(value: number): string {
    return value == JobPostStatus.Active ? "Aktivan Oglas" : value == JobPostStatus.Closed ? "Završen Oglas" : "Obrisan Oglas";
  }

  disableForm() {
    this.adUpdateForm.disable();
  }

  enableForm() {
    this.adUpdateForm.enable();
  }

  
    
  onKey(searchValue: string, formControlName: string) {
    if(formControlName == "cityId"){
      this.citieSearchKeyword = searchValue;
      this.filteredCities = this.cities.filter(city =>
        city.name.toLowerCase().includes(searchValue.toLowerCase())
      );
    } 
    else if(formControlName == "jobCategoryId"){
      this.jobCategoriesSearchKeyword = searchValue;
      this.filteredCategories = this.jobCategories.filter(cg =>
        cg.name.toLowerCase().includes(searchValue.toLowerCase())
      );
    } 
    else if(formControlName == "jobTypeId"){
      this.jobTypesSearchKeyword = searchValue;
      this.filteredJobTypes = this.jobTypes.filter(jt =>
        jt.name.toLowerCase().includes(searchValue.toLowerCase())
      );
    } 
  }

  resetSearch(formControlName: string): void {
    this.citieSearchKeyword = "";
    this.jobTypesSearchKeyword = "";
    this.jobCategoriesSearchKeyword = "";
    if(formControlName == "cityId"){
      this.filteredCities = [...this.cities];
    }
    else if(formControlName == "jobCategoryId"){
      this.filteredCategories = [...this.jobCategories];
    }
    else if(formControlName == "jobTypeId"){
      this.filteredJobTypes = [...this.jobTypes];
    }
  }
}
