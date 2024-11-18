import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, Subscription, map, switchMap, takeUntil } from 'rxjs';
import { AdvertisementType, ApplicantEducation, JobCategory, JobType, UserJobPost, UserJobSubcategory } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { UtilityService } from 'src/app/services/utility.service';
import { FormBuilder, FormGroup, Validators, FormControl, FormArray } from '@angular/forms';
import { LocationService } from 'src/app/services/location.service';
import { City, Country } from 'src/app/models/location';
import { AdvertisementTypeEnum, Gender, JobPostStatus } from 'src/app/models/enums';
import { MatOption } from '@angular/material/core';
import { User, UserInfo, UserProfile } from 'src/app/modal/user';
import { AccountService } from 'src/app/services/account.service';
import * as moment from 'moment';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'src/app/services/toastr.service';
import { PricingPlan } from 'src/app/models/pricingPlan';
import { PricingPlanService } from 'src/app/services/pricingPlan.service';

@Component({
  selector: 'app-job-details-manager',
  templateUrl: './job-details-manager.component.html',
  styleUrls: ['./job-details-manager.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class JobDetailsManagerComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  isEditMode: boolean = false;
  jobId: number;
  job: UserJobPost;
  private subscription: Subscription;
  form: FormGroup;
  countries: Country[] = [];
  cities: City[] = [];
  jobTypes: JobType[] = [];
  jobCategories: JobCategory[] = [];
  advertisementTypes: AdvertisementType[] = [];
  selectedCategory: any;
  applicantEducations: FormArray;
  genders = Object.keys(Gender) as Array<keyof typeof Gender>;
  genderMap = Gender;
  selectedAdvertisementType: any;
  AdvertisementTypeEnum = AdvertisementTypeEnum;
  selectAllId: number = 0;
  user: UserProfile;
  isJobAd: boolean = true;
  selectedFile: File;
  existingFilePath: string | null = null;
  selectedFileName: string | null = null;
  selectedFilePath: string | null = null;
  @ViewChild('filePreviewModal') filePreviewModal!: TemplateRef<any>;
  populateFormWithUserProfileData: boolean = false;
  selectedPlan: string;
  pricingPlans: PricingPlan[];
  filteredPricingPlans: PricingPlan[];

  filteredCities = []; 
  filteredCategories = []; 
  filteredJobTypes = []; 
  citieSearchKeyword = "";
  jobTypesSearchKeyword = "";
  jobCategoriesSearchKeyword = "";

  @ViewChild('allSelected', { static: true }) private allSelected: MatOption;

  constructor(private jobService: JobService, private locationService: LocationService, utility: UtilityService,
    private route: ActivatedRoute, private cdr: ChangeDetectorRef,
    private fb: FormBuilder, private accountService: AccountService, private pricingPlanService: PricingPlanService, private toastrService: ToastrService, private router: Router, private dialog: MatDialog) {
    this.route.url.pipe(takeUntil(this.destroy$)).subscribe(urlSegment => {
      this.isJobAd = !urlSegment.some(segment => segment.path.includes('service'));
    });
    utility.setTitle('Detalji oglasa');
  }

  ngOnInit(): void {
    this.form = this.createForm();
    this.form.get('adDuration')?.valueChanges.pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.filterPricingPlans();
    });
    this.pricingPlanService.getAllPricingPlans().pipe(takeUntil(this.destroy$))
      .subscribe(pplans => {
        this.pricingPlans = pplans;
        this.selectedPlan = pplans.find(r => r.name === 'Plus').name;
        if (this.selectedPlan) {
          this.form.get('pricingPlanName')?.setValue(this.selectedPlan);
        }
        this.filterPricingPlans();
        this.cdr.detectChanges();
    });

    this.accountService.getProfile().pipe(takeUntil(this.destroy$)).subscribe((user: UserProfile) => {
      this.user = user;
      if (this.user.cvFilePath) {
        this.existingFilePath = this.user.cvFilePath;
      }
    })
    this.loadCountries();
    this.loadCities();
    this.loadJobTypes();
    this.loadJobCategories();
    this.loadAdTypes();
  }

  genderName(gender: Gender): string {
    return this.genderMap[gender] || 'Ostali';
  }

  private createForm(): FormGroup {
    this.applicantEducations = this.fb.array([]);

    const formGroup = this.fb.group({
      id: new FormControl(0),
      position: new FormControl('', Validators.required),
      biography: new FormControl(''),
      applicantFirstName: new FormControl('', Validators.required),
      applicantLastName: new FormControl('', Validators.required),
      applicantEmail: new FormControl('', [Validators.required, Validators.email]),
      applicantPhoneNumber: new FormControl('', Validators.required),
      applicantGender: new FormControl('', Validators.required),
      applicantDateOfBirth: new FormControl('', Validators.required),
      applicantEducations: this.applicantEducations,
      jobTypeId: new FormControl('', Validators.required),
      cityId: new FormControl('', Validators.required),
      countryId: new FormControl({ value: null, disabled: true }, Validators.required),
      jobCategoryId: new FormControl('', Validators.required),
      advertisementTypeId: new FormControl({ value: this.isJobAd ? this.AdvertisementTypeEnum.JobAd : this.AdvertisementTypeEnum.Service, disabled: true }, Validators.required),
      price: new FormControl(''),
      cvFile: new FormControl(null),
      adDuration: new FormControl("7", Validators.required),
      adTitle: new FormControl(null, Validators.required),
      adAdditionalDescription: new FormControl(null, Validators.required),
      pricingPlanName: new FormControl("Plus", Validators.required)
    });

    if (!this.isJobAd) {
      formGroup.get('position')?.clearValidators();
      formGroup.get('biography')?.clearValidators();
      formGroup.get('jobTypeId')?.clearValidators();
    }

    formGroup.updateValueAndValidity();
    return formGroup;
  }

  selectPlan(value: string) {
    console.log('Selected plan:', value);
    this.selectedPlan = value;
    this.form.get('pricingPlanName').setValue(this.selectedPlan); // Update form control value
  }

  prepareModel(data: any): FormData {
    const applicantEducations: ApplicantEducation[] = [];
    this.applicantEducations.controls.forEach(control => {
      const educationModel: ApplicantEducation = {
        degree: control.get('degree').value,
        institutionName: control.get('institutionName').value,
        fieldOfStudy: control.get('fieldOfStudy').value,
        educationStartYear: control.get('educationStartYear').value,
        educationEndYear: control.get('educationEndYear').value,
        university: control.get('university').value
      };
      applicantEducations.push(educationModel);
    });

    let now = moment();
    console.log("CV IS ", data);
    const model: UserJobPost = {
      id: data.id,
      position: data.position || '',
      biography: data.biography || '',
      applicantFirstName: data.applicantFirstName,
      applicantLastName: data.applicantLastName,
      applicantEmail: data.applicantEmail,
      applicantPhoneNumber: data.applicantPhoneNumber,
      applicantGender: data.applicantGender,
      applicantDateOfBirth: data.applicantDateOfBirth,
      applicantEducations: applicantEducations,
      jobTypeId: data.jobTypeId,
      cityId: data.cityId,
      countryId: data.countryId,
      jobCategoryId: data.jobCategoryId,
      jobPostStatusId: JobPostStatus.Active,
      advertisementTypeId: data.advertisementTypeId,
      cvFile: data.cvFile,
      adDuration: data.adDuration,
      adStartDate: now,
      adEndDate: moment(now).add(data.adDuration, 'days'),
      adAdditionalDescription: data.adAdditionalDescription,
      adTitle: data.adTitle,
      price: data.price,
      pricingPlanName: data.pricingPlanName
    };

    const formData = new FormData();
    for (const [key, value] of Object.entries(model)) {
      if (value instanceof Date || moment.isMoment(value)) {
        formData.append(key, moment(value).toISOString());
      } else if (Array.isArray(value)) {
        value.forEach((item, index) => {
          Object.entries(item).forEach(([itemKey, itemValue]) => {
            formData.append(`${key}[${index}].${itemKey}`, itemValue as string);
          });
        });
      }
      else {
        if (key !== 'cvFile' && value != null) {
          formData.append(key, value.toString());
        }
      }
    }
    formData.append('cvFile', model.cvFile);
    return formData;
  }

  private updateApplicantEducations(educations: any[]): void {
    this.applicantEducations = this.form.get('applicantEducations') as FormArray;

    this.applicantEducations.clear();

    educations.forEach(education => {
      this.applicantEducations.push(this.createEducation(education));
    });
  }

  onCategoryChange(categoryId: number): void {
    this.selectedCategory = this.jobCategories.find(category => category.id === categoryId);
  }

  onAdTypeChange(adTypeId: number): void {
    this.selectedAdvertisementType = adTypeId;
  }
  logInvalidControls(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      if (control instanceof FormControl && control.invalid) {
      } else if (control instanceof FormGroup) {
        this.logInvalidControls(control);
      } else if (control instanceof FormArray) {
        control.controls.forEach((ctrl, index) => {
          if (ctrl instanceof FormGroup) {
            this.logInvalidControls(ctrl);
          } else if (ctrl instanceof FormControl && ctrl.invalid) {
          }
        });
      }
    });
  }
  onSubmit(): void {
    const formData = this.form.getRawValue();
    if (this.form.valid) {
      const model = this.prepareModel(formData);
      this.subscription = this.jobService.upsertJob(this.isEditMode, model).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.toastrService.success("Uspješno ste kreirali objavu!");
          setTimeout(() => {
            window.location.reload();
          }, 2000);
        },
        error: () => {
          this.toastrService.error("Desila se greška prilikom kreiranja objave!");
        }
      });
    }
    else {
      this.logInvalidControls(this.form);
      Object.keys(this.form.controls).forEach(field => {
        const control = this.form.get(field);
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
        this.form.get('countryId').setValue(defaultCountry.id);
      });
  }


  filterPricingPlans(): void {
    const selectedDuration = this.form.get('adDuration')?.value ?? "7";
    if (this.pricingPlans && this.pricingPlans.length > 0) {
      this.filteredPricingPlans = this.pricingPlans.filter(r => r.adActiveDays === +selectedDuration);
    } else {
      this.filteredPricingPlans = []; // Reset if there are no plans
    }
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
        console.log("Job categories" + JSON.stringify(this.jobCategories));
      });
  }

  loadAdTypes(): void {
    this.jobService.getAdTypes().pipe(takeUntil(this.destroy$))
      .subscribe(adTypes => {
        this.advertisementTypes = adTypes;
        const defaultType = this.isJobAd ? this.AdvertisementTypeEnum.JobAd : this.AdvertisementTypeEnum.Service;
        this.form.get('advertisementTypeId').setValue(defaultType);
        console.log("FETCH TYPES", this.form.get('advertisementTypeId').value);
      });
  }

  createEducation(education?: ApplicantEducation): FormGroup {
    return this.fb.group({
      degree: new FormControl(education?.degree || '', Validators.required),
      institutionName: new FormControl(education?.institutionName || '', Validators.required),
      fieldOfStudy: new FormControl(education?.fieldOfStudy || '', Validators.required),
      educationStartYear: new FormControl(education?.educationStartYear || '', Validators.required),
      educationEndYear: new FormControl(education?.educationEndYear || ''),
      university: new FormControl(education?.university || '', Validators.required)
    });
  }

  addEducation(): void {
    this.applicantEducations.push(this.createEducation());
  }

  removeEducation(index: number): void {
    this.applicantEducations.removeAt(index);
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }

    this.destroy$.next();
    this.destroy$.complete();
  }

  onToggleUseUserDataChange(event): void {
    const checked = event.checked;
    this.populateFormWithUserProfileData = checked;
    if (checked) {
      this.form.patchValue({
        applicantFirstName: this.user.firstName,
        applicantLastName: this.user.lastName,
        cityId: this.user.cityId,
        applicantGender: this.user.gender,
        applicantEmail: this.user.email,
        applicantPhoneNumber: this.user.phoneNumber,
        jobTypeId: this.user.jobTypeId,
        jobCategoryId: this.user.jobCategoryId,
        applicantDateOfBirth: this.user.dateOfBirth,
        biography: this.user.biography,
        position: this.user.position,
        //cvFile: this.user.cvFilePath
      });
      this.updateApplicantEducations(this.user.userEducations);
    } else {
      this.form.reset();
      this.form.patchValue({
        advertisementTypeId: this.isJobAd ? this.AdvertisementTypeEnum.JobAd : this.AdvertisementTypeEnum.Service,
      })
    }
  }

  openFilePreview(): void {
    if (this.selectedFilePath) {
      this.dialog.open(this.filePreviewModal, {
        width: '80%',
        height: 'auto',
      });
    }
  }

  onFileSelected(event): void {
    event.preventDefault();
    event.stopPropagation();
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.selectedFileName = file.name;
      this.selectedFilePath = URL.createObjectURL(file);
      this.selectedFile = file;
      this.form.patchValue({ cvFile: this.selectedFile });

      // Clear input value to allow re-upload of the same file
      input.value = '';
    }
  }

  previewUserCV(fileName: string) {
    this.jobService.getCVFileByName(fileName).pipe(takeUntil(this.destroy$)).subscribe((fileBlob) => {
      const blobUrl = URL.createObjectURL(fileBlob);
      this.selectedFilePath = blobUrl;
      this.dialog.open(this.filePreviewModal, {
        width: '80%',
        height: 'auto',
      });
    })
  }

  removeSelectedFile(event: Event): void {
    event.stopPropagation(); // Prevent triggering openFilePreview
    this.selectedFileName = null;
    this.selectedFilePath = null;
  }

  deleteExistingFile() {
    this.existingFilePath = null;
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
