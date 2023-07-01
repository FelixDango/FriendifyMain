// Import the necessary modules
import { Component, EventEmitter, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { ConfirmationService } from 'primeng/api'; // A service that provides confirmation dialogs
import { MessageService } from 'primeng/api'; // A service that provides toast messages
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';

interface Role {
  name: string;
}

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss'],
  providers: [ConfirmationService, MessageService] // Inject the services
})
export class UserManagementComponent implements OnInit {

  users: User[] = []; // An array of users to display in a table
  selectedUser: User | undefined; // The user that is currently selected in the table

  roles: Role[] = [
    { name: 'User' },
    { name: 'Moderator' },
    { name: 'Admin' },
  ];
  selectedRole = this.roles[0].name;



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
        this.users = users; // Assign the users to the component 
      },
      (error: any) => {
        console.error(error); // Handle any possible errors
      }
    );
    
  }

  suspendUser(user: User): void {
    this.confirmationService.confirm({ // Use the confirmation service to show a dialog
      message: `Are you sure you want to suspend ${user.userName}?`,
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      key: 'confirmWeightTest',
      accept: () => {
        this.userService.suspendUser(user.userName).subscribe( // Subscribe to the observable returned by the user service
          (response: { message: any; }) => {
            user.suspended = true; // Update the user property
            this.messageService.add({ severity: 'success', summary: 'Success', detail: `User ${user.userName} has been suspended successfully!` }); // Show a success message

          },
          (error: { message: any; }) => {
            console.error(error); // Handle any possible errors
            this.messageService.add({ severity: 'error', summary: 'Error', detail: error.message }); // Show an error message
          }
        );
      },
      reject: () => {
        // Do something when rejected
      }
    });
  }

  unsuspendUser(user: User): void {
    this.confirmationService.confirm({ // Use the confirmation service to show a dialog
      message: `Are you sure you want to unsuspend ${user.userName}?`,
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      key: 'confirmWeightTest',
      accept: () => {
        this.userService.unsuspendUser(user.userName).subscribe( // Subscribe to the observable returned by the user service
          (response: { message: any; }) => {
            user.suspended = false; // Update the user property
            this.messageService.add({ severity: 'success', summary: 'Success', detail: `User ${user.userName} has been unsuspended successfully!` }); // Show a success message
          },
          (error: { message: any; }) => {
            console.error(error); // Handle any possible errors
            this.messageService.add({ severity: 'error', summary: 'Error', detail: error.message }); // Show an error message
          }
        );
      },
      reject: () => {
        // Do something when rejected
      }
    });
    

  }

  assignRole(user: User, role: string): void {
    this.confirmationService.confirm({ // Use the confirmation service to show a dialog
      message: `Are you sure you want to assign ${role} role to ${user.userName}?`,
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      key: 'confirmWeightTest',
      accept: () => {
        this.userService.assignRole(user.userName, role).subscribe( // Subscribe to the observable returned by the user service
          (response: { message: any; }) => {
            user.isAdmin = role === 'Admin'; // Update the user property
            user.isModerator = role === 'Moderator'; // Update the user property
            this.messageService.add({ severity: 'success', summary: 'Success', detail: `Role ${role} assigned successfully to ${user.userName}!` }); // Show a success message
          },
          (error: { message: any; }) => {
            console.error(error); // Handle any possible errors
            this.messageService.add({ severity: 'error', summary: 'Error', detail: error.message }); // Show an error message
          }
        );
      },
      reject: () => {
        // Do something when rejected
      }
    });
  }

  // A method that deletes a user by their username
  removeUser(user: User): void {
    this.confirmationService.confirm({ // Use the confirmation service to show a dialog
      message: `Are you sure you want to delete ${user.userName}?`,
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      key: 'confirmWeightTest',
      accept: () => {
        this.userService.deleteUser(user.userName).subscribe( // Subscribe to the observable returned by the user service
          (response: { message: any; }) => {
            this.users = this.users.filter(u => u.id !== user.id); // Remove the user from the users array
            this.messageService.add({ severity: 'success', summary: 'Success', detail: `User ${user.userName} has been deleted successfully!` }); // Show a success message
          },
          (error: { message: any; }) => {
            console.error(error); // Handle any possible errors
            this.messageService.add({ severity: 'error', summary: 'Error', detail: error.message }); // Show an error message
          }
        );

      },
       reject: () => {
        // Do something when rejected
      }
    });
  }
 

}
