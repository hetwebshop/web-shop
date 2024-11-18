import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject, Subscription, takeUntil } from 'rxjs';
import { AccountService } from 'src/app/services/account.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup;
  redirectUrl: string;
  private destroy$ = new Subject<void>();

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
    this.redirectUrl = this.route.snapshot.queryParams.redirectTo;

    if (this.accountService.loggedIn) {
      this.redirect();
    }
  }

  initializeForm() {
    this.loginForm = this.fb.group({
      userNameOrEmail: ['', [Validators.email, Validators.required]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.accountService.login(this.loginForm.value).pipe(
        takeUntil(this.destroy$)
      ).subscribe(() => {
        location.reload();
      });
    }
  }

  loginAsTester() {
    this.accountService
      .login({ username: 'test_user', password: 'notpassword' })
      .subscribe(() => {
        location.reload();
      });
  }

  redirect() {
    this.router.navigateByUrl(this.redirectUrl ? this.redirectUrl : '/', {
      replaceUrl: true,
    });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
