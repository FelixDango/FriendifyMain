import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './views/home/home.component';
import { FetchDataComponent } from './views/fetch-data/fetch-data.component';
import { UserPostComponent } from './components/user-post/user-post.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ProfilePageComponent } from './views/profile-page/profile-page.component';
import { ProfileComponent } from './components/profile/profile.component';
import { ComposePostComponent } from './components/compose-post/compose-post.component';
import {CheckboxModule} from "primeng/checkbox";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    FetchDataComponent,
    UserPostComponent,
    LoginComponent,
    RegisterComponent,
    ProfilePageComponent,
    ProfileComponent,
    ComposePostComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    InputTextModule,
    PasswordModule,
    ButtonModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full'},
      {path: 'fetch-data', component: FetchDataComponent},
      {path: 'login', component: LoginComponent},
      {path: 'register', component: RegisterComponent},
      {path: 'profile-page', component: ProfilePageComponent},
    ]),
    CheckboxModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
