import {User} from "./user";
import {Picture} from "./picture";
import {Video} from "./video";
import {Like} from "./like";

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
}
