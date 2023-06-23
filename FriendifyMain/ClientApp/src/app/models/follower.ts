import {User} from "./user";

export interface Follower {
  user: User;
  following: User;
  userId: number;
  followerId: number;
  dateTime: Date;
  username: string;
}
