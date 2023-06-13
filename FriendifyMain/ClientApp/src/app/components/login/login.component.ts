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

        document.cookie = response.headers.get('Set-Cookie') || '';


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



  private setCookie(name: string, value: string, expiresInDays: number) {
    const expires = new Date();
    expires.setDate(expires.getDate() + expiresInDays);
    const cookieValue = encodeURIComponent(value) + (expires ? '; expires=' + expires.toUTCString() : '');
    document.cookie = name + '=' + cookieValue + '; path=/';
  }
}
