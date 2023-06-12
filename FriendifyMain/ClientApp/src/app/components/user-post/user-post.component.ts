import { Component, Input } from '@angular/core';
import {HttpService} from "../../services/http.service";
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-user-post',
  templateUrl: './user-post.component.html',
  styleUrls: ['./user-post.component.scss']
})
export class UserPostComponent {
  user: any = this.authService.user$;
  constructor(private httpService: HttpService, private authService: AuthService) {
  }
  @Input() post: any; // Change the type as per your data structure

  likePost() {
    // Implement the logic to like the post here
  }

  // create post method
  createPost( post: any, user: any) {
    // Implement the logic to create the post here

  }

  toggleLikePost() {
    if (this.post.liked) {
      // Unlike the post
      this.post.likesCount--;
      this.post.liked = false;
    } else {
      // Like the post
      this.post.likesCount++;
      this.post.liked = true;
    }
  }

}
