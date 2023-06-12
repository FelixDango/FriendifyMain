import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { HttpService } from 'src/app/services/http.service';
import { Observable } from 'rxjs';
import {Post} from "../../models/post";
import {User} from "../../models/user";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  userPosts$: Observable<Post[]>;
  user: Observable<User> = this.authService.user$;

  constructor(public authService: AuthService, private httpService: HttpService) {
    this.userPosts$ = new Observable<Post[]>();
    // Subscribe to isLoggedIn$ to update the navigation in real time
    this.authService.isLoggedIn$.subscribe(() => {
      this.loadUserPosts();
    }, (error) => {
      console.log(error);
    });
  }

  ngOnInit() {
    // Check if the user is logged in
    if (this.authService.isLoggedIn()) {
      this.loadUserPosts();
    }
  }

  loadUserPosts() {
    // Call the service to retrieve the user posts
    this.userPosts$ = this.httpService.get('/Home');
  }
}
