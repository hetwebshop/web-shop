import { DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, TemplateRef, ViewChild } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, UntypedFormGroup, Validators } from '@angular/forms';
import { MatOption } from '@angular/material/core';
import { MatDialog } from '@angular/material/dialog';
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
import { ToastrService } from 'src/app/services/toastr.service';
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
  selectedFilePath: string | null = null;
  @ViewChild('filePreviewModal') filePreviewModal!: TemplateRef<any>;
  populateFormWithUserProfileData: boolean = false;
  isJobInStatusClosedOrDeleted: boolean = false;
  canJobBeReactivated: boolean = false;
  isJobAd = true;

  @ViewChild('allSelected', { static: true }) private allSelected: MatOption;

  constructor(private jobService: JobService, private locationService: LocationService, utility: UtilityService, 
    private route: ActivatedRoute, private cdr: ChangeDetectorRef, 
    private fb: FormBuilder, private accountService: AccountService, private toastr: ToastrService, private router: Router, private dialog: MatDialog) {
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
              this.isJobAd = response.advertisementTypeId == 1 ? true : false;
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
      adDuration: new FormControl(this.job?.adDuration, Validators.required),
      adTitle: new FormControl(this.job?.adTitle),
      adAdditionalDescription: new FormControl(this.job?.adAdditionalDescription)
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

    if (!this.isJobAd) {
      this.adUpdateForm.get('position')?.clearValidators();
      this.adUpdateForm.get('biography')?.clearValidators();
      this.adUpdateForm.get('jobTypeId')?.clearValidators();
    }

    this.adUpdateForm.updateValueAndValidity();

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
      adEndDate: moment(now).add(data.adDuration, 'days'),
      adAdditionalDescription: data.adAdditionalDescription,
      adTitle: data.adTitle,
      price: data.price,
      pricingPlanName: this.job.pricingPlanName
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
      this.subscription = this.jobService.upsertJob(this.isEditMode, model).subscribe({
        next: (updatedJob) => {
          this.job = updatedJob;
          this.toastr.success('Uspješno ste uredili objavu!');
        },
        error: () => {
          this.toastr.error('Desila se greška prilikom uređivanja objave!');
        } 
      });
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
    this.jobService.deleteAd(this.job.id).subscribe({
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

  closeAd(event): void {
    event.preventDefault();
    this.jobService.closeAd(this.job.id).subscribe({
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

  reactivateAd(event): void {
    event.preventDefault();
    this.jobService.closeAd(this.job.id).subscribe({
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

  openFilePreview(): void {
    if (this.selectedFilePath) {
      this.dialog.open(this.filePreviewModal, {
        width: '80%',
        height: 'auto',
      });
    }
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
      this.adUpdateForm.patchValue({ cvFile: this.selectedFile });
      this.adUpdateForm.markAsDirty();
      // Clear input value to allow re-upload of the same file
      input.value = ''; 
    }
  }

  removeSelectedFile(event: Event): void {
    event.stopPropagation();
    this.selectedFileName = null;
    this.selectedFilePath = null;
    this.adUpdateForm.patchValue({cvFile: null});
    this.adUpdateForm.markAsDirty();
  }

  deleteExistingFile(event: Event) {
    event.stopPropagation();
    this.removeSelectedFile(event);
    this.job = { ...this.job, cvFilePath: null };
    this.existingFilePath = null;
    this.removeSelectedFile(event)
  }

  getEnumStatusValue(value: number): string {
    return value == JobPostStatus.Active ? "Aktivan Oglas" : value == JobPostStatus.Closed ? "Završen Oglas" : "Obrisan Oglas";
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
