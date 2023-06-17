import {User} from "./user";

export interface Video {
  id: number;
  userId: number;
  user: User;
  url: string;
}
