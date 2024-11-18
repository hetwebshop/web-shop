import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../modal/user';
import { AccountService } from '../services/account.service';
import { AdvertisementTypeEnum } from '../models/enums';
import { Observable, Subject, map, shareReplay, takeUntil } from 'rxjs';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { ToastrService } from '../services/toastr.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  value: string;
  @Input() isDark = false;
  @Output() changeTheme = new EventEmitter<boolean>();
  user: User;
  AdvertisementTypeEnum = AdvertisementTypeEnum;
  isLargeScreen: boolean = false;

  constructor(
    public accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private breakpointObserver: BreakpointObserver
  ) {
    this.accountService.user$.pipe(takeUntil(this.destroy$)).subscribe((u) => (this.user = u));
  }

  @ViewChild('sidenav') sidenav: MatSidenav;

  ngOnInit(): void {
    this.breakpointObserver.observe([
      "(min-width: 1400px)"
    ]).pipe(takeUntil(this.destroy$)).subscribe(result => {
      if(result.matches)
      {
          this.isLargeScreen = true;
          if(this.sidenav?.opened)
            this.sidenav.close();
      }
      else 
        this.isLargeScreen = false;
    });
    this.route.queryParamMap.pipe(takeUntil(this.destroy$)).subscribe((params) => {
      this.value = params.get('q');
    });

    this.router.events.pipe(takeUntil(this.destroy$)).subscribe((event) => {
      this.sidenav?.close();
    });
  }

  toggleTheme(): void {
    this.isDark = !this.isDark;
    this.changeTheme.emit(this.isDark);
  }

  isActive(route: string): boolean {
    return this.router.url.includes(route);
  }

  logout() {
    this.router.navigate(['/login'], { queryParams: { logout: true } });
  }

  // search() {
  //   if (this.isValid()) {
  //     this.router.navigate(['/search'], {
  //       queryParams: { q: this.value.trim() },
  //     });
  //   }
  // }

  isValid() {
    return this.value && this.value.trim();
  }

  getEnumName(value: number): string {
    return AdvertisementTypeEnum[value];
  }

  
  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
