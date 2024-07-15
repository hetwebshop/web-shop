import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { CancelConfirmationModalComponent } from '../cancel-confirmation-modal/cancel-confirmation-modal.component';

@Component({
  selector: 'app-submit-application-modal',
  templateUrl: './submit-application-modal.component.html',
  styleUrls: ['./submit-application-modal.component.css']
})
export class SubmitApplicationModalComponent {
  fromEmail: string;
  toEmail: string; 
  coverLetter: string; 
  title: string;
  cvFile: File | null = null;
  
  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private dialog: MatDialog, public dialogRef: MatDialogRef<SubmitApplicationModalComponent>) {
    this.fromEmail = data.fromEmail;
    this.toEmail = data.toEmail;
    this.title = "Prijava na oglas za posao";
  }

  onFileChange(event: any) {
    if (event.target.files.length > 0) {
      this.cvFile = event.target.files[0];
    }
  }

  submitApplication() {
    // Logic to send email
    // You can access form values here
    // Close the dialog after sending email
    console.log(this.fromEmail);
    console.log(this.toEmail);
    console.log(this.coverLetter);

    this.dialogRef.close();
  }

  openCancelConfirmationModal(): void {
    if(this.coverLetter || this.title){
      const cancelDialogRef = this.dialog.open(CancelConfirmationModalComponent, {
        data: {
          title: "Otkaži aplikaciju",
          message: "Da li ste sigurni da želite odustati od apliciranja na ovaj oglas ?"
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
