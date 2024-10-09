import { DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, UntypedFormGroup, Validators } from '@angular/forms';
import { MatOption } from '@angular/material/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as moment from 'moment';
import { Observable, Subscription, map, switchMap } from 'rxjs';
import { UserProfile } from 'src/app/modal/user';
import { AdvertisementTypeEnum, Gender, JobPostStatus } from 'src/app/models/enums';
import { City, Country } from 'src/app/models/location';
import { AdvertisementType, ApplicantEducation, JobCategory, JobType, UserJobPost } from 'src/app/models/userJobPost';
import { AccountService } from 'src/app/services/account.service';
import { JobService } from 'src/app/services/job.service';
import { LocationService } from 'src/app/services/location.service';
import { UtilityService } from 'src/app/services/utility.service';
import { FiltersQuery } from 'src/app/store/filters/filters.query';
import { FiltersStore } from 'src/app/store/filters/filters.store';
import { AdsQuery } from 'src/app/store/jobs/ads.query';
import { AdsStore } from 'src/app/store/jobs/ads.store';

@Component({
  selector: 'app-my-ad',
  templateUrl: './my-ad.component.html',
  styleUrls: ['./my-ad.component.css']
})
export class MyAdComponent {
  isEditMode: boolean = true;
  jobId: number;
  job: UserJobPost;
  private subscription: Subscription;
  form: FormGroup;
  adUpdateForm: UntypedFormGroup;
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
  selectedFile: File;
  existingFilePath: string | null = null;
  selectedFileName: string | null = null;
  populateFormWithUserProfileData: boolean = false;
  isJobInStatusClosedOrDeleted: boolean = false;
  canJobBeReactivated: boolean = false;

  @ViewChild('allSelected', { static: true }) private allSelected: MatOption;

  constructor(private jobService: JobService, private locationService: LocationService, utility: UtilityService, 
    private route: ActivatedRoute, private cdr: ChangeDetectorRef, 
    private fb: FormBuilder, private accountService: AccountService, private router: Router) {
    utility.setTitle('Detalji oglasa');
  }

  genderName(gender: Gender): string {
    return this.genderMap[gender] || 'Ostali';
  }

  ngOnInit(): void {
    this.accountService.getProfile().subscribe((user: UserProfile) => {
      this.user = user;
    });

    this.route.params.subscribe(params => {
      this.jobId = +params['id'];
        this.subscription = this.jobService.getMyJobById(this.jobId)
          .subscribe({
            next: (response) => {
              this.job = response;
              console.log("JOB IS ", response);
              this.existingFilePath = this.job.cvFilePath;
              this.loadAdTypes();

              this.initializeForm();
            },
            error: (errorResponse) => {
              console.log('Error fetching job', errorResponse);
            }
          });
    });
  }

