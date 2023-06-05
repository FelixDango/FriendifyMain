import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout',
  template: '',
})
export class LogoutComponent implements OnInit {
  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    // Implement logout functionality here
    this.authService.logout(); // Call the logout method in your AuthService to clear the user session or perform any necessary cleanup
    this.router.navigate(['/login']); // Redirect to the login page after logout
  }
}
