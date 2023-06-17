import {Component, Input, OnChanges, OnInit} from '@angular/core';
import {HttpService} from "../../services/http.service";
import {AuthService} from "../../services/auth.service";
import {User} from "../../models/user";
import {Post} from "../../models/post";

@Component({
  selector: 'app-user-post',
  templateUrl: './user-post.component.html',
  styleUrls: ['./user-post.component.scss']
})
export class UserPostComponent implements OnInit {
  user: any = this.authService.user$;
  postingUser: User | undefined;
  dummypost: any = {
    id: 1,
    userId: 1,
    liked: false
  }
  @Input() post: Post | undefined; // Change the type as per your data structure
  constructor(private httpService: HttpService, private authService: AuthService) {
  }

  ngOnInit(): void {
    console.log('POST',this.post);

    if (this.post) {
      this.getUserById(this.post.userId);
    }
  }

  likePost() {
    // Implement the logic to like the post here
  }

  getUserById(id: number) {
    this.httpService.get('/Profile/' + id + '/view').subscribe(
      (response: any) => {
        this.postingUser = response;
      },
      (error: any) => {
        console.log(error);
      }
    );
  }

  toggleLikePost() {
    if (this.dummypost.liked) {
      // Unlike the post
      this.dummypost.likesCount--;
      this.dummypost.liked = false;
    } else {
      // Like the post
      this.dummypost.likesCount++;
      this.dummypost.liked = true;
    }
  }

}
