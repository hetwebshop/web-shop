import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { ToastrService } from '../services/toastr.service';
import { UtilityService } from '../services/utility.service';
import { City } from '../models/location';
import { LocationService } from '../services/location.service';
import { JobService } from '../services/job.service';
import { MatDialog } from '@angular/material/dialog';
import { UserProfile } from '../modal/user';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-edit-company-profile',
  templateUrl: './edit-company-profile.component.html',
  styleUrls: ['./edit-company-profile.component.css'],
})
export class EditCompanyProfileComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  company: UserProfile;
  companyProfileUpdate: UntypedFormGroup;
  loading: string;
  cities: City[] = [];

  filteredCities: City[] = [];
  citieSearchKeyword = "";

  constructor(
    private fb: UntypedFormBuilder,
    public accountService: AccountService,
    private toastr: ToastrService,
    utility: UtilityService,
    private locationService: LocationService,
    private jobService: JobService,
    public dialog: MatDialog
  ) {
    utility.setTitle('Uredi kompaniju');
  }

  ngOnInit(): void {
    this.loadCities();
    
    this.accountService.getProfile().pipe(takeUntil(this.destroy$)).subscribe((response: UserProfile) => {
      console.log(response);
      this.company = response;
      this.initilizeProfileForm();
      console.log("company");
      console.log(this.company);
    });
  }


  initilizeProfileForm() {
    this.companyProfileUpdate = this.fb.group({
      id: [this.company.id],
      companyName: [this.company.companyName, Validators.required],
      userName: [
        this.company.userName,
        [Validators.required, Validators.minLength(3)],
        this.accountService.userExistAsync(false, this.company.userName),
      ],
      companyPhone: [this.company.phoneNumber, Validators.required],
      aboutCompany: [this.company.aboutCompany],
      companyAddress: [this.company.companyAddress],
      email: [this.company.email, [Validators.email, Validators.required]],
      cityId: [this.company.cityId, Validators.required],
    });
  }

  loadCities(): void {
    this.locationService.getCities().pipe(takeUntil(this.destroy$))
      .subscribe(cities => {
        this.cities = cities;
        this.filteredCities = cities;
      });
  }

  onSubmit() {
    this.accountService
      .updateCompanyProfile(this.companyProfileUpdate.value).pipe(takeUntil(this.destroy$))
      .subscribe({
        next:(response) => {
          this.company = response;
          this.initilizeProfileForm();
          this.toastr.success('Uspješno ste uredili profil kompanije!');
        },
        error: () => {
          this.toastr.error('Desila se greška prilikom uređivanja profila kompanije!');
        }
      });
  }

  changePhoto(files: FileList) {
    if (files.length > 0) {
      this.accountService.changePhoto(files.item(0)).pipe(takeUntil(this.destroy$)).subscribe((response) => {
        this.company.photoUrl = response.photoUrl;
        this.toastr.success('Photo updated.');
      });
    }
  }

  removePhoto() {
    this.accountService.removePhoto().pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.company.photoUrl = null;
      this.toastr.success('Photo removed.');
    });
  }

  onKey(searchValue: string, formControlName: string) {
    if(formControlName == "cityId"){
      this.citieSearchKeyword = searchValue;
      this.filteredCities = this.cities.filter(city =>
        city.name.toLowerCase().includes(searchValue.toLowerCase())
      );
    } 
  }

  resetSearch(formControlName: string): void {
    this.citieSearchKeyword = "";
   
    if(formControlName == "cityId"){
      this.filteredCities = [...this.cities];
    }
  }
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
