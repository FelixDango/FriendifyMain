import {Post} from "./post";
import {User} from "./user";

export interface Like {
  userId: number;
  user: User;
  postId: number;
  post: Post;
  dateTime: Date;
}
