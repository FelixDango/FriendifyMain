import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { HttpService } from 'src/app/services/http.service';
import {Observable, of} from 'rxjs';
import {Post} from "../../models/post";
import {User} from "../../models/user";
import {PostsService} from "../../services/posts.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  userPosts: Post[] = [];
  user: User | undefined;

  constructor(public authService: AuthService, private postsService: PostsService) {
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
      // Get the user posts
      this.postsService.posts$.subscribe((posts: Post[]) => {
        this.userPosts = posts;
        console.log('POSTS', this.userPosts);
      });
      this.postsService.loadPosts();
    }
  }

}
