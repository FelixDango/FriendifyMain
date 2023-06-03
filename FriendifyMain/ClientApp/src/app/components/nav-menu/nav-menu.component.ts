import {Component} from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {MenuItem} from "primeng/api";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent {
  isExpanded = false;
  items: MenuItem[];

  constructor(private authService: AuthService) {
    this.items = [
      {
        label: 'Home',
        routerLink: '/'
      },
      {
        label: 'Profile',
        routerLink: '/profile-page'
      },
      {
        label: 'Login',
        routerLink: '/login'
      },
      {
        label: 'Register',
        routerLink: '/register'
      }
    ];
  }
  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
