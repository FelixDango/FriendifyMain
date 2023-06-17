import {User} from "./user";
import {Post} from "./post";

export interface Comment {
  id: number;
  postId: number;
  post: Post;
  userId: number;
  user: User;
  text: string;
  date: Date;
}
