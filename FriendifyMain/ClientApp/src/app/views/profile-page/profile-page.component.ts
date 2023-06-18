import {Component, OnInit} from '@angular/core';
import {Observable, of} from "rxjs";
import {Post} from "../../models/post";
import {AuthService} from "../../services/auth.service";
import {Profile} from "oidc-client";
import {PostsService} from "../../services/posts.service";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-profile-page',
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.scss']
})
export class ProfilePageComponent implements OnInit {
  userProfile$: Observable<Profile> | undefined;
  userPosts: Post[] = [];
  id: number | undefined;
  constructor(public authService: AuthService, private postsService: PostsService, private route : ActivatedRoute) {
    let routeString = this.route.snapshot.paramMap.get('id');
    if (routeString) this.id = parseInt(routeString, 10);
  }

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      // Get the user posts
      this.postsService.userPosts$.subscribe((posts: Post[]) => {
        this.userPosts = posts;
      });
      if (this.id) this.postsService.loadUserPosts(this.id);

    }
  }





}
