import {User} from "./user";

export interface Picture {
  id: number;
  userId: number;
  user: User;
  url?: string;
}
