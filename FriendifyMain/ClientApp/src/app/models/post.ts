import {User} from "./user";
import {Picture} from "./picture";
import {Video} from "./video";
import {Like} from "./like";
import {Comment} from "./comment";

export interface Post {
  id: number;
  userId: number;
  user: User;
  content: string;
  date: Date;
  likes: Like[];
  comments: Comment[];
  pictures: Picture[];
  videos: Video[];
  username: string;
  profilePicture: string;
}
