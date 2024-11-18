import { Component } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';
import { MessageService } from 'src/app/services/message.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent {
  resetPasswordForm: FormGroup;
  message: string | null = null
  isCompany: boolean = false;

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
    private utility: UtilityService,
    private messageService: MessageService
  ) {
    utility.setTitle('Promijena lozinke');
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.isCompany = params['company'] === 'true';  // Check if company=true
    });
    this.initializeForm();
  }

  initializeForm() {
    this.resetPasswordForm = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: [
        '',
        [Validators.required, this.matchValues('newPassword')],
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
      const { currentPassword, newPassword, confirmPassword } = this.resetPasswordForm.value;
      this.accountService.changePassword(currentPassword, newPassword, confirmPassword).subscribe({
        next: () => {
          this.messageService.setMessage('Lozinka uspješno promijenjena.', true);
          this.isCompany ? this.router.navigate(['/edit-company-profile']) : this.router.navigate(['/edit-profile']);
        },
        error: () => {
          this.messageService.setMessage('Desila se greška prilikom promjene lozinke.', false);
        }
      });
    }
  }
}
