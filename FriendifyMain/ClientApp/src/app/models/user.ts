import {Picture} from "./picture";
import {Follower} from "./follower";
import {Post} from "./post";
import {Video} from "./video";
import {Message} from "./message";
import {Like} from "./like";

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  birthDate: Date;
  sex: SexEnum;
  status: StatusEnum;
  biography: string;
  suspended: boolean;
  picture: Picture;
  country?: string;
  phoneNumber?: string;
  following: Follower[];
  followers: Follower[];
  posts: Post[];
  comments: Comment[];
  images: Picture[];
  videos: Video[];
  messages: Message[];
  likes: Like[];
  registeredAt: Date;
  address?: string;
  city?: string;
  isModerator: boolean;
  isAdmin: boolean;
}

export enum SexEnum {
  Male,
  Female,
  Other
}

export enum StatusEnum {
  Single,
  Relationship,
  Married
}
