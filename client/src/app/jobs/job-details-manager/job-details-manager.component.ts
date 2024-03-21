import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, map, switchMap } from 'rxjs';
import { AdvertisementType, ApplicantEducation, JobCategory, JobType, UserJobPost, UserJobSubcategory } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { UtilityService } from 'src/app/services/utility.service';
import { FormBuilder, FormGroup, Validators, FormControl, FormArray } from '@angular/forms';
import { LocationService } from 'src/app/services/location.service';
import { City, Country } from 'src/app/models/location';
import { AdvertisementTypeEnum, Gender, JobPostStatus } from 'src/app/models/enums';
import { MatOption } from '@angular/material/core';

@Component({
  selector: 'app-job-details-manager',
  templateUrl: './job-details-manager.component.html',
  styleUrls: ['./job-details-manager.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class JobDetailsManagerComponent implements OnInit, OnDestroy {
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
  genders = Object.values(Gender);
  selectedAdvertisementType: any;
  AdvertisementTypeEnum = AdvertisementTypeEnum;
  selectAllId: number = 0;

  @ViewChild('allSelected', { static: true }) private allSelected: MatOption;

  constructor(private jobService: JobService, private locationService: LocationService, utility: UtilityService, 
    private route: ActivatedRoute, private cdr: ChangeDetectorRef, 
    private fb: FormBuilder) {
    utility.setTitle('Job Details');
    this.form = this.createForm();
  }

  ngOnInit(): void {
    this.loadCountries();
    this.loadCities();
    this.loadJobTypes();
    this.loadJobCategories();
    this.loadAdTypes();

    this.trackCountryChanges();
    this.trackCityChanges();

    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.jobId = +params['id'];
        this.subscription = this.jobService.getJobById(this.jobId)
          .subscribe({
            next: (response) => {
              this.job = response;
              console.log("job cat" + JSON.stringify(this.job));
              this.updateApplicantEducations(this.job?.applicantEducations);
              this.onCategoryChange(this.job.jobCategoryId);
              const categoryIds = this.job.userJobSubcategories.map(value => value.jobCategoryId);
              if(categoryIds.length == this.selectedCategory?.subcategories.length){
                categoryIds.push(this.selectAllId);
              }
              this.form.patchValue({
                id: this.job.id,
                position: this.job.position,
                biography: this.job.biography,
                //skills: this.job.skills,
                applicantFirstName: this.job.applicantFirstName,
                applicantLastName: this.job.applicantLastName,
                applicantEmail: this.job.applicantEmail,
                applicantDateOfBirth: this.job.applicantDateOfBirth,
                applicantPhoneNumber: this.job.applicantPhoneNumber,
                applicantGender: this.job.applicantGender,
                jobTypeId: this.job.jobTypeId,
                jobCategoryId: this.job.jobCategoryId,
                cityId: this.job.cityId,
                countryId: this.job.countryId,
                price: this.job.price,
                jobSubcategories: categoryIds
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
    this.applicantEducations = this.fb.array([]);

    return this.fb.group({
      id: new FormControl(0),
      position: new FormControl('', Validators.required),
      biography: new FormControl(''),
      //skills: new FormControl(''),
      applicantFirstName: new FormControl('', Validators.required),
      applicantLastName: new FormControl('', Validators.required),
      applicantEmail: new FormControl('', [Validators.required, Validators.email]),
      applicantPhoneNumber: new FormControl('', Validators.required),
      applicantGender: new FormControl('', Validators.required),
      applicantDateOfBirth: new FormControl('', Validators.required),
      applicantEducations: this.applicantEducations,
      jobTypeId: new FormControl(0, Validators.required),
      cityId: new FormControl('', Validators.required),
      countryId: new FormControl('', Validators.required),
      jobCategoryId: new FormControl('', Validators.required),
      jobSubcategories: new FormControl({value: [[]], disabled: true}, Validators.required),
      advertisementTypeId: new FormControl(AdvertisementTypeEnum.JobAd, Validators.required),
      price: new FormControl('')
    });
  }

  prepareModel(formData: any) : UserJobPost {
    const jobSubcategories: UserJobSubcategory[] = [];
    const applicantEducations: ApplicantEducation[] = [];
    formData.jobSubcategories.forEach(catId => {
      if(catId != this.selectAllId){
        const jobSubCat: UserJobSubcategory = {
          //userJobPostId: 0,
          jobCategoryId: catId
        };
        jobSubcategories.push(jobSubCat);
      }
    });

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
    
    const model: UserJobPost = {
      id: formData.id,
      position: formData.position || '',
      biography: formData.biography || '',
      //skills: formData.skills || '',
      applicantFirstName: formData.applicantFirstName,
      applicantLastName: formData.applicantLastName,
      applicantEmail: formData.applicantEmail,
      applicantPhoneNumber: formData.applicantPhoneNumber,
      applicantGender: formData.applicantGender,
      applicantDateOfBirth: formData.applicantDateOfBirth,
      applicantEducations: applicantEducations,
      jobTypeId: formData.jobTypeId,
      cityId: formData.cityId,
      countryId: formData.countryId,
      jobCategoryId: formData.jobCategoryId,
      jobPostStatusId: JobPostStatus.Active,
      userJobSubcategories: jobSubcategories,
      advertisementTypeId: formData.advertisementTypeId
    };
  
    return model;
  }

private updateApplicantEducations(educations: any[]): void {
  this.applicantEducations = this.form.get('applicantEducations') as FormArray;

  this.applicantEducations.clear();

  educations.forEach(education => {
    this.applicantEducations.push(this.createEducationFormGroup(education));
  });
}

private createEducationFormGroup(education: any): FormGroup {
  return this.fb.group({
      degree: new FormControl(education.degree, Validators.required),
      institutionName: new FormControl(education.institutionName, Validators.required),
      fieldOfStudy: new FormControl(education.fieldOfStudy, Validators.required),
      educationStartYear: new FormControl(education.educationStartYear, Validators.required),
      educationEndYear: new FormControl(education.educationEndYear),
      university: new FormControl(education.university, Validators.required)
  });
}

  onCategoryChange(categoryId: number): void {
    this.selectedCategory = this.jobCategories.find(category => category.id === categoryId);
    this.form.get('jobSubcategories').setValue([]);
    if(this.selectedCategory){
      this.form.get('jobSubcategories').enable();
    }
  }

  onAdTypeChange(adTypeId: number): void {
    this.selectedAdvertisementType = adTypeId;
  }

  onSubmit(): void {
    console.log(this.form.valid);
    if (this.form.valid) {
      const formData = this.form.value;
      const model = this.prepareModel(formData);
      this.subscription = this.jobService.upsertJob(this.isEditMode, model).subscribe();
    } 
    // else {
    //   Object.keys(this.form.controls).forEach(field => {
    //     const control = this.form.get(field);
    //     if (control.invalid) {
    //       control.markAsTouched();
    //     }
    //   });
    // }
  }

  loadCountries(): void {
    this.locationService.getCountries()
      .subscribe(countries => {
        this.countries = countries;
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

  loadAdTypes(): void {
    this.jobService.getAdTypes()
      .subscribe(adTypes => {
        this.advertisementTypes = adTypes;
      });
  }

  trackCountryChanges(): void {

  }

  trackCityChanges(): void {
    this.form.get('cityId').valueChanges.subscribe(cityId => {
      if (cityId) {
        const selectedCity = this.cities.find(city => city.id === cityId);
        if (selectedCity) {
          this.form.get('countryId').setValue(selectedCity.countryId);
        }
      }
    });
  }

  filterCitiesByCountry(countryId: number): void {
    this.cities = this.cities.filter(city => city.countryId == countryId);
  }

  filterSubcategories(selectedCategoryId: number): JobCategory[] {
    const filteredSubcategories: JobCategory[] = this.jobCategories.filter(category => category.parentId === selectedCategoryId);
    filteredSubcategories.forEach(subcategory => {
      subcategory.subcategories = this.filterSubcategories(subcategory.id);
    });
    return filteredSubcategories;
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

  tosslePerOne(): boolean { 
    if (this.allSelected.selected) {  
     this.allSelected.deselect();
     return false;
    }
    const control = this.form.get('jobSubcategories');
    if(control.value.length==this.selectedCategory?.subcategories.length)
      this.allSelected.select();
  }

  toggleAllSelection(): void {
    const control = this.form.get('jobSubcategories');
    console.log("all selec");
    if (this.allSelected.selected) {
      console.log("all selec IF");
      const allSubcategoryIds = this.selectedCategory?.subcategories.map(subcategory => subcategory.id);
      control.patchValue([...allSubcategoryIds, this.selectAllId]);
    } else {
      control.patchValue([]);
    }
  }
}
