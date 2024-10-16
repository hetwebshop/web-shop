import { Component, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { AccountService } from '../services/account.service';
import { ToastrService } from '../services/toastr.service';
import { UtilityService } from '../services/utility.service';
import { City } from '../models/location';
import { LocationService } from '../services/location.service';
import { JobService } from '../services/job.service';
import { MatDialog } from '@angular/material/dialog';
import { UserProfile } from '../modal/user';

@Component({
  selector: 'app-edit-company-profile',
  templateUrl: './edit-company-profile.component.html',
  styleUrls: ['./edit-company-profile.component.css'],
})
export class EditCompanyProfileComponent implements OnInit {
  company: UserProfile;
  companyProfileUpdate: UntypedFormGroup;
  loading: string;
  cities: City[] = [];

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
    console.log("TEST KOMP");
    this.loadCities();
    
    this.accountService.getProfile().subscribe((response: UserProfile) => {
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
    this.locationService.getCities()
      .subscribe(cities => {
        this.cities = cities;
      });
  }

  onSubmit() {
    this.accountService
      .updateCompanyProfile(this.companyProfileUpdate.value)
      .subscribe((response) => {
        console.log("Update user profile");
        console.log(response);
        this.company = response;
        this.initilizeProfileForm();
        this.toastr.success('Profile updated.');
      });
  }

  changePhoto(files: FileList) {
    if (files.length > 0) {
      this.accountService.changePhoto(files.item(0)).subscribe((response) => {
        this.company.photoUrl = response.photoUrl;
        this.toastr.success('Photo updated.');
      });
    }
  }

  removePhoto() {
    this.accountService.removePhoto().subscribe(() => {
      this.company.photoUrl = null;
      this.toastr.success('Photo removed.');
    });
  }

}
