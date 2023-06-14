import { Component } from '@angular/core';
import { AuthService } from "../../services/auth.service";
import {HttpResponse} from "@angular/common/http";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  username: string = "";
  password: string = "";
  rememberMe: boolean = false;

  constructor(private authService: AuthService) {}

  signIn(username: string, password: string, rememberMe: boolean): void {
    this.authService.login(username, password, rememberMe).subscribe(
      (response: HttpResponse<any>) => {


            // Save user data in local storage
            localStorage.setItem('user', JSON.stringify(response.body));

            // Emit the authentication status
            this.authService.isLoggedInSubject.next(true);
            this.authService.user$ = response.body;
            console.log('auth user', this.authService.user$);
      },
      (error) => {
        // Handle login error
        console.log(error);
      }
    );
  }

}
