import { Component, OnInit } from '@angular/core';
import { AuthService } from "../../services/auth.service";
import { MenuItem } from "primeng/api";
import { Observable } from 'rxjs';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  items: MenuItem[] = [];
  isLoggedIn$: Observable<boolean>;

  constructor(private authService: AuthService) {
    this.isLoggedIn$ = this.authService.isLoggedIn$;

  }

  ngOnInit() {
    this.updateMenuItems();

    // Subscribe to isLoggedIn$ to update the navigation in real time
    this.isLoggedIn$.subscribe(() => {
      this.updateMenuItems();
    });
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout(): void {
    this.authService.logout();
  }

  private updateMenuItems(): void {
    this.items = [
      {
        label: 'Home',
        routerLink: '/'
      },
      {
        label: 'Profile',
        routerLink: '/profile-page',
        visible: this.authService.isLoggedIn()
      },
      {
        label: this.authService.isLoggedIn() ? 'Logout' : 'Login',
        routerLink: this.authService.isLoggedIn() ? '/logout' : '/login'
      },
      {
        label: 'Register',
        routerLink: '/register',
        visible: !this.authService.isLoggedIn()
      }
    ];
  }
}
