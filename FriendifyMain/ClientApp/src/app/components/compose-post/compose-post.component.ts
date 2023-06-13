import { Component } from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {HttpService} from "../../services/http.service";

@Component({
  selector: 'app-compose-post',
  templateUrl: './compose-post.component.html',
  styleUrls: ['./compose-post.component.scss']
})
export class ComposePostComponent {
  selectedFile: File | null = null;
  text: string = '';
  characterCount: number = 0;
  isOverLimit: boolean = false;
  postContent: string = '';
  errorMessage: string = '';

  constructor(private httpService: HttpService, private authService: AuthService) {}

  handleFileInput(event: any) {
    const file: File = event.files[0];
    const allowedTypes = ['image/jpeg', 'image/png', 'image/gif', 'video/mp4', 'video/avi'];

    if (file && allowedTypes.includes(file.type)) {
      this.selectedFile = file;
      this.errorMessage = '';
    } else {
      this.selectedFile = null;
      this.errorMessage = 'Invalid file format. Only images (JPEG, PNG, GIF) and videos (MP4, AVI) are allowed.';
    }
    console.log('file',this.selectedFile);

  }

  handleTextInput(event: any): void {
    this.characterCount = this.text.length;
    this.isOverLimit = this.characterCount > 281;
    if (!this.isOverLimit) this.text = event.target.value;
  }

  submitPost() {
    // Perform post submission logic here
    this.httpService.post('/Home/createpost', { Content: this.text, Pictures: [], Videos: []}).subscribe((response) => {
      console.log('response', response);
    }, (error) => {
      console.log('error', error);
      }
    );
    // You can access the entered text using a template reference variable or ngModel
    // The selected file can be accessed via this.selectedFile
    console.log('Text:', this.text);
    console.log('Selected File:', this.selectedFile);
  }
}
