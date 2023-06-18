import {Component, Input, OnChanges, OnInit} from '@angular/core';
import {HttpService} from "../../services/http.service";
import {AuthService} from "../../services/auth.service";
import {User} from "../../models/user";
import {Post} from "../../models/post";
import {BehaviorSubject, Observable} from "rxjs";
import {PostsService} from "../../services/posts.service";
import {Like} from "../../models/like";

@Component({
  selector: 'app-user-post',
  templateUrl: './user-post.component.html',
  styleUrls: ['./user-post.component.scss']
})
export class UserPostComponent implements OnInit {
  user: any = this.authService.user$;
  postingUser: User | undefined;
  assetsUrl: string = this.httpService.assetsUrl;
  postId: number = 0;
  // post as behavior subject
  post$: BehaviorSubject<Post>  = new BehaviorSubject<Post>( {} as Post);
  likedByUser$: BehaviorSubject<boolean> = new BehaviorSubject(false);
  @Input() post: Post | undefined; // Change the type as per your data structure
  constructor(private httpService: HttpService, private authService: AuthService) {
    this.user = this.authService.user$;
    this.post$.subscribe((post: Post) => {
      // if user liked the post
      if ( post.likes !== undefined) {
        this.likedByUser$.next(post.likes.some(obj => obj.userId === this.user.id));
      }

    })

    this.authService.user$.subscribe((user: User) => {
      this.user = user;
    })

  }

  ngOnInit(): void {
    if (this.post) {
      this.getUserById(this.post.userId);
      this.postId = this.post.id;
      this.post$.next(this.post);
    }
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
    if (this.likedByUser$.value) {
      // Unlikelike post
      this.httpService.post('/Home/' + this.postId + '/like', {}).subscribe(
        (response: any) => {
          console.log(response, 'unlike post');
          this.likedByUser$.next(false);
          // remove like by user
          this.post$.value.likes.splice(this.post$.value.likes.findIndex(obj => obj.userId === this.user.id), 1);
        },
        (error: any) => {
          console.log(error);
        }
      );
    } else {
      // like post
      this.httpService.post('/Home/' + this.postId + '/like', {}).subscribe(
        (response: any) => {
          console.log(response, 'like post');
          this.likedByUser$.next(true);
          var like: Like = {
            userId: this.user.id,
            postId: this.postId,
            dateTime: new Date(),
            user: this.user,
            post: this.post$.value
          }
          this.post$.value.likes.push(like);
        },
        (error: any) => {
          console.log(error);
        }
      );
    }
  }

}
