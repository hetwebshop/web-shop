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
import { UserJobsComponent } from './jobs/user-jobs/user-jobs.component';
import { JobDetailsPreviewComponent } from './jobs/job-details-preview/job-details-preview.component';
import { JobDetailsManagerComponent } from './jobs/job-details-manager/job-details-manager.component';
import { MyAdsComponent } from './jobs/my-ads/my-ads.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'about', component: AboutComponent },
  { path: 'edit-profile', component: EditProfileComponent },
  { path: 'user-jobs', component: UserJobsComponent},
  { path: 'user-job-details-preview/:id', component: JobDetailsPreviewComponent},
  { path: 'user-job-details/:id', component: JobDetailsManagerComponent},
  { path: 'user-job-details', component: JobDetailsManagerComponent},
  { path: 'my-ads', component: MyAdsComponent},
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
  { path: 'server-error', component: ServerErrorComponent },
  { path: '**', component: PageNotFoundComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
