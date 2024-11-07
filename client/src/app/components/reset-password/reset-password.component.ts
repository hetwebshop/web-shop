import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css']
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup;
  redirectUrl: string;
  token: string = '';
  email: string = '';
  message: string | null = null

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
    private utility: UtilityService
  ) {
    utility.setTitle('Login');
  }

  ngOnInit(): void {
    this.initializeForm();
    this.route.queryParams.subscribe(params => {
      this.token = encodeURIComponent(params['token']);
      this.email = decodeURIComponent(params['email']);
    });
  }

  initializeForm() {
    this.resetPasswordForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('password')],
      ],
    });
  }

  matchValues(name: string): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      return control?.value === control?.parent?.controls[name]?.value
        ? null
        : { notMatching: true };
    };
  }

  onSubmit() {
    if (this.resetPasswordForm.valid) {
      const { password, confirmPassword } = this.resetPasswordForm.value;
      this.accountService.resetPassword(this.email, this.token, password, confirmPassword).subscribe({
        next: () => {
          this.message = 'Lozinka uspješno promijenjena.';
          this.router.navigate(['/login']);
        },
        error: (error) => {
          this.message = 'Link za promjenu lozinke neispravan ili je već iskorišten.';
        }
      });
    }
  }

  redirect() {
    this.router.navigateByUrl(this.redirectUrl ? this.redirectUrl : '/', {
      replaceUrl: true,
    });
  }
}
