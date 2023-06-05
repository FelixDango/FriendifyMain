import { Component } from '@angular/core';
import { AuthService } from "../../services/auth.service";
import { Router } from "@angular/router";

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
      (response) => {
        // Handle login success
        //const token = response.headers.get('Authorization');
        console.log(response.headers);
        // set token in local storage
        //if (token) localStorage.setItem('token', token);
        // redirect to home page
        //this.router.navigate(['/']);
      },
      (error) => {
        // Handle login error
        console.log(error);
      }
    );
  }
}
