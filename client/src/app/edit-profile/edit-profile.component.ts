import { Component, OnChanges, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, NgForm, Validators, FormGroup, FormArray, FormControl } from '@angular/forms';
import { finalize } from 'rxjs';
import { Address, LocationInfo } from '../modal/address';
import { AccountService } from '../services/account.service';
import { ToastrService } from '../services/toastr.service';
import { UtilityService } from '../services/utility.service';
import { Gender } from '../models/enums';
import { City } from '../models/location';
import { LocationService } from '../services/location.service';
import { JobCategory, JobType, UserEducation } from '../models/userJobPost';
import { JobService } from '../services/job.service';
import { MatDialog } from '@angular/material/dialog';
import { CancelConfirmationModalComponent } from '../modal/cancel-confirmation-modal/cancel-confirmation-modal.component';
import { UserProfile } from '../modal/user';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.css'],
})
export class EditProfileComponent implements OnInit {
  user: UserProfile;
  profileUpdate: UntypedFormGroup;
  genders = Object.values(Gender);
  jobTypes: JobType[] = [];
  jobCategories: JobCategory[] = [];
  loading: string;
  cities: City[] = [];
  selectedFile: File;
  //jobInfoForm: FormGroup;
  userEducations: FormArray;

  constructor(
    private fb: UntypedFormBuilder,
    public accountService: AccountService,
    private toastr: ToastrService,
    utility: UtilityService,
    private locationService: LocationService,
    private jobService: JobService,
    public dialog: MatDialog
  ) {
    utility.setTitle('Uredi profil');
  }

  ngOnInit(): void {
    this.loadCities();
    this.loadJobTypes();
    this.loadJobCategories();
    
    this.accountService.getProfile().subscribe((response: UserProfile) => {
      this.user = response;
      this.initilizeProfileForm();
      console.log("user");
      console.log(this.user);
      this.updateUserEducations(this.user?.userEducations);
      //this.jobInfoForm = this.initializeJobInfoForm();
    });
    // this.accountService.getAddress().subscribe((response) => {
    //   this.initilizeAddressForm(response);
    // });
  }

  private updateUserEducations(educations: any[]): void {
    console.log(educations);
    this.userEducations = this.profileUpdate.get('userEducations') as FormArray;
    console.log(this.userEducations);
    this.userEducations.clear();
  
    educations.forEach(education => {
      this.userEducations.push(this.createEducationFormGroup(education));
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

  initilizeProfileForm() {
    this.userEducations = this.fb.array([]);

    this.profileUpdate = this.fb.group({
      id: [this.user.id],
      firstName: [this.user.firstName, Validators.required],
      lastName: [this.user.lastName, Validators.required],
      userName: [
        this.user.userName,
        [Validators.required, Validators.minLength(3)],
        this.accountService.userExistAsync(false, this.user.userName),
      ],
      phoneNumber: [this.user.phoneNumber, Validators.required],
      email: [this.user.email, [Validators.email, Validators.required]],
      dateOfBirth: [new Date(this.user.dateOfBirth), Validators.required],
      gender: [this.user.gender, Validators.required],
      cityId: [this.user.cityId, Validators.required],
      jobTypeId: [this.user.jobTypeId],
      jobCategoryId: [this.user.jobCategoryId],
      userEducations: this.userEducations,
      position: [this.user.position],
      biography: [this.user.biography],
      cvFile: [null]
    });
  }

  initilizeAddressForm(response: Address) {
    // this.address = response;
    // this.areas = response.locations.areas;
    // this.cities = response.locations.cities;
    // this.states = response.locations.states;
  }

  loadCities(): void {
    this.locationService.getCities()
      .subscribe(cities => {
        this.cities = cities;
      });
  }

  onSelect(event, childType: string) {
  }

  onSubmit() {
    // const formData = new FormData();
    // Object.keys(this.profileUpdate.controls).forEach(key => {
    //   if (key !== 'cvFile') {
    //     formData.append(key, this.profileUpdate.get(key).value);
    //   }
    // });

    // if (this.selectedFile) {
    //   //this.profileUpdate.get('cvFile').setValue(this.selectedFile);
    //   formData.append('cvFile', this.selectedFile, this.selectedFile.name);
    // }
    const formData = new FormData();
    console.log("FORM ", this.profileUpdate.value);
    // Append form data fields
    Object.keys(this.profileUpdate.value).forEach(key => {
      if (key === 'userEducations') {
        const userEducations = this.profileUpdate.get('userEducations') as FormArray;
        userEducations.controls.forEach((educationGroup: FormGroup, index: number) => {
          Object.keys(educationGroup.value).forEach(controlKey => {
            formData.append(`userEducations[${index}].${controlKey}`, educationGroup.get(controlKey).value);
          });
        });
      }
      else if (key === 'dateOfBirth' && this.profileUpdate.get(key).value) {
        formData.append(key, new Date(this.profileUpdate.get(key).value).toISOString());
      } else {
        formData.append(key, this.profileUpdate.get(key).value);
      }
    });

    // Append cvFile if selected
    if (this.selectedFile) {
      formData.append('cvFile', this.selectedFile, this.selectedFile.name);
    }


    this.accountService
      .updateProfile(formData)
      .subscribe((response) => {
        console.log("Update user profile");
        console.log(response);
        this.user = response;
        this.initilizeProfileForm();
        this.updateUserEducations(this.user?.userEducations);
        this.toastr.success('Profile updated.');
      });
  }

  updateAddress(form: NgForm) {
  }

  openFile() {
    const filePath = this.user.cvFilePath;
    console.log("FILE PATH", filePath);
    // Ensure the path is a valid URL
    if (filePath) {
      window.open("C:/dev/web-shop/web-shop/API/uploads/EminDukic_CV.pdf", '_blank');
    } else {
      console.error('File path is not valid');
    }
  }

  changePhoto(files: FileList) {
    if (files.length > 0) {
      this.accountService.changePhoto(files.item(0)).subscribe((response) => {
        this.user.photoUrl = response.photoUrl;
        this.toastr.success('Photo updated.');
      });
    }
  }

  removePhoto() {
    this.accountService.removePhoto().subscribe(() => {
      this.user.photoUrl = null;
      this.toastr.success('Photo removed.');
    });
  }

  genderName(gender: Gender) {
    if(gender == Gender.Male){
      return "Muškarac";
    }
    else if(gender == Gender.Female){
      return "Žena";
    }
    else 
      return "Ostali";
  }

  createEducation(education?: UserEducation): FormGroup {
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
    this.userEducations.push(this.createEducation());
  }
  
  removeEducation(index: number): void {
    this.userEducations.removeAt(index);
  }

  openCancelConfirmationModal(index: number): void {
    const cancelDialogRef = this.dialog.open(CancelConfirmationModalComponent,
      {data: {
        title: "Obriši obrazovanje",
        message: "Da li ste sigurni da želite obrisati obrazovanje?"
      }});

    cancelDialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.profileUpdate.markAsDirty();
        this.removeEducation(index);
      }
    });
  }

  onFileSelected(event): void {
    this.selectedFile = (event.target as HTMLInputElement).files[0];
    this.profileUpdate.patchValue({ cvFile: this.selectedFile });
    this.profileUpdate.markAsDirty();
  }

}
