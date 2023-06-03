import { Component } from '@angular/core';
import { AuthService } from "../../services/auth.service";

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
    console.log(this.username, this.password, this.rememberMe);
    this.authService.login(username, password, rememberMe).subscribe(
      (response) => {
        // Handle successful login response
        console.log(response);
      },
      (error) => {
        // Handle login error
        console.log(error);
      }
    );
  }
}
