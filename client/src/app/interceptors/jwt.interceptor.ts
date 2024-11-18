import { Injectable, OnDestroy } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable, Subject, take, takeUntil } from 'rxjs';
import { AccountService } from '../services/account.service';
import { User } from '../modal/user';

@Injectable()
export class JwtInterceptor implements HttpInterceptor, OnDestroy {
  private destroy$ = new Subject<void>();
  
  constructor(private accountService: AccountService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    let currentUser: User;
    this.accountService.user$.pipe(take(1)).pipe(takeUntil(this.destroy$)).subscribe((user) => {
      currentUser = user;
    });
    if (currentUser) {
      request = request.clone({
        setHeaders: { Authorization: 'Bearer ' + currentUser.token },
      });
    }
    return next.handle(request);
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
