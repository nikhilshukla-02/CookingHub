import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { AgGridModule } from 'ag-grid-angular';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { AdminnavComponent } from './components/adminnav/adminnav.component';
import { UsernavComponent } from './components/usernav/usernav.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { RegistrationComponent } from './components/registration/registration.component';
import { AdminaddclassComponent } from './components/adminaddclass/adminaddclass.component';
import { AdmineditclassComponent } from './components/admineditclass/admineditclass.component';
import { AdminviewclassComponent } from './components/adminviewclass/adminviewclass.component';
import { AdminviewappliedrequestComponent } from './components/adminviewappliedrequest/adminviewappliedrequest.component';
import { AdminviewfeedbackComponent } from './components/adminviewfeedback/adminviewfeedback.component';
import { UseraddrequestComponent } from './components/useraddrequest/useraddrequest.component';
import { UserviewclassComponent } from './components/userviewclass/userviewclass.component';
import { UserviewappliedrequestComponent } from './components/userviewappliedrequest/userviewappliedrequest.component';
import { UseraddfeedbackComponent } from './components/useraddfeedback/useraddfeedback.component';
import { UserviewfeedbackComponent } from './components/userviewfeedback/userviewfeedback.component';
import { CookingImpComponent } from './components/cooking-imp/cooking-imp.component';
import { ErrorComponent } from './components/error/error.component';
import { ChatbotComponent } from './components/chatbot/chatbot.component';

 
@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    AdminnavComponent,
    UsernavComponent,
    HomeComponent,
    LoginComponent,
    RegistrationComponent,
    AdminaddclassComponent,
    AdmineditclassComponent,
    AdminviewclassComponent,
    AdminviewappliedrequestComponent,
    AdminviewfeedbackComponent,
    UseraddrequestComponent,
    UserviewclassComponent,
    UserviewappliedrequestComponent,
    UseraddfeedbackComponent,
    UserviewfeedbackComponent,
    CookingImpComponent,
    ErrorComponent,
    ChatbotComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
    AgGridModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
