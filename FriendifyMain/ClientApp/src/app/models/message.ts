import {Video} from "./video";
import {Picture} from "./picture";
import {User} from "./user";

export interface Message {
  id: number;
  userId: number;
  user: User;
  receiverId: number;
  receiver: User;
  content: string;
  date: Date;
  pictures: Picture[];
  videos: Video[];
}
