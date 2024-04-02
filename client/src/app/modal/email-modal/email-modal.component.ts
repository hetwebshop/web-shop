import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { CancelConfirmationModalComponent } from '../cancel-confirmation-modal/cancel-confirmation-modal.component';

@Component({
  selector: 'app-email-modal',
  templateUrl: './email-modal.component.html',
  styleUrls: ['./email-modal.component.css']
})
export class EmailModalComponent {
  fromEmail: string;
  toEmail: string; 
  message: string; 
  title: string;
  
  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private dialog: MatDialog, public dialogRef: MatDialogRef<EmailModalComponent>) {
    this.fromEmail = data.fromEmail;
    this.toEmail = data.toEmail;
  }

  sendEmail() {
    // Logic to send email
    // You can access form values here
    // Close the dialog after sending email
    console.log(this.fromEmail);
    console.log(this.toEmail);
    console.log(this.message);

    this.dialogRef.close();
  }

  openCancelConfirmationModal(): void {
    if(this.message || this.title){
      const cancelDialogRef = this.dialog.open(CancelConfirmationModalComponent, {
        data: {
          title: "Email",
          message: "Da li ste sigurni da želite odustati od slanja email-a ?"
        }
      });

      cancelDialogRef.afterClosed().subscribe(result => {
        if (result === true) {
          this.dialogRef.close();
          // Perform cancellation action here
          console.log('Changes canceled');
        }
      });
    }
    else {
      this.dialogRef.close();
    }
  }

  
}
