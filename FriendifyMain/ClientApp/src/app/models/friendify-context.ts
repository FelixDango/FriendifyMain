import {User} from "./user";
import {Post} from "./post";
import {AssignedRole, Role} from "./role";


export interface FriendifyContext {
  users: User[];
  posts: Post[];
  roles: Role[];
  assignedRoles: AssignedRole[];
  // Add other properties as needed
}
