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
    console.log(this.username, this.password, this.rememberMe);
    this.authService.login(username, password, rememberMe).subscribe(
      (response: HttpResponse<any>) => {
        // Handle login success
        //const token = response.headers.get('Authorization');
        const authToken = response.headers.get('Authorization');
        if (authToken) {
          console.log(authToken.length);
          // Set token in local storage
          localStorage.setItem('token', authToken);
          // Redirect to home page
          this.router.navigate(['/']);
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
