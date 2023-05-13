import {User} from "./user";

export interface Role {
  id: number;
  name: string;
  assignedRoles: AssignedRole[];
  // Add other properties as needed
}

export interface AssignedRole {
  id: number;
  userId: number;
  roleId: number;
  user: User;
  role: Role;
}
