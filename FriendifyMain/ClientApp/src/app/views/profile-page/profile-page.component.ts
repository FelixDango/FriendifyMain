import { Component } from '@angular/core';

@Component({
  selector: 'app-profile-page',
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.scss']
})
export class ProfilePageComponent {
  userPosts = [
    {
      title: 'First post',
      content: 'This is the first Twitter post.',
      image: 'assets/media/cat.jpg',
      likes: 10,
      profilePicture: 'assets/media/profile.png',
      username: 'John Doe'
    },
    {
      title: 'Second post',
      content: 'This is the second Twitter post.',
      video: 'assets/media/cat.mp4',
      likes: 5,
      profilePicture: 'assets/media/profile.png',
      username: 'Jane Smith'
    },
    {
      title: 'Third post',
      content: 'This is the third Twitter post.',
      image: 'assets/media/cat.jpg',
      likes: 2,
      profilePicture: 'assets/media/profile.png',
      username: 'Alex Johnson'
    }
  ];
}
