import {User} from "./user";
import {Picture} from "./picture";
import {Video} from "./video";

export interface Post {
  id: number;
  userId: number;
  content: string;
  date: Date;
  likedBy: User[];
  comments: Comment[];
  pictures: Picture[];
  videos: Video[];
}
