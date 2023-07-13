import { Component, OnInit } from '@angular/core';
import { AuthService } from "../../services/auth.service";
import { MenuItem } from "primeng/api";
import { BehaviorSubject, Observable } from 'rxjs';
import { User } from "../../models/user";
import { HttpService } from "../../services/http.service";
import { ProfileService } from "../../services/profile.service";
import { SearchService } from "../../services/search.service";
import { Post } from "../../models/post";
import { AutoComplete } from "primeng/autocomplete";
import { Router } from "@angular/router";

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.scss']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  items: MenuItem[] = [];
  // Define the user property as User or undefined
  user: User | undefined;


  assetsUrl: string;

  searchText: string = "";
  searchResults: User[] = [];
  showPopup: boolean = false;
  loading: boolean = false;

  public window = window;
  username = '';

  baseUrl: string = "https://localhost:44401";
  backendUrl: string = "https://localhost:7073";

  constructor(
    // Inject the AuthService
    public authService: AuthService,
    private httpService: HttpService,
    private profileService: ProfileService,
    private searchService: SearchService,
  ) {
    this.assetsUrl = this.httpService.assetsUrl;
    // Assign the user property with the value from the authService
    this.authService.user$.subscribe((user: User) => {
      this.user = user;
    });
    this.authService.isLoggedIn$.subscribe((isLoggedIn: boolean) => {
      if (!isLoggedIn) {
        this.updateMenuItems();
      } else {
        this.updateMenuItems();
      }
    });

  }

  ngOnInit() {
    this.authService.updateUser();
    if (this.authService.isLoggedIn()) {
      if (this.user) this.profileService.loadProfile(this.user.userName);
    }
    this.updateMenuItems();

  }
  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout(): void {
    this.authService.logout();
  }


  private updateMenuItems(): void {

    if (this.authService.isLoggedIn()) {
      this.username = this.authService.getUserName();

    }

    this.items = [
      {
        label: 'Home',
        icon: 'pi pi-fw pi-home',
        routerLink: '/'
      },
      {
        label: 'Register',
        routerLink: '/register',
        icon: 'pi pi-fw pi-user-plus',
        visible: !this.authService.isLoggedIn()
      },
      {
        label: 'Login',
        routerLink: '/login',
        icon: 'pi pi-fw pi-sign-in',
        visible: !this.authService.isLoggedIn()
      },
      {
        label: 'Profile',
        icon: 'pi pi-fw pi-user',
        visible: this.authService.isLoggedIn(),
        items: [
          {
            label: 'View Profile',
            icon: 'pi pi-fw pi-user',
            routerLink: ['/own-profile/' + this.username],
            visible: this.authService.isLoggedIn(),
          },
          {
            label: 'Edit Profile',
            icon: 'pi pi-fw pi-user-edit',
            routerLink: '/edit-profile'
          },
          {
            label: 'Messages',
            routerLink: '/messages',
            icon: 'pi pi-fw pi-envelope',
            visible: this.authService.isLoggedIn()
          },
          {
            icon: 'pi pi-fw pi-sign-out',
            label: 'Logout',
            routerLink: '/logout',
            visible: this.authService.isLoggedIn()
          },
          {
            label: 'Admin Panel',
            icon: 'pi pi-fw pi-cog',
            routerLink: '/admin',
            visible: this.authService.isLoggedIn() && this.authService.isAdmin() || this.authService.isModerator()
          }
        ]
      },
    ];
  }

  doSearch() {

    this.loading = true;
    this.showPopup = true;
    this.searchResults = [];

    this.searchService.findUser(this.searchText).subscribe((users: User[]) => {
      this.searchResults = users;

      this.loading = false;
    });

  }
}

