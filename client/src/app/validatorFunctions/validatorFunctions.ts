import { AbstractControl, ValidationErrors } from '@angular/forms';

export function toDateGreaterThanFromDate(control: AbstractControl): ValidationErrors | null {
  const fromDate = control.get('fromDate');
  const toDate = control.get('toDate');

  if (fromDate && toDate && fromDate.value && toDate.value && toDate.value >= fromDate.value) {
    return { toDateGreaterThanFromDate: true };
  }
  
  return null;
}
