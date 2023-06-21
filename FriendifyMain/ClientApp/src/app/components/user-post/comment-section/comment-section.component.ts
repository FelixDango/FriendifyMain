import {Component, Input, OnInit} from '@angular/core';
import {PostsService} from "../../../services/posts.service";
import {Comment} from "../../../models/comment";
import {AuthService} from "../../../services/auth.service";

@Component({
  selector: 'app-comment-section',
  templateUrl: './comment-section.component.html',
  styleUrls: ['./comment-section.component.scss']
})
export class CommentSectionComponent implements OnInit {
  @Input() comments: Comment[] | undefined = undefined;
  @Input() postId: number = 0;
  @Input() postIsPublic: boolean = false;
  comment: string = '';
  userId: number;
  username: string;

  constructor(private postService: PostsService, private authService: AuthService) {
    this.userId = this.authService.getUserId();
    this.username = this.authService.getUserName();
  }

  ngOnInit(): void {
  }

  sortedComments(): Comment[] {
    if (this.comments == undefined) return [];
    return this.comments.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
  }

  addComment() {
    if (this.comment != '' && this.postId != 0) {
      this.postService.addComment(this.postId, this.comment);
      // fake comment for instant display
      this.comments?.push({
        id: 0,
        text: this.comment,
        date: new Date(),
        postId: this.postId,
        username: this.username,
        userId: this.userId} as Comment);
      this.comment = '';
    }
  }
}
