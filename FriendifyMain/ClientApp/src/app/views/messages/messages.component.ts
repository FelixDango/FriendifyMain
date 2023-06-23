import {Component, OnInit} from '@angular/core';
import {HttpService} from "../../services/http.service";
import {AuthService} from "../../services/auth.service";
import {User} from "../../models/user";
import {MessagesService} from "../../services/messages.service";
import {BehaviorSubject} from "rxjs";
import {Message} from "../../models/message";

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {
  user: User | undefined = undefined;

  constructor(private httpService: HttpService, private authService: AuthService, private messageService: MessagesService) {
    this.authService.user$.subscribe((user: User) => {
      this.user = user;
    })



  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.authService.updateUser();
      //this.postMessage();
      this.messageService.getMessages();
    }
    console.log('Messages component initialized');
  }

  // Add code to get the messages for the current user


  postMessage() {
    // Add code to post a message
    const message = {
      content: 'Hello world',
      receiverUsername: 'Test12345'
    };
    this.httpService.post('/Message', message).subscribe((response: any) => {
      console.log(response);
    })
  }
}
