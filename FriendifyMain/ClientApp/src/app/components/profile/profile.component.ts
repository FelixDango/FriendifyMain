import {Component, Input, OnInit} from '@angular/core';
import {SexEnum, StatusEnum, User} from "../../models/user";
import {HttpService} from "../../services/http.service";
import {AuthService} from "../../services/auth.service";
import {BehaviorSubject} from "rxjs";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  @Input() id: number | undefined;
  user: User | undefined = undefined;
  loggedInUser: User | undefined = undefined;

  isFollowing$ = new BehaviorSubject(<boolean>false);
  isHovered: boolean = false;

  constructor(private httpService: HttpService, private authService: AuthService) {
    // Add code to load user data
    this.authService.user$.subscribe((user: User) => {
        this.loggedInUser = user;
        console.log('LOGGED IN USER', this.loggedInUser);
      },
      (error: any) => {
        console.log(error);
      }
    );
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      if (this.id) {
        this.loadUser(this.id)
        this.authService.updateUser();
      }


    }
  }

  loadUser(userId: number) {
    // Add code to load user data
    this.httpService.get('/Profile/' + userId + '/view').subscribe(
      (response: any) => {
        this.user = response;
        console.log('this USER', this.user?.followers);
        if (this.loggedInUser) this.checkFollow(this.loggedInUser?.id);

      },
      (error: any) => {
        console.log(error);
      }
    );
  }

  // check if the user follows this profile
  checkFollow(id: number) {
    const followers = this.user?.followers;
    console.log('followers', followers);
    if (followers) {
      for (let i = 0; i < followers.length; i++) {
        if (followers[i].followerId === id) {
          this.isFollowing$.next(true);
          console.log('isFollowing', this.isFollowing$.value);
          break;
        } else {
          this.isFollowing$.next(false);
          console.log('isFollowing', this.isFollowing$.value);
        }
      }
    }
  }

  // follow
  followUser(id: number) {
    this.httpService.post('/Profile/' + id + '/follow', {}).subscribe(
      (response: any) => {
        console.log('follow res:',response);
        this.isFollowing$.next(true);
      },
      (error: any) => {
        console.log(error);
      }
    );
  }

  // unfollow
  unfollowUser(id: number) {
    this.httpService.post('/Profile/' + id + '/unfollow', {}).subscribe(
      (response: any) => {
        console.log('unfollow res:',response);
        this.isFollowing$.next(false);
      },
      (error: any) => {
        console.log(error);
      }
    );
  }



  toggleFollow() {
    if (this.isFollowing$.value) {
      this.unfollowUser(this.user?.id || 0);
    } else {
      this.followUser(this.user?.id || 0);
    }
  }

}
