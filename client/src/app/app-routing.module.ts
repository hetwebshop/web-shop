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

const routes: Routes = [
  { path: '', component: HomeComponent, data: { breadcrumb: {alias: 'Uredi oglas 1'}} },
  { path: 'login', component: LoginComponent, data: { breadcrumb: {alias: 'Uredi oglas 2'} } },
  { path: 'register', component: RegisterComponent, data: { breadcrumb: {alias: 'Uredi oglas 3'} } },
  { path: 'about', component: AboutComponent, data: { breadcrumb: {alias: 'Uredi oglas 4'}} },
  { path: 'edit-profile', component: EditProfileComponent, data: { breadcrumb: {alias: 'Uredi oglas 5'} } },
  { path: 'ads', component: UserJobsComponent, data: { breadcrumb: {alias: 'Uredi oglas6'} }, 
  children: [
    { path: '', component: UserJobsListComponent },
    { path: ':id', component: JobDetailsPreviewComponent}
  ]},
  //{ path: 'user-job-details-preview/:id', component: JobDetailsPreviewComponent, data: { breadcrumb: {alias: 'Uredi oglas7'} }},
  { path: 'user-job-details/:id', component: JobDetailsManagerComponent,  data: { breadcrumb: {alias: 'Uredi oglas8'} }},
  { path: 'user-job-details', component: JobDetailsManagerComponent, data: { breadcrumb: {alias: 'Uredi oglas9'}  }},
  { path: 'my-ads', component: MyAdsComponent, data: {breadcrumb: {alias: 'Uredi oglas91'} }},
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
