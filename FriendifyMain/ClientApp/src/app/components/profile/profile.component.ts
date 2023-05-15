import {Component} from '@angular/core';
import {SexEnum, StatusEnum, User} from "../../models/user";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent {
  user: User = {
    id: 1,
    username: 'john_doe',
    firstName: 'John',
    lastName: 'Doe',
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
}
