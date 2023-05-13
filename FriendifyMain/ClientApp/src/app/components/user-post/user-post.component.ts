import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-user-post',
  templateUrl: './user-post.component.html',
  styleUrls: ['./user-post.component.scss']
})
export class UserPostComponent {
  @Input() post: any; // Change the type as per your data structure

  likePost() {
    // Implement the logic to like the post here
  }
}
