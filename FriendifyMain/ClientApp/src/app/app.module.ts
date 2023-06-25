import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './components/nav-menu/nav-menu.component';
import { HomeComponent } from './views/home/home.component';
import { UserPostComponent } from './components/user-post/user-post.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ProfilePageComponent } from './views/profile-page/profile-page.component';
import { ProfileComponent } from './components/profile/profile.component';
import { ComposePostComponent } from './components/compose-post/compose-post.component';
import {CheckboxModule} from "primeng/checkbox";
import {FileUploadModule} from "primeng/fileupload";
import {InputTextareaModule} from "primeng/inputtextarea";
import {RippleModule} from "primeng/ripple";
import {ImageModule} from "primeng/image";
import {AvatarModule} from "primeng/avatar";
import {MenubarModule} from "primeng/menubar";
import { LogoutComponent } from './components/logout/logout.component';
import { TokenInterceptor } from './services/token-interceptor.service';
import { AdminComponent } from './views/admin/admin.component';
import { MessagesComponent } from './views/messages/messages.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { PrivateMessagesComponent } from './components/private-messages/private-messages.component';
import {SidebarModule} from "primeng/sidebar";
import {ChartModule} from "primeng/chart";
import { EditProfileComponent } from './views/edit-profile/edit-profile.component';
import { CommentSectionComponent } from './components/user-post/comment-section/comment-section.component';
import { CommentComponent } from './components/user-post/comment/comment.component';
import { MenuModule } from 'primeng/menu';
import {DropdownModule} from "primeng/dropdown";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    UserPostComponent,
    LoginComponent,
    RegisterComponent,
    ProfilePageComponent,
    ProfileComponent,
    ComposePostComponent,
    LogoutComponent,
    AdminComponent,
    MessagesComponent,
    SidebarComponent,
    PrivateMessagesComponent,
    EditProfileComponent,
    CommentSectionComponent,
    CommentComponent
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
            {path: 'login', component: LoginComponent},
            {path: 'register', component: RegisterComponent},
            {path: 'profile/:username', component: ProfilePageComponent},
            {path: 'own-profile/:username', component: ProfilePageComponent},
            {path: 'logout', component: LogoutComponent},
            {path: 'admin', component: AdminComponent},
            {path: 'messages', component: MessagesComponent},
            {path: 'edit-profile', component: EditProfileComponent},
        ]),
        CheckboxModule,
        FileUploadModule,
        InputTextareaModule,
        RippleModule,
        ImageModule,
        AvatarModule,
        MenubarModule,
        SidebarModule,
        ChartModule,
        MenuModule,
        DropdownModule,
    ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
