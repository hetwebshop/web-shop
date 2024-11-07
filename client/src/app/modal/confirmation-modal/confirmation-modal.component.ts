import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-confirmation-modal',
  templateUrl: './confirmation-modal.component.html',
  styleUrls: ['./confirmation-modal.component.css']
})
export class ConfirmationModalComponent {
  title: string;
  message: string;
  constructor(@Inject(MAT_DIALOG_DATA) public data: any, public dialogRef: MatDialogRef<ConfirmationModalComponent>) {
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
