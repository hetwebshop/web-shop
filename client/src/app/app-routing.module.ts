import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './authentication/login/login.component';
import { RegisterComponent } from './authentication/register/register.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { AboutComponent } from './about/about.component';
import { AddAdminRoleComponent } from './roles/admin/add-admin-role/add-admin-role.component';
import { AdminRolesComponent } from './roles/admin/admin-roles/admin-roles.component';
import { AuthGuard } from './components/guards/auth.guard';
import { PageNotFoundComponent } from './error/page-not-found/page-not-found.component';
import { ServerErrorComponent } from './error/server-error/server-error.component';
import { JobDetailsPreviewComponent } from './jobs/job-details-preview/job-details-preview.component';
import { JobDetailsManagerComponent } from './jobs/job-details-manager/job-details-manager.component';
import { MyAdsComponent } from './jobs/my-ads/my-ads.component';
import { CreateAdAuthGard } from './components/guards/createAdAuth.guard';
import { UserJobsListComponent } from './jobs/user-jobs/user-jobs-list.component';
import { UserJobsComponent } from './jobs/user-jobs/user-jobs.component';
import { ContactUsComponent } from './contactUs/contactus.component';
import { MyAdComponent } from './jobs/my-ads/my-ad/my-ad.component';
import { MyAdsWrapper } from './jobs/my-ads/my-ads-wrapper.component';
import { EditCompanyProfileComponent } from './edit-company-profile/edit-company-profile.component';
import { CompanyCreateJobComponent } from './jobs/company-create-job/company-create-job.component';
import { CompanyJobAdsWrapperComponent } from './jobs/company-job-ads/company-job-ads-wrapper.component';
import { CompanyJobAdsComponent } from './jobs/company-job-ads/company-job-ads/company-job-ads.component';
import { CompanyJobPreviewComponent } from './jobs/company-job-ads/company-job-preview/company-job-preview.component';
import { CompanyMyAdsWrapperComponent } from './jobs/company-my-ads/company-my-ads-wrapper.component';
import { CompanyMyAdsComponent } from './jobs/company-my-ads/company-my-ads.component';
import { CompanyMyAdManagerComponent } from './jobs/company-my-ads/company-my-ad-manager/company-my-ad-manager.component';
import { EmailConfirmationComponent } from './components/email-confirmation/email-confirmation.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ChangePasswordComponent } from './components/change-password/change-password.component';

const routes: Routes = [
  { path: '', redirectTo: '/ads?type=JobAd', pathMatch: 'full' },
  { path: 'login', component: LoginComponent, data: { breadcrumb: {alias: 'Uredi oglas 2'} } },
  { path: 'register', component: RegisterComponent, data: { breadcrumb: {alias: 'Uredi oglas 3'} } },
  { path: 'confirm-email', component: EmailConfirmationComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent},
  { path: 'reset-password', component: ResetPasswordComponent},
  { path: 'change-password', component: ChangePasswordComponent},
  { path: 'about', component: AboutComponent, data: { breadcrumb: {alias: 'Uredi oglas 4'}} },
  { path: 'contact-us', component: ContactUsComponent, data: { breadcrumb: {alias: 'Uredi oglas 4'}} },
  { path: 'edit-profile', component: EditProfileComponent, data: { breadcrumb: {alias: 'Uredi oglas 5'} } },
  { path: 'edit-company-profile', component: EditCompanyProfileComponent, data: { breadcrumb: {alias: 'Uredi oglas 5'} } },
  { path: 'ads', component: UserJobsComponent, data: { breadcrumb: {alias: 'Uredi oglas6'} }, 
  children: [
    { path: '', component: UserJobsListComponent },
    { path: ':id', component: JobDetailsPreviewComponent}
  ]},
  { path: 'company-job-ads', component: CompanyJobAdsWrapperComponent, data: { breadcrumb: {alias: 'Uredi oglas6'} }, 
  children: [
    { path: '', component: CompanyJobAdsComponent },
    { path: ':id', component: CompanyJobPreviewComponent}
  ]},
  //{ path: 'user-job-details-preview/:id', component: JobDetailsPreviewComponent, data: { breadcrumb: {alias: 'Uredi oglas7'} }},
  { path: 'user-job-details/:id', component: JobDetailsManagerComponent,  data: { breadcrumb: {alias: 'Uredi oglas8'} }},
  { path: 'create-job-ad', component: JobDetailsManagerComponent, data: { breadcrumb: {alias: 'Uredi oglas9'}  }},
  { path: 'company-create-job-ad', component: CompanyCreateJobComponent, data: { breadcrumb: {alias: 'Uredi oglas9'}  }},
  { path: 'create-service-ad', component: JobDetailsManagerComponent, data: { breadcrumb: {alias: 'Uredi oglas9'}  }},
  { path: 'my-ads', component: MyAdsWrapper, data: {breadcrumb: {alias: 'Uredi oglas91'} },
  children: [
    { path: '', component: MyAdsComponent },
    { path: ':id', component: MyAdComponent}
  ]},
  { path: 'company-my-ads', component: CompanyMyAdsWrapperComponent, data: {breadcrumb: {alias: 'Uredi oglas91'} },
  children: [
    { path: '', component: CompanyMyAdsComponent },
    { path: ':id', component: CompanyMyAdManagerComponent}
  ]},
  // {
  //   path: '',
  //   runGuardsAndResolvers: 'always',
  //   canActivate: [AuthGuard],
  //   data: { roles: ['StoreAgent', 'StoreAdmin'] },
  //   // children: [
  //   //   { path: 'store/order/:id', component: StoreOrderComponent },
  //   //   { path: 'store/order', component: StoreOrderListComponent },
  //   // ],
  // },
  // {
  //   path: '',
  //   runGuardsAndResolvers: 'always',
  //   canActivate: [AuthGuard],
  //   data: { roles: ['Admin'] },
  //   children: [
  //     {
  //       path: 'admin/moderate/admin-role/add',
  //       component: AddAdminRoleComponent,
  //     },
  //     { path: 'admin/moderate/admin-role', component: AdminRolesComponent },
  //   ],

  // },
  { path: 'server-error', component: ServerErrorComponent, data: {breadcrumb: {alias: 'Uredi oglas21'} } },
  { path: '**', component: PageNotFoundComponent, data: { breadcrumb: {alias: 'Uredi oglas43'} } },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
