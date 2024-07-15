import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { ActivatedRoute, Router } from '@angular/router';
import { User } from '../modal/user';
import { AccountService } from '../services/account.service';
import { AdvertisementTypeEnum } from '../models/enums';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  value: string;
  @Input() isDark = false;
  @Output() changeTheme = new EventEmitter<boolean>();
  user: User;
  AdvertisementTypeEnum = AdvertisementTypeEnum;
  
  constructor(
    public accountService: AccountService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.accountService.user$.subscribe((u) => (this.user = u));

    console.log("USER US ", this.accountService.user$);
  }

  @ViewChild('sidenav') sidenav: MatSidenav;

  ngOnInit(): void {
    this.route.queryParamMap.subscribe((params) => {
      this.value = params.get('q');
    });

    this.router.events.subscribe((event) => {
      this.sidenav?.close();
    });
  }

  toggleTheme(): void {
    this.isDark = !this.isDark;
    this.changeTheme.emit(this.isDark);
  }

  logout() {
    this.router.navigate(['/'], { queryParams: { logout: true } });
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
}
