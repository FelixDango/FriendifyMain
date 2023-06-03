import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
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
    },
    {
      title: 'Fourth post',
      content: 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce vestibulum tincidunt sem eget viverra. Sed tristique mi in ante semper, a sagittis lorem vestibulum. Proin iaculis convallis mauris vitae consequat. Nullam ac felis eget metus tincidunt lobortis id et lacus. Nam nec aliquam metus, id convallis nisi. Nulla nec metus in tellus posuere bibendum vel eget lorem. Etiam gravida quam sed urna condimentum, et tristique tortor luctus. Cras iaculis ex non feugiat fermentum. Quisque at risus non ante fermentum gravida. Vestibulum consectetur eros sit amet aliquet tristique. Quisque eget leo non felis eleifend consequat.',
      image: 'assets/media/cat.jpg',
      likes: 8,
      profilePicture: 'assets/media/profile.png',
      username: 'Emily Wilson'
    },
    {
      title: 'Fifth post',
      content: 'Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Pellentesque sed lorem sagittis, posuere risus in, semper tellus. Sed eget tristique lacus. Ut sollicitudin, mauris sit amet pharetra tempor, elit ante iaculis dolor, vel interdum velit mi in tortor. Fusce scelerisque turpis vitae nisi eleifend tristique. In sed neque fringilla, egestas arcu sed, pharetra tellus. Etiam ullamcorper consequat mauris, at congue neque pharetra vitae. Phasellus ut lectus rhoncus, malesuada leo ut, semper metus.',
      video: 'assets/media/cat.mp4',
      likes: 12,
      profilePicture: 'assets/media/profile.png',
      username: 'David Anderson'
    }
  ];



}
