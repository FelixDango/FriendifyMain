import { Component } from '@angular/core';
import { AuthService } from "../../services/auth.service";
import { Router } from "@angular/router";
import {HttpResponse} from "@angular/common/http";
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  username: string = "";
  password: string = "";
  rememberMe: boolean = false;

  constructor(private authService: AuthService, private router: Router) {}

  signIn(username: string, password: string, rememberMe: boolean): void {
    this.authService.login(username, password, rememberMe).subscribe(
      (response: HttpResponse<any>) => {
        // Handle login success
        // get only the token from the response header
        let authToken = response.headers.get('Authorization');
        if (authToken) {
          authToken = authToken.replace('Bearer ', '');
          // Set token in local storage
          localStorage.setItem('token', authToken);
          // Redirect to home page
          this.router.navigate(['/']);
          // Save user data in local storage
          localStorage.setItem('user', JSON.stringify(response.body));
          // Emit the authentication status
          this.authService.isLoggedInSubject.next(true);
          this.authService.user$ = response.body;
          console.log('auth user', this.authService.user$);
        } else {
          console.log('Authorization header not found');
        }
      },
      (error) => {
        // Handle login error
        console.log(error);
      }
    );
  }
}
