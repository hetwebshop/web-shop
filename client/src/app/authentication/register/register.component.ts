import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  UntypedFormBuilder,
  UntypedFormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Gender } from 'src/app/models/enums';
import { City } from 'src/app/models/location';
import { AccountService } from 'src/app/services/account.service';
import { LocationService } from 'src/app/services/location.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  registerForm: UntypedFormGroup;
  registerCompanyForm: UntypedFormGroup;
  redirectUrl: string;
  cities: City[];
  genders = Object.values(Gender);
  registrationType: string = 'user';
  loginRegistrationError: string | null = null;

  constructor(
    private fb: UntypedFormBuilder,
    private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
    utility: UtilityService,
    private locationService: LocationService
  ) {
    utility.setTitle('Kreiranje novog računa');
    this.initilizeForms();
  }

  ngOnInit(): void {
    this.loadCities();
    this.redirectUrl = this.route.snapshot.queryParams.redirectTo;
    if (this.accountService.loggedIn) {
      this.redirect();
    }
  }

  toggleRegistrationType(type: string): void {
    this.registrationType = type;
  }

  loadCities(): void {
    this.locationService.getCities()
      .subscribe(cities => {
        this.cities = cities;
      });
  }

  initilizeForms() {
    this.registerForm = this.fb.group({
      firstName: [
        '',
        [Validators.required],
      ],
      lastName: [
        '',
        [Validators.required],
      ],
      phoneNumber: ['', Validators.required],
      email: ['', [Validators.email, Validators.required]],
      dateOfBirth: ['', Validators.required],
      gender: [, Validators.required],
      cityId: [null, Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(12),
        ],
      ],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });

    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    });

    this.registerCompanyForm = this.fb.group({
      companyName: [
        '',
        [Validators.required],
      ],
      phoneNumber: ['', Validators.required],
      email: ['', [Validators.email, Validators.required]],
      cityId: [null, Validators.required],
      address: [null, Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(12),
        ],
      ],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });

    this.registerCompanyForm.controls.password.valueChanges.subscribe(() => {
      this.registerCompanyForm.controls.confirmPassword.updateValueAndValidity();
    });
  }

  matchValues(name: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      return control?.value === control?.parent?.controls[name]?.value
        ? null
        : { notMatching: true };
    };
  }

  redirect() {
    this.router.navigateByUrl(this.redirectUrl ? this.redirectUrl : '/', {
      replaceUrl: true,
    });
  }

  onSubmit() {
    const formData = this.registerForm.value;
    formData.gender = Gender[formData.gender];
    this.accountService.register(formData).subscribe({
      next: (response) => {
        if (response) {
          this.router.navigate(['/login']); 
        }
      },
      error: (error) => {
        console.error('Registration failed', error);
        this.loginRegistrationError = 'Greška prilikom registracije. Molimo pokušajte ponovo.';
      },
    });
  }

  onSubmitCompany() {
    this.accountService.registerCompany(this.registerCompanyForm.value).subscribe({
      next: (response) => {
        if (response) {
          this.router.navigate(['/login']); 
        }
      },
      error: (error) => {
        console.error('Registration failed', error);
        this.loginRegistrationError = 'Greška prilikom registracije. Molimo pokušajte ponovo.';
      },
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
}
