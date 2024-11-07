import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from 'src/app/services/account.service';

@Component({
  selector: 'app-email-confirmation',
  templateUrl: './email-confirmation.component.html',
  styleUrls: ['./email-confirmation.component.css']
})
export class EmailConfirmationComponent implements OnInit {
  loading: boolean = true;
  successMessage: string = '';
  errorMessage: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private accountService: AccountService // Service to interact with the backend
  ) {}

  ngOnInit(): void {
    // Extract userId and token from URL query parameters
    this.route.queryParams.subscribe(params => {
      const userId = params['userId'];
     const token = encodeURIComponent(params['token']);

      if (userId && token) {
        this.confirmEmail(userId, token);
      } else {
        this.errorMessage = 'Invalid confirmation link.';
        this.loading = false;
      }
    });
  }

  confirmEmail(userId: string, token: string): void {
    // Call backend API to confirm the email
    this.accountService.confirmEmail(userId, token).subscribe(
      response => {
        // Handle success
        this.successMessage = 'Email uspješno potvrđen!';
        this.loading = false;
      },
      error => {
        // Handle error
        this.errorMessage = error.error || 'Desila se greška prilikom potvrđivanja email adrese.';
        this.loading = false;
      }
    );
  }

  // Redirect the user to the login page after confirmation
  redirectToLogin(): void {
    this.router.navigate(['/login']);
  }
}
