import { Component, OnInit } from '@angular/core';
import { Post } from '../../models/post';
import { PostmanageService } from '../../services/posts-management.service';
import { ConfirmationService, MessageService } from 'primeng/api';

@Component({
  selector: 'app-post-management',
  templateUrl: './post-management.component.html',
  styleUrls: ['./post-management.component.scss'],
  providers: [ConfirmationService, MessageService]
})
export class PostManagementComponent implements OnInit {

  // Declare the variables and properties
  posts: Post[] = []; // An array of posts to display
  selectedPost!: any; // The currently selected post
  displayDialog: boolean = false; // A flag to show or hide the dialog for editing a post
  cols: any[] = []; // An array of column definitions for the table

  constructor(
    private postService: PostmanageService, // Inject the post service
    private confirmationService: ConfirmationService, // Inject the confirmation service
    private messageService: MessageService // Inject the message service
  ) { }

  ngOnInit(): void {
    // Initialize the variables and properties
    this.posts = [];
    this.selectedPost = {};
    this.displayDialog = false;
    this.cols = [
      { field: 'id', header: 'Post ID' },
      { field: 'username', header: 'Username' },
      { field: 'content', header: 'Content' },
      { field: 'date', header: 'Date' }
    ];

    // Load the posts from the post service
    this.postService.getAllPosts().subscribe(
      data => {
        this.posts = data;
      },
      error => {
        console.log(error);
      }
    );
  }

  // A method to show the dialog for editing a post when a row is selected
  onRowSelect(event: { data: Post }) {
    this.selectedPost = event.data || {};
    this.displayDialog = true;
  }

  // A method to hide the dialog when it is closed
  onHide() {
    this.displayDialog = false;
  }

  // A method to save the changes made to a post when the save button is clicked
  save() {
    if (this.selectedPost) {
      this.postService.editPost(this.selectedPost.id, this.selectedPost).subscribe(
        data => {
          this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Post updated successfully' });
          this.displayDialog = false;
        },
        error => {
          console.log(error);
          this.messageService.add({ severity: 'error', summary: 'Error', detail: error.error });
        }
      );
    }
  }

  // A method to delete a post when the delete button is clicked
  delete(post: Post) {
    // Show the confirmation dialog
    this.confirmationService.confirm({
      key: 'confirmWeightTest',
      message: 'Are you sure that you want to delete this post?',
      accept: () => {
        // Update the selectedPost property with the current row data
        this.selectedPost = post.id;
        // Delete the post using the selectedPost property
        this.postService.deletePost(this.selectedPost).subscribe(
          data => {
              this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Post deleted successfully' });
              this.displayDialog = false;
              this.posts = this.posts.filter(p => p.id !== post.id); // Remove the user from the users array
            },
            error => {
              console.log(error);
              this.messageService.add({ severity: 'error', summary: 'Error', detail: error.error });
            }
          );
        }
      });
    }
  }
