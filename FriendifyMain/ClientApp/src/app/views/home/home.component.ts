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
  user: User | undefined;

  constructor(public authService: AuthService, private httpService: HttpService) {
    this.userPosts$ = new Observable<Post[]>(); // Initialize the user posts
    // get user from local storage
    if (localStorage.getItem('user') === null) {
      this.user = undefined;
    } else {
      this.user = JSON.parse(localStorage.getItem('user') || '{}');
    }
  }

  ngOnInit() {
    // Check if the user is logged in
    if (this.authService.isLoggedIn()) {
      console.log('user is logged in');
      //this.loadUserPosts();
    }
  }

  loadUserPosts() {
    // Call the service to retrieve the user posts
    this.userPosts$ = this.httpService.get('/Home');
  }
}
