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
  sex: number = 0;
  status: number | undefined;
  biography: string | undefined;
  picture: Blob | undefined;
  fileUpload: any;



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
    const file: Blob = event.files;
    const allowedImageTypes = ['image/jpeg', 'image/png','image/jpg' ];

    if (allowedImageTypes.includes(file.type)) {
      this.picture = file;
    }

    this.fileUpload = fileUpload;
    console.log(this.picture);
  }

  updateProfile() {
    // Add code to update the user profile
    const updateData = {
      username: this.username,
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email,
      birthdate: this.birthdate,
      sex: parseInt(String(this.sex), 10),
      status: 0,
      biography: this.biography,
      pictureUrl: this.picture,
    };

    this.httpService.put('/Profile/' + this.user?.userName + '/update', updateData).subscribe(
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
