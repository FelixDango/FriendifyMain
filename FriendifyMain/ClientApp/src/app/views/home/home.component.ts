import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  userPosts = [
    { title: 'First post', content: 'This is the first Twitter post.' },
    { title: 'Second post', content: 'This is the second Twitter post.' },
    { title: 'Third post', content: 'This is the third Twitter post.' }
  ]
}
