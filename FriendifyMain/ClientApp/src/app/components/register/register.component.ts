import { Component } from '@angular/core';
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  username: string | undefined;
  password: string | undefined;
  confirmPassword: string | undefined;
  firstName: string | undefined;
  lastName: string | undefined;
  email: string | undefined;
  birthdate: Date | undefined;
  sex: string | undefined;

  constructor(private authService: AuthService) {}
  register(): void {
    if (this.password !== this.confirmPassword) {
      // Handle password confirmation mismatch
      return;
    }

    const registerData = {
      username: this.username,
      password: this.password,
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      birthdate: this.birthdate,
      sex: this.sex
    };

    this.authService.register(registerData).subscribe(
      (response: any) => {
        console.log(response);
        // Handle successful registration response
      },
      (error: any) => {
        // Handle registration error
        console.error(error);

      }
    );
  }
}
