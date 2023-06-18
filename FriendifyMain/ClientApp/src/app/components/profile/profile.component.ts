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
  user$: BehaviorSubject<User | undefined> = new BehaviorSubject<User | undefined>(undefined);
  loggedInUser: User | undefined = undefined;

  isFollowing$ = new BehaviorSubject(<boolean>false);
  isHovered: boolean = false;

  constructor(private httpService: HttpService, private authService: AuthService) {
    // Add code to load user data
    this.authService.user$.subscribe((user: User) => {
        this.loggedInUser = user;
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
        this.user$.next(response);
        if (this.loggedInUser) this.checkFollow(this.loggedInUser?.id);

      },
      (error: any) => {
        console.log(error);
      }
    );
  }

  // check if the user follows this profile
  checkFollow(id: number) {
    const followers = this.user$?.value?.followers;
    if (followers) {
      for (let i = 0; i < followers.length; i++) {
        if (followers[i].followerId === id) {
          this.isFollowing$.next(true);
          break;
        } else {
          this.isFollowing$.next(false);
        }
      }
    }
  }

  // follow
  followUser(id: number) {
    this.httpService.post('/Profile/' + id + '/follow', null).subscribe(
      (response) => {
        this.isFollowing$.next(true);
        this.loadUser(id);
      }
    );
  }

  // unfollow
  unfollowUser(id: number) {
    this.httpService.post('/Profile/' + id + '/unfollow',null).subscribe(
      (response) => {
        console.log('unfollow res:',response);
        this.isFollowing$.next(false);
        this.loadUser(id);
      }
    );
  }



  toggleFollow() {
    if (this.isFollowing$.value) {
      this.unfollowUser(this.user$.value?.id || 0);
      console.log('this follow', this.isFollowing$.value);

    } else {
      this.followUser(this.user$.value?.id || 0);
      console.log('this follow', this.isFollowing$.value);

    }
  }

}
