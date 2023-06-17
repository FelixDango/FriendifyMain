import {Component, Input, OnInit} from '@angular/core';
import {SexEnum, StatusEnum, User} from "../../models/user";
import {HttpService} from "../../services/http.service";
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  @Input() id: number | undefined;
  user: User | undefined = undefined;

  isFollowing: boolean = false;
  isHovered: boolean = false;

  constructor(private httpService: HttpService, private authService: AuthService) {
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      if (this.id) this.loadUser(this.id)
    }
  }

  loadUser(userId: number) {
    // Add code to load user data
    this.httpService.get('/Profile/' + userId + '/view').subscribe(
      (response: any) => {
        this.user = response;
        console.log('USER', this.user);
      },
      (error: any) => {
        console.log(error);
      }
    );
  }


  toggleFollow() {
    this.isFollowing = !this.isFollowing;
    if (this.isFollowing) {
      // Follow logic
      // Add code to handle follow functionality
    } else {
      // Unfollow logic
      // Add code to handle unfollow functionality
    }
  }

}