  private initializeForm() {
    this.applicantEducations = this.fb.array([]);
    this.adUpdateForm = this.fb.group({
      id: new FormControl(this.job?.id),
      position: new FormControl(this.job?.position, Validators.required),
      biography: new FormControl(this.job?.biography),
      applicantFirstName: new FormControl(this.job?.applicantFirstName, Validators.required),
      applicantLastName: new FormControl(this.job?.applicantLastName, Validators.required),
      applicantEmail: new FormControl(this.job?.applicantEmail, [Validators.required, Validators.email]),
      applicantPhoneNumber: new FormControl(this.job?.applicantPhoneNumber, Validators.required),
      applicantGender: new FormControl(this.job?.applicantGender, Validators.required),
      applicantDateOfBirth: new FormControl(this.job?.applicantDateOfBirth, Validators.required),
      applicantEducations: this.applicantEducations,
      jobTypeId: new FormControl(this.job?.jobTypeId, Validators.required),
      cityId: new FormControl(this.job?.cityId, Validators.required),
      countryId: new FormControl({ value: this.job?.countryId, disabled: true}, Validators.required),
      jobCategoryId: new FormControl(this.job?.jobCategoryId, Validators.required),
      advertisementTypeId: new FormControl({ value: this.job?.advertisementTypeId, disabled: true}, Validators.required),
      price: new FormControl(this.job?.price),
      cvFile: new FormControl(this.job?.cvFile),
      adDuration: new FormControl(this.job?.adDuration, Validators.required)
    });

    this.updateApplicantEducations(this.job?.applicantEducations);

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

  prepareModel(data: any) : FormData {
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
      adEndDate: moment(now).add(data.adDuration, 'days')
    };

    const formData = new FormData();
    for (const [key, value] of Object.entries(model)) {
        if (value instanceof Date || moment.isMoment(value)) {
          const dateOfBirth = moment(value).format('YYYY-MM-DD');
          formData.append(key, dateOfBirth);
        } else if (Array.isArray(value)) {
            value.forEach((item, index) => {
                Object.entries(item).forEach(([itemKey, itemValue]) => {
                  formData.append(`${key}[${index}].${itemKey}`, itemValue as string);
                });
            });
        } 
        // else if (typeof value === 'object' && value !== null) {
        //     Object.entries(value).forEach(([objKey, objValue]) => {
        //       formData.append(`${key}.${objKey}`, objValue as string);
        //     });
        //} 
        else {
          console.log("KEY", key, value);
          if(key !== 'cvFile' && value != null){
            formData.append(key, value.toString());
          }
        }
    }
    formData.append('cvFile', model.cvFile);
    return formData;
  }

private updateApplicantEducations(educations: any[]): void {
  this.applicantEducations = this.adUpdateForm.get('applicantEducations') as FormArray;

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
        console.log(`Invalid Field: ${key}`);
      } else if (control instanceof FormGroup) {
        this.logInvalidControls(control);
      } else if (control instanceof FormArray) {
        control.controls.forEach((ctrl, index) => {
          if (ctrl instanceof FormGroup) {
            console.log(`FormArray Group Index: ${index}`);
            this.logInvalidControls(ctrl);
          } else if (ctrl instanceof FormControl && ctrl.invalid) {
            console.log(`Invalid Field in FormArray: ${key}[${index}]`);
          }
        });
      }
    });
  }
  onSubmit(): void {
    if (this.adUpdateForm.valid) {
      const formData = this.adUpdateForm.getRawValue();
      const model = this.prepareModel(formData);
      this.subscription = this.jobService.upsertJob(this.isEditMode, model).subscribe();
    } 
    else {
      this.logInvalidControls(this.adUpdateForm);
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

  loadAdTypes(): void {
    this.jobService.getAdTypes()
      .subscribe(adTypes => {
        this.advertisementTypes = adTypes;
        const defaultType = this.job.jobTypeId;
        this.adUpdateForm.get('advertisementTypeId').setValue(defaultType);
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
  }

  deleteAd(event): void {
    event.preventDefault();
    this.jobService.deleteAd(this.job.id).subscribe((response) => {
      if(response == true) {
        this.job = { ...this.job, isDeleted: true, jobPostStatusId: JobPostStatus.Deleted };
        this.isJobInStatusClosedOrDeleted = true;
        this.disableForm();
      }
    });
  }

  closeAd(event): void {
    event.preventDefault();
    this.jobService.closeAd(this.job.id).subscribe((response) => {
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
    this.jobService.closeAd(this.job.id).subscribe((response) => {
      if(response == true) {
        this.job = { ...this.job, jobPostStatusId: JobPostStatus.Active };
        this.isJobInStatusClosedOrDeleted = false;
        this.canJobBeReactivated = false;
        this.enableForm();
      }
    });
  }

  onFileSelected(event): void {
    this.selectedFile = (event.target as HTMLInputElement).files[0];
    this.existingFilePath = null;
    this.selectedFileName = this.selectedFile.name;
    this.adUpdateForm.patchValue({ cvFile: this.selectedFile });
    this.adUpdateForm.markAsDirty();
  }

  deleteExistingFile() {
    this.existingFilePath = null;
    // Optionally, handle the deletion on the backend
  }

  getEnumStatusValue(value: number): string {
    return value == JobPostStatus.Active ? "Aktivan Oglas" : value == JobPostStatus.Closed ? "Istekao Oglas" : "Obrisan Oglas";
  }

  disableForm() {
    this.adUpdateForm.disable();
    const formArray = this.adUpdateForm.get('applicantEducations') as FormArray;
    formArray.controls.forEach(control => {
      control.disable();
    });
  }

  enableForm() {
    this.adUpdateForm.enable();
    const formArray = this.adUpdateForm.get('applicantEducations') as FormArray;
    formArray.controls.forEach(control => {
      control.enable();
    });
  }
}
