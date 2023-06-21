import {Component, Input, OnInit} from '@angular/core';
import {PostsService} from "../../../services/posts.service";
import {Comment} from "../../../models/comment";

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

  constructor(private postService: PostsService) { }

  ngOnInit(): void {
  }

  addComment() {
    if (this.comment != '' && this.postId != 0) {
      this.postService.addComment(this.postId, this.comment);
      this.comment = '';
    }
  }
}
