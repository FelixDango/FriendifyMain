// Import the necessary modules
import { Component, OnInit } from '@angular/core';
import { ConfirmationService } from 'primeng/api'; // A service that provides confirmation dialogs
import { MessageService } from 'primeng/api'; // A service that provides toast messages
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
  providers: [ConfirmationService, MessageService] // Inject the services
})
export class UserManagementComponent implements OnInit {

  users: User[] = []; // An array of users to display in a table
  selectedUser: User | undefined; // The user that is currently selected in the table
  roles: string[] = ['Admin', 'Moderator']; // The possible roles to assign to a user

  constructor(
    private userService: UserService, // Inject the user service
    private confirmationService: ConfirmationService, // Inject the confirmation service
    private messageService: MessageService // Inject the message service
  ) { }

  ngOnInit(): void {
    this.getUsers(); // Get the users from the user service when the component is initialized
  }

  getUsers(): void {
    this.userService.getUsers().subscribe( // Subscribe to the observable returned by the user service
      (users: User[]) => {
        this.users = users; // Assign the users to the component property
      },
      (error: any) => {
        console.error(error); // Handle any possible errors
      }
    );
  }

  suspendUser(user: User): void {
    this.confirmationService.confirm({ // Use the confirmation service to show a dialog
      message: `Are you sure you want to suspend ${user.userName}?`,
      accept: () => {
        this.userService.suspendUser(user.userName).subscribe( // Subscribe to the observable returned by the user service
          (response: { message: any; }) => {
            user.suspended = true; // Update the user property
            this.messageService.add({ severity: 'success', summary: 'Success', detail: response.message }); // Show a success message
          },
          (error: { message: any; }) => {
            console.error(error); // Handle any possible errors
            this.messageService.add({ severity: 'error', summary: 'Error', detail: error.message }); // Show an error message
          }
        );
      }
    });
  }

  unsuspendUser(user: User): void {
    this.confirmationService.confirm({ // Use the confirmation service to show a dialog
      message: `Are you sure you want to unsuspend ${user.userName}?`,
      accept: () => {
        this.userService.unsuspendUser(user.userName).subscribe( // Subscribe to the observable returned by the user service
          (response: { message: any; }) => {
            user.suspended = false; // Update the user property
            this.messageService.add({ severity: 'success', summary: 'Success', detail: response.message }); // Show a success message
          },
          (error: { message: any; }) => {
            console.error(error); // Handle any possible errors
            this.messageService.add({ severity: 'error', summary: 'Error', detail: error.message }); // Show an error message
          }
        );
      }
    });
  }

  assignRole(user: User, role: string): void {
    this.confirmationService.confirm({ // Use the confirmation service to show a dialog
      message: `Are you sure you want to assign ${role} role to ${user.userName}?`,
      accept: () => {
        this.userService.assignRole(user.userName, role).subscribe( // Subscribe to the observable returned by the user service
          (response: { message: any; }) => {
            user.isAdmin = role === 'Admin'; // Update the user property
            user.isModerator = role === 'Moderator'; // Update the user property
            this.messageService.add({ severity: 'success', summary: 'Success', detail: response.message }); // Show a success message
          },
          (error: { message: any; }) => {
            console.error(error); // Handle any possible errors
            this.messageService.add({ severity: 'error', summary: 'Error', detail: error.message }); // Show an error message
          }
        );
      }
    });
  }

  removeRole(user: User, role: string): void {
    this.confirmationService.confirm({ // Use the confirmation service to show a dialog
      message: `Are you sure you want to remove ${role} role from ${user.userName}?`,
      accept: () => {
        this.userService.removeRole(user.userName, role).subscribe( // Subscribe to the observable returned by the user service
          (response: { message: any; }) => {
            user.isAdmin = false; // Update the user property
            user.isModerator = false; // Update the user property
            this.messageService.add({ severity: 'success', summary: 'Success', detail: response.message }); // Show a success message
          },
          (error: { message: any; }) => {
            console.error(error); // Handle any possible errors
            this.messageService.add({ severity: 'error', summary: 'Error', detail: error.message }); // Show an error message
          }
        );
      }
    });
  }

  // A method that deletes a user by their username
  removeUser(user: User): void {
    this.confirmationService.confirm({ // Use the confirmation service to show a dialog
      message: `Are you sure you want to delete ${user.userName}?`,
      accept: () => {
        this.userService.deleteUser(user.userName).subscribe( // Subscribe to the observable returned by the user service
          (response: { message: any; }) => {
            this.users = this.users.filter(u => u.id !== user.id); // Remove the user from the users array
            this.messageService.add({ severity: 'success', summary: 'Success', detail: response.message }); // Show a success message
          },
          (error: { message: any; }) => {
            console.error(error); // Handle any possible errors
            this.messageService.add({ severity: 'error', summary: 'Error', detail: error.message }); // Show an error message
          }
        );
      }
    });
  }


}
