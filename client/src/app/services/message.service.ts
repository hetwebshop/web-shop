// message.service.ts
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  private message: string | null = null;
  private isSuccessResponse: boolean | null = null;

  setMessage(message: string, isSuccessResponse: boolean): void {
    this.message = message;
    this.isSuccessResponse = isSuccessResponse;
  }

  getMessage() {
    return { message: this.message, isSuccessResponse: this.isSuccessResponse};
  }

  clearMessage(): void {
    this.message = null;
    this.isSuccessResponse = false;
  }
}
