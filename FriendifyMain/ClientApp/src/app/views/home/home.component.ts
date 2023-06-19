import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { HttpService } from 'src/app/services/http.service';
import {map, Observable, of} from 'rxjs';
import {Post} from "../../models/post";
import {User} from "../../models/user";
import {PostsService} from "../../services/posts.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  posts$: Observable<Post[]> = of([]);
  publicPosts$: Observable<Post[]> = of([]);
  user: User | undefined;

  constructor(public authService: AuthService, private postsService: PostsService) {
    // get user posts from posts service
    this.postsService.posts$.subscribe((posts: Post[]) => {
      this.posts$ = this.postsService.posts$.pipe(
        map((posts: Post[]) => {
          // Sort the posts by date in descending order
          return posts.sort((a, b) => {
            const dateA = new Date(a.date);
            const dateB = new Date(b.date);
            return dateB.getTime() - dateA.getTime();
          });
        })
      );
    });

    this.postsService.publicPosts$.subscribe((posts: Post[]) => {
      this.publicPosts$ = this.postsService.publicPosts$.pipe(
        map((posts: Post[]) => {
          // Sort the posts by date in descending order
          return posts.sort((a, b) => {
            const dateA = new Date(a.date);
            const dateB = new Date(b.date);
            return dateB.getTime() - dateA.getTime();
          });
        })
      );
    });

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
      this.postsService.loadPosts();
    } else {
      this.postsService.loadPublicPosts();
      console.log('Not logged in');
    }
  }

}
