import {Picture} from "./picture";
import {Follower} from "./follower";
import {Post} from "./post";

export interface User {
  id: number;
  username: string;
  firstName: string;
  lastName: string;
  password: string;
  birthDate: Date;
  email: string;
  suspended: boolean;
  sex: SexEnum;
  status: StatusEnum;
  picture: Picture | null;
  country: string | null;
  phoneNumber: string | null;
  follows: Follower[];
  followedBy: Follower[];
  posts: Post[];
  address: string | null;
  city: string | null;
  isModerator: boolean;
  isAdmin: boolean;
}

export enum SexEnum {
  Male = 'Male',
  Female = 'Female',
  Other = 'Other',
}

export enum StatusEnum {
  Single = 'Single',
  Relationship = 'Relationship',
  Married = 'Married',
}
