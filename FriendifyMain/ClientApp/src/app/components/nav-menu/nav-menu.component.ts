import { Component, OnInit } from '@angular/core';
import { AuthService } from "../../services/auth.service";
import { MenuItem } from "primeng/api";
import { Observable } from 'rxjs';
import {User} from "../../models/user";
import {HttpService} from "../../services/http.service";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  items: MenuItem[] = [];
  isLoggedIn$: Observable<boolean>;
  user: User = {} as User;
  assetsUrl: string;

  constructor(private authService: AuthService, private httpService: HttpService) {
    this.assetsUrl = this.httpService.assetsUrl;
    this.isLoggedIn$ = this.authService.isLoggedIn$;
    this.authService.user$.subscribe((user: User) => {
      this.user = user;
    })
  }

  ngOnInit() {
    this.authService.updateUser();
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
        label: 'My Profile',
        routerLink: '/profile/' + this.authService.getUserName(),
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
      },
      {
        label: 'Admin',
        routerLink: '/admin',
        visible: this.authService.isLoggedIn() && this.user.isAdmin
      },
      {
        label: 'Messages',
        routerLink: '/messages',
        visible: this.authService.isLoggedIn()
      }
    ];
  }
}
