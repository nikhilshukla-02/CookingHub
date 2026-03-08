import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { AdminviewclassComponent } from './components/adminviewclass/adminviewclass.component';
import { AdminaddclassComponent } from './components/adminaddclass/adminaddclass.component';
import { AdmineditclassComponent } from './components/admineditclass/admineditclass.component';
import { AdminviewappliedrequestComponent } from './components/adminviewappliedrequest/adminviewappliedrequest.component';
import { AdminviewfeedbackComponent } from './components/adminviewfeedback/adminviewfeedback.component';
import { UserviewclassComponent } from './components/userviewclass/userviewclass.component';
import { UseraddrequestComponent } from './components/useraddrequest/useraddrequest.component';
import { UserviewappliedrequestComponent } from './components/userviewappliedrequest/userviewappliedrequest.component';
import { UseraddfeedbackComponent } from './components/useraddfeedback/useraddfeedback.component';
import { UserviewfeedbackComponent } from './components/userviewfeedback/userviewfeedback.component';
import { CookingImpComponent } from './components/cooking-imp/cooking-imp.component';
import { ErrorComponent } from './components/error/error.component';
import { AuthGuard } from './components/authguard/auth.guard';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegistrationComponent },

  // ✅ Admin routes — only 'admin' role can access
  {
    path: 'admin/view-class',
    component: AdminviewclassComponent,
    canActivate: [AuthGuard],
    data: { role: 'admin' }
  },
  {
    path: 'admin/add-class',
    component: AdminaddclassComponent,
    canActivate: [AuthGuard],
    data: { role: 'admin' }
  },
  {
    path: 'admin/edit-class/:id',
    component: AdmineditclassComponent,
    canActivate: [AuthGuard],
    data: { role: 'admin' }
  },
  {
    path: 'admin/view-requests',
    component: AdminviewappliedrequestComponent,
    canActivate: [AuthGuard],
    data: { role: 'admin' }
  },
  {
    path: 'admin/view-feedbacks',
    component: AdminviewfeedbackComponent,
    canActivate: [AuthGuard],
    data: { role: 'admin' }
  },

  // ✅ User routes — only 'user' role can access
  {
    path: 'user/view-classes',
    component: UserviewclassComponent,
    canActivate: [AuthGuard],
    data: { role: 'user' }
  },
  {
    path: 'user/apply/:classId',
    component: UseraddrequestComponent,
    canActivate: [AuthGuard],
    data: { role: 'user' }
  },
  {
    path: 'user/my-requests',
    component: UserviewappliedrequestComponent,
    canActivate: [AuthGuard],
    data: { role: 'user' }
  },
  {
    path: 'user/add-feedback',
    component: UseraddfeedbackComponent,
    canActivate: [AuthGuard],
    data: { role: 'user' }
  },
  {
    path: 'user/my-feedbacks',
    component: UserviewfeedbackComponent,
    canActivate: [AuthGuard],
    data: { role: 'user' }
  },
  {
    path: 'user/cooking-imp',
    component: CookingImpComponent,
    canActivate: [AuthGuard],
    data: { role: 'user' }
  },

  { path: 'error', component: ErrorComponent },
  { path: '**', redirectTo: '/error' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }