import {Component, Input, OnInit} from '@angular/core';
import {User} from "../../models/user";
import {HttpService} from "../../services/http.service";
import {AuthService} from "../../services/auth.service";
import {BehaviorSubject} from "rxjs";
import {PostsService} from "../../services/posts.service";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  @Input() username: string | undefined;
  user$: BehaviorSubject<User | undefined> = new BehaviorSubject<User | undefined>(undefined);
  loggedInUser: User | undefined = undefined;
  assetsUrl: string;
  isFollowing$ = new BehaviorSubject(<boolean>false);
  birthdate: Date = new Date();
  constructor(
    private httpService: HttpService,
    private authService: AuthService
  ) {
    this.assetsUrl = httpService.assetsUrl;
    // Add code to load user data
    this.authService.user$.subscribe((user: User) => {
        this.loggedInUser = user;
        this.checkFollow(this.loggedInUser.id)
      },
      (error: any) => {
        console.log(error);
      }
    );
  }

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.authService.updateUser();
      if (this.username) {
        this.loadUser(this.username);
      }
    }
  }


  loadUser(username: string) {
    // Add code to load user data
    this.httpService.get('/Profile/' + username + '/View').subscribe(
      (response: User) => {
        this.authService.updateUser();
        this.user$.next(response);
        this.birthdate = new Date(response.birthDate);
        if (this.loggedInUser) {
          this.checkFollow(this.loggedInUser.id);
        }
      },
      (error: any) => {
        console.log(error);
      }
    );
  }

  // check if the user follows this profile
  checkFollow(id: number) {
    const followers = this.user$.value?.followers;
    console.log('followers', followers);
    console.log('id to check', id);

    if (followers) {
      for (let i = 0; i < followers.length; i++) {
        if (followers[i].followerId === id) {
          this.isFollowing$.next(true);
        } else {
          this.isFollowing$.next(false);
        }
      }
    }
  }

  // follow
  followUser(username: string) {
    this.httpService.post('/Profile/' + username + '/follow', null).subscribe(
      (response) => {
        this.isFollowing$.next(true);
        this.loadUser(username);
      }
    );
  }

  // unfollow
  unfollowUser(username: string) {
    this.httpService.post('/Profile/' + username + '/unfollow',null).subscribe(
      (response) => {
        console.log('unfollow res:',response);
        this.isFollowing$.next(false);
        this.loadUser(username);
      }
    );
  }
  toggleFollow() {
    if (this.isFollowing$.value) {
      this.unfollowUser(this.user$.value?.userName || '');
      console.log('this follow', this.isFollowing$.value);

    } else {
      this.followUser(this.user$.value?.userName || '');
      console.log('this follow', this.isFollowing$.value);

    }
  }
}
