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
  user: User = {
    id: 1,
    username: 'john_doe',
    firstName: 'John',
    lastName: 'Doe',
    bio: 'This is my bio.',
    password: 'password',
    birthDate: new Date('1990-01-01'),
    email: 'john.doe@example.com',
    suspended: false,
    sex: SexEnum.Male,
    status: StatusEnum.Single,
    picture: null,
    country: 'USA',
    phoneNumber: '+1 1234567890',
    follows: [],
    followedBy: [],
    posts: [],
    address: '123 Main St',
    city: 'New York',
    isModerator: false,
    isAdmin: false
  };

  isFollowing: boolean = false;
  isHovered: boolean = false;

  constructor(private httpService: HttpService, private authService: AuthService) {}

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
