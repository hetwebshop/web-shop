import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AdminService } from 'src/app/services/admin.service';
import { ToastrService } from 'src/app/services/toastr.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-admin-roles',
  templateUrl: './admin-roles.component.html',
  styleUrls: ['./admin-roles.component.css'],
})
export class AdminRolesComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  roles = [
    { name: 'Any (Default)', value: null },
    { name: 'Admin', value: 'Admin' },
    { name: 'TrackModerator', value: 'TrackModerator' },
    { name: 'StoreModerator', value: 'StoreModerator' },
  ];
  step = 0;

  constructor(
    private adminService: AdminService,
    private route: ActivatedRoute,
    private router: Router,
    private toastrService: ToastrService,
    utility: UtilityService
  ) {
    utility.setTitle('Admin Roles');
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe((params) => {
    });
  }

  ngOnInit(): void {}

  apply() {
  }

  pageChange(page: number) {
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
