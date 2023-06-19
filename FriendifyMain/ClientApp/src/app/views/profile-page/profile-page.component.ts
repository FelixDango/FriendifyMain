import {Component, OnInit} from '@angular/core';
import {Observable, of} from "rxjs";
import {Post} from "../../models/post";
import {AuthService} from "../../services/auth.service";
import {PostsService} from "../../services/posts.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-profile-page',
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.scss']
})
export class ProfilePageComponent implements OnInit {
  userPosts$: Observable<Post[]> = of([] as Post[]);
  posts : Post[] = [];
  username: string | undefined;

  constructor(public authService: AuthService, private postsService: PostsService, private route : ActivatedRoute) {
    let name = this.route.snapshot.paramMap.get('username');
    if (name) this.username = name;

    this.postsService.userPosts$.subscribe((posts: Post[]) => {
        if (posts.length > 0) {
          this.posts = posts;
        }
    });
    this.userPosts$ = this.postsService.userPosts$;
  }

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      // get user posts from posts service
      this.postsService.userPosts$.subscribe((posts: Post[]) => {
        if (posts.length > 0) {
          this.userPosts$ = this.postsService.userPosts$;
        }
      });
      // Get the user posts
      if (this.username) {
        this.postsService.loadUserPosts(this.username);
      }
    }
  }
}
