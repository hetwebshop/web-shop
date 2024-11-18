import { Component, OnChanges, OnInit, TemplateRef, ViewChild } from '@angular/core';
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
import * as moment from 'moment';
import { Router } from '@angular/router';
import { MessageService } from '../services/message.service';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.css'],
})
export class EditProfileComponent implements OnInit {
  user: UserProfile;
  profileUpdate: UntypedFormGroup;
  genders = Object.keys(Gender) as Array<keyof typeof Gender>;
  genderMap = Gender;
  jobTypes: JobType[] = [];
  jobCategories: JobCategory[] = [];
  loading: string;
  cities: City[] = [];
  selectedFile: File;
  selectedFileName: string | null = null;
  selectedFilePath: string | null = null;
  existingFilePath: string | null = null;
  @ViewChild('filePreviewModal') filePreviewModal!: TemplateRef<any>;
  //jobInfoForm: FormGroup;
  userEducations: FormArray;

  filteredCities = []; 
  filteredCategories = []; 
  filteredJobTypes = []; 
  citieSearchKeyword = "";
  jobTypesSearchKeyword = "";
  jobCategoriesSearchKeyword = "";

  constructor(
    private fb: UntypedFormBuilder,
    public accountService: AccountService,
    private toastr: ToastrService,
    utility: UtilityService,
    private locationService: LocationService,
    private jobService: JobService,
    public dialog: MatDialog,
    private router: Router,
    private messageService: MessageService
  ) {
    utility.setTitle('Uredi profil');
  }

  ngOnInit(): void {
    const messageData = this.messageService.getMessage();
    if (messageData?.message) {
      if (messageData.isSuccessResponse) {
        this.toastr.success(messageData.message);  // Show success message
      } else {
        this.toastr.error(messageData.message);  // Show error message
      }
      this.messageService.clearMessage();  // Clear the message after showing
    }
    this.loadCities();
    this.loadJobTypes();
    this.loadJobCategories();

    this.accountService.getProfile().subscribe((response: UserProfile) => {
      this.user = response;
      this.initilizeProfileForm();
      console.log("user");
      console.log(this.user);
      this.updateUserEducations(this.user?.userEducations);
      this.existingFilePath = this.user.cvFilePath;
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
        this.jobTypes = types;
        this.filteredJobTypes = types;
      });
  }


  loadJobCategories(): void {
    this.jobService.getJobCategories()
      .subscribe(categories => {
        this.jobCategories = categories.filter(r => r.parentId == null);
        this.filteredCategories = this.jobCategories;
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
        this.filteredCities = cities;
      });
  }

  onSelect(event, childType: string) {
  }

  onSubmit() {
    const formData = new FormData();
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
        const dateOfBirth = moment(this.profileUpdate.get(key).value).format('YYYY-MM-DD');
        formData.append(key, dateOfBirth);
      }
      else {
        formData.append(key, this.profileUpdate.get(key).value);
      }
    });

    // Append cvFile if selected
    if (this.selectedFile) {
      formData.append('cvFile', this.selectedFile, this.selectedFile.name);
    }
    this.accountService.updateProfile(formData).subscribe({
      next: (response) => {
        this.user = response;
        this.initilizeProfileForm();
        this.updateUserEducations(this.user?.userEducations);
        this.toastr.success('Uspješno ste uredili profil!');
      },
      error: () => {
        this.toastr.error('Desila se greška prilikom uređivanja profila!');
      }
    });
  }

  updateAddress(form: NgForm) {
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
      this.profileUpdate.patchValue({ cvFile: this.selectedFile });
      this.profileUpdate.markAsDirty();
      // Clear input value to allow re-upload of the same file
      input.value = '';
    }
  }

  removeSelectedFile(event: Event): void {
    event.stopPropagation();
    this.selectedFileName = null;
    this.selectedFilePath = null;
    this.profileUpdate.patchValue({ cvFile: null });
    this.user.cvFilePath = null;
    this.profileUpdate.markAsDirty();
  }

  deleteExistingFile(event: Event) {
    event.stopPropagation();
    this.existingFilePath = null;
    this.removeSelectedFile(event);
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

  genderName(gender: Gender): string {
    return this.genderMap[gender] || 'Ostali';
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
      {
        data: {
          title: "Obriši obrazovanje",
          message: "Da li ste sigurni da želite obrisati obrazovanje?"
        }
      });

    cancelDialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.profileUpdate.markAsDirty();
        this.removeEducation(index);
      }
    });
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
