import {Component, Injectable} from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {HttpService} from "../../services/http.service";
import {PostsService} from "../../services/posts.service";

@Component({
  selector: 'app-compose-post',
  templateUrl: './compose-post.component.html',
  styleUrls: ['./compose-post.component.scss']
})


  export class ComposePostComponent {
  text: string = '';
  characterCount: number = 0;
  isOverLimit: boolean = false;
  postContent: string = '';
  errorMessage: string = '';
  uploadedImages: Blob[] = [];
  uploadedVideos: Blob[] = [];
  formData = new FormData();
  fileUpload: any;


  constructor(private httpService: HttpService, private authService: AuthService, private postsService: PostsService) {
  }

  handleFileInput(event: any, fileUpload: any) {
    const files: Blob[] = event.files;
    const allowedImageTypes = ['image/jpeg', 'image/png', 'image/gif'];
    const allowedVideoTypes = ['video/mp4', 'video/avi'];

    for (let file of files) {
      if (allowedImageTypes.includes(file.type)) {
        this.uploadedImages.push(file)
      } else if (allowedVideoTypes.includes(file.type)) {
        this.uploadedVideos.push(file);
      } else {
        this.errorMessage = 'Invalid file format. Only images (JPEG, PNG, GIF) and videos (MP4, AVI) are allowed.';
      }
    }
    this.fileUpload = fileUpload;
  }


  handleTextInput(event: any): void {
    this.characterCount = this.text.length;
    this.isOverLimit = this.characterCount > 281;
    if (!this.isOverLimit) this.text = event.target.value;
  }

  submitPost() {
    this.formData.append('content', this.text);
    for (let file of this.uploadedVideos) {
      this.formData.append('videoFiles', file);
    }
    for (let file of this.uploadedImages) {
      this.formData.append('pictureFiles', file);
    }
    this.httpService.post('/Home/createpost', this.formData).subscribe(
      (response: any) => {
        this.text = '';
        this.characterCount = 0;
        this.isOverLimit = false;
        this.postContent = '';
        this.errorMessage = '';
        this.uploadedImages = [];
        this.uploadedVideos = [];
        this.formData = new FormData();
        const textarea = document.getElementById('postContent') as HTMLTextAreaElement;
        textarea.value = '';
        this.postsService.loadPosts();
        this.fileUpload.clear();
      },
      (error) => {
        if (error.status === 401) this.authService.logout();
      }
    );
  }
}
