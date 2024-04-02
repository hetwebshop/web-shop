import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { map, Observable } from 'rxjs';
import { AccountService } from 'src/app/services/account.service';
import { ToastrService } from 'src/app/services/toastr.service';

@Injectable({
  providedIn: 'root',
})
export class CreateAdAuthGard implements CanActivate {
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private router: Router
  ) {}
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.accountService.user$.pipe(
      map((user) => {
        if (!user) {
          this.toastr.error('You are not allowed');
          this.router.navigate(['login']);
          return false;
        }
        if (
          route.data.roles &&
          user.roles.some((r) => route.data.roles.includes(r))
        ) {
          return true;
        }
        this.toastr.error('You are not allowed');
        this.router.navigate(['login']);
        return false;
      })
    );
  }
}
