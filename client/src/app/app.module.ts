import { Injectable, NgModule } from '@angular/core';
import {
  BrowserModule,
  HammerModule,
  HammerGestureConfig,
  HAMMER_GESTURE_CONFIG,
} from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import * as Hammer from 'hammerjs';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { MaterialModule } from './modules/material.module';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './authentication/login/login.component';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { JwtInterceptor } from './interceptors/jwt.interceptor';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
import { RangeInputComponent } from './components/range-input/range-input.component';
import { ReversePipe } from './components/pipes/reverse.pipe';
import { ToIntArrayPipe } from './components/pipes/to-int-array.pipe';
import { MultiSelectComponent } from './components/multi-select/multi-select.component';
import { GalleryComponent } from './components/gallery/gallery.component';
import { PaginationComponent } from './components/pagination/pagination.component';
import { RegisterComponent } from './authentication/register/register.component';
import { UserExistDirective } from './components/directives/user-exist.directive';
import { InputComponent } from './components/forms/input/input.component';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { ReducePipe } from './components/pipes/reduce.pipe';
import { ScrollContentComponent } from './components/scroll-content/scroll-content.component';
import { AboutComponent } from './about/about.component';
import { ScrollToTopComponent } from './components/scroll-to-top/scroll-to-top.component';
import { FilterListComponent } from './components/filter-list/filter-list.component';
import { SpaceBetweenPipe } from './components/pipes/space-between.pipe';
import { SingleSelectComponent } from './components/single-select/single-select.component';
import { SelectLocationComponent } from './components/select-location/select-location.component';
import { AddItemsComponent } from './components/add-items/add-items.component';
import { AdminRolesComponent } from './roles/admin/admin-roles/admin-roles.component';
import { GetUserDirective } from './components/directives/get-user.directive';
import { AddAdminRoleComponent } from './roles/admin/add-admin-role/add-admin-role.component';
import { PageNotFoundComponent } from './error/page-not-found/page-not-found.component';
import { InRoleDirective } from './components/directives/in-role.directive';
import { ServerErrorComponent } from './error/server-error/server-error.component';
import { UserJobsListComponent } from './jobs/user-jobs/user-jobs-list.component';
import { JobDetailsPreviewComponent } from './jobs/job-details-preview/job-details-preview.component';
import { JobDetailsManagerComponent } from './jobs/job-details-manager/job-details-manager.component';
import { MyAdsComponent } from './jobs/my-ads/my-ads.component';
import { UserAdsFilterComponent } from './jobs/user-jobs/user-ads-filter/user-ads-filter.component';
import { DatePipe } from '@angular/common';
import { EmailModalComponent } from './modal/email-modal/email-modal.component';
import { MatDialogModule } from '@angular/material/dialog';
import { CancelConfirmationModalComponent } from './modal/cancel-confirmation-modal/cancel-confirmation-modal.component';
import { TruncateModule } from './customPipes/truncate.module';
import { BreadcrumbsComponent } from './components/breadcrumbs/breadcrumbs.component';
import { UserJobsComponent } from './jobs/user-jobs/user-jobs.component';
import { BackButtonDirective } from './components/directives/back-button.directive';
import { ContactUsComponent } from './contactUs/contactus.component';
import {MatButtonToggleModule} from '@angular/material/button-toggle';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MyAdComponent } from './jobs/my-ads/my-ad/my-ad.component';
import { MyAdsWrapper } from './jobs/my-ads/my-ads-wrapper.component';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatRadioModule } from '@angular/material/radio';
import { MatTabsModule } from '@angular/material/tabs';
import { EditCompanyProfileComponent } from './edit-company-profile/edit-company-profile.component';
import { CompanyCreateJobComponent } from './jobs/company-create-job/company-create-job.component';
import { CompanyJobAdsWrapperComponent } from './jobs/company-job-ads/company-job-ads-wrapper.component';
import { CompanyJobAdsComponent } from './jobs/company-job-ads/company-job-ads/company-job-ads.component';
import { SubmitApplicationModalComponent } from './modal/submit-application-modal/submit-application-modal.component';
import { CompanyJobPreviewComponent } from './jobs/company-job-ads/company-job-preview/company-job-preview.component';
import { CompanyMyAdsComponent } from './jobs/company-my-ads/company-my-ads.component';
import { CompanyMyAdsWrapperComponent } from './jobs/company-my-ads/company-my-ads-wrapper.component';
import { CompanyMyAdManagerComponent } from './jobs/company-my-ads/company-my-ad-manager/company-my-ad-manager.component';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import { FilterPipe } from './customPipes/filter.pipe';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { registerLocaleData } from '@angular/common';
import localeBS from '@angular/common/locales/bs';
import { BaseAdCardComponent } from './components/base-ad-card/base-ad-card.component';
import { ConfirmationModalComponent } from './modal/confirmation-modal/confirmation-modal.component';
import { EmailConfirmationComponent } from './components/email-confirmation/email-confirmation.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';

registerLocaleData(localeBS);


@Injectable()
export class HammerConfig extends HammerGestureConfig {
  overrides = {
    swipe: { direction: Hammer.DIRECTION_HORIZONTAL },
    pinch: { enable: false },
    rotate: { enable: false },
  };
}

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    LoginComponent,
    SpinnerComponent,
    RangeInputComponent,
    ReversePipe,
    ToIntArrayPipe,
    MultiSelectComponent,
    GalleryComponent,
    PaginationComponent,
    RegisterComponent,
    UserExistDirective,
    InputComponent,
    EditProfileComponent,
    ReducePipe,
    ScrollContentComponent,
    AboutComponent,
    ContactUsComponent,
    ScrollToTopComponent,
    FilterListComponent,
    SpaceBetweenPipe,
    SingleSelectComponent,
    SelectLocationComponent,
    AddItemsComponent,
    AdminRolesComponent,
    GetUserDirective,
    AddAdminRoleComponent,
    PageNotFoundComponent,
    InRoleDirective,
    ServerErrorComponent,
    UserJobsListComponent,
    JobDetailsPreviewComponent,
    JobDetailsManagerComponent,
    MyAdsComponent,
    UserAdsFilterComponent,
    EmailModalComponent,
    CancelConfirmationModalComponent,
    BreadcrumbsComponent,
    UserJobsComponent,
    BackButtonDirective,
    MyAdComponent,
    MyAdsWrapper,
    EditCompanyProfileComponent,
    CompanyCreateJobComponent,
    CompanyJobAdsWrapperComponent,
    CompanyJobAdsComponent,
    SubmitApplicationModalComponent,
    CompanyJobPreviewComponent,
    CompanyMyAdsComponent,
    CompanyMyAdsWrapperComponent,
    CompanyMyAdManagerComponent,
    FilterPipe,
    BaseAdCardComponent,
    ConfirmationModalComponent,
    EmailConfirmationComponent,
    ResetPasswordComponent,
    ForgotPasswordComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MaterialModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HammerModule,
    MatDialogModule,
    TruncateModule,
    MatButtonToggleModule,
    MatSlideToggleModule,
    MatCheckboxModule,
    MatRadioModule,
    MatTabsModule,
    NgxDocViewerModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
    { provide: HAMMER_GESTURE_CONFIG, useClass: HammerConfig },
    { provide: MAT_DATE_LOCALE, useValue: 'hr' },
    [DatePipe]    
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
