import { Component } from '@angular/core';

@Component({
  selector: 'app-profile-page',
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.scss']
})
export class ProfilePageComponent {
  userPosts = [
    { title: 'My First post', content: 'This is the first post of the User.' },
    { title: 'My Second post', content: 'This is the second post of the User.' },
    { title: 'My Third post', content: 'This is the third post of the User.' }
  ]
}
