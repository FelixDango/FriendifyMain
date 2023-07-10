import {Component, OnInit} from '@angular/core';
import {AuthService} from "../../services/auth.service";
import {User} from "../../models/user";
import {HttpService} from "../../services/http.service";

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.scss']
})
export class EditProfileComponent implements OnInit{
  user: User | undefined = undefined;

  username: string | undefined;
  firstName: string | undefined;
  lastName: string | undefined;
  email: string | undefined;
  birthdate: Date | undefined;
  sex: number | undefined = undefined;
  status: number | undefined = undefined;
  biography: string | undefined;
  picture: Blob | undefined;
  fileUpload: any;
  formData = new FormData();



  constructor(private authService: AuthService, private httpService: HttpService) {
    this.authService.user$.subscribe((user: User) => {
      this.user = user;
    })

  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.authService.updateUser();
    }
  }


  handleFileInput(event: any, fileUpload: any) {
    console.log(event);
    const file: Blob = event.files[0];
    const allowedImageTypes = ['image/jpeg', 'image/png','image/jpg' ];

    if (allowedImageTypes.includes(file.type)) {
      this.picture = file;
    }

    this.fileUpload = fileUpload;
    console.log('pic',this.picture);
  }

  updateProfile() {
    // Add code to update the user profile
    if (this.username) this.formData.append('Username', this.username);
    if (this.firstName) this.formData.append('FirstName', this.firstName);
    if (this.lastName) this.formData.append('LastName', this.lastName);
    if (this.email) this.formData.append('Email', this.email);
    if (this.birthdate) this.formData.append('BirthDate', this.birthdate.toISOString());
    if (this.sex) this.formData.append('Sex', String(this.sex));
    if (this.status) this.formData.append('Status', String(this.status));
    if (this.biography) this.formData.append('Biography', this.biography);
    if (this.picture) this.formData.append('PictureFile', this.picture);

    this.httpService.put('/Profile/' + this.user?.userName + '/update', this.formData).subscribe(
      (response: any) => {
        console.log(response);
        // Handle successful registration response
      },
      (error: any) => {
        // Handle registration error
        console.error(error);
      }
    );
  }
}
