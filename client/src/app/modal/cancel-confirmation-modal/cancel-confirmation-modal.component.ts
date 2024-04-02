import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-cancel-confirmation-modal',
  templateUrl: './cancel-confirmation-modal.component.html',
  styleUrls: ['./cancel-confirmation-modal.component.css']
})
export class CancelConfirmationModalComponent {
  title: string;
  message: string;
  constructor(@Inject(MAT_DIALOG_DATA) public data: any, public dialogRef: MatDialogRef<CancelConfirmationModalComponent>) {
    this.title = data.title;
    this.message = data.message;
  }

  onCancel(): void {
    this.dialogRef.close(true);
  }

  onClose(): void {
    this.dialogRef.close(false);
  }
}
