import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
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
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppLayoutModule } from './layout/app.layout.module';
import { AppMenuitemComponent } from './layout/app.menuitem.component';
import { SelectButtonModule } from 'primeng/selectbutton';
import { UserManagementComponent } from './views/user-management/user-management.component';
import { TableModule } from 'primeng/table';
import { Dialog, DialogModule } from 'primeng/dialog';
import { AdminSidebarComponent } from './components/admin-sidebar/admin-sidebar.component';
import { RadioButtonModule } from 'primeng/radiobutton';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { TagModule } from 'primeng/tag';
import { SkeletonModule } from 'primeng/skeleton';
import { PostManagementComponent } from './views/post-management/post-management.component';



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
    CommentComponent,
    UserManagementComponent,
    AdminSidebarComponent,
    PostManagementComponent
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
            {path: 'edit-profile', component: EditProfileComponent },
            {path: 'admin/users', component: UserManagementComponent },
            {path: 'admin/posts', component: PostManagementComponent },

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
      BrowserAnimationsModule,
      AppLayoutModule,
      SelectButtonModule,
      TableModule,
      DialogModule,
      ConfirmDialogModule,
      RadioButtonModule,
      ToastModule,
      TagModule,
      SkeletonModule
    ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true
    },
    ConfirmationService,
    MessageService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
