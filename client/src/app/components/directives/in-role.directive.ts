import {
  Directive,
  Input,
  OnDestroy,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { User } from 'src/app/modal/user';
import { AccountService } from 'src/app/services/account.service';

@Directive({
  selector: '[appInRole]',
})
export class InRoleDirective implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  @Input() appInRole: string[];
  user: User;
  constructor(
    private templateRef: TemplateRef<any>,
    private vcRef: ViewContainerRef,
    private accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.accountService.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
      this.user = user;
      this.createView();
    });
  }

  createView() {
    if (this.user == null || !this.user.roles) {
      this.vcRef.clear();
    } else if (this.user.roles.some((r) => this.appInRole.includes(r))) {
      this.vcRef.createEmbeddedView(this.templateRef);
    } else {
      this.vcRef.clear();
    }
  }

  ngOnDestroy() {
    // Trigger cleanup of all subscriptions
    this.destroy$.next();
    this.destroy$.complete();
  }
}
