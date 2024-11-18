import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AccountService } from 'src/app/services/account.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  forgotPasswordForm: FormGroup;
  redirectUrl: string;
  message: string | null;

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
  }

  initializeForm() {
    this.forgotPasswordForm = this.fb.group({
      userNameOrEmail: ['', [Validators.email, Validators.required]],
    });
  }

  onSubmit() {
    if (this.forgotPasswordForm.valid) {
      const email = this.forgotPasswordForm.get('userNameOrEmail').value;
      this.accountService.forgotPassword(email).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => this.message = 'Link za promjenu lozinke će vam biti poslan na unijetu email adresu.',
        error: () => this.message = 'Desila se greška, molimo vas da pokušate ponovo.'
      });
    }
  }
  ngOnDestroy() {
    // Trigger cleanup of all subscriptions
    this.destroy$.next();
    this.destroy$.complete();
  }
}
