import {Component, OnInit} from '@angular/core';
import {HttpService} from "../../services/http.service";
import {AuthService} from "../../services/auth.service";
import {User} from "../../models/user";

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss']
})
export class MessagesComponent implements OnInit {
  user: User | undefined = undefined;
  constructor(private httpService: HttpService, private authService: AuthService) {
    this.authService.user$.subscribe((user: User) => {
      this.user = user;
      console.log('user',this.user);
    })
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.authService.updateUser();
      this.getMessages();
      this.postMessage();
    }
    console.log('Messages component initialized');
  }

  // Add code to get the messages for the current user
  getMessages() {
    this.httpService.get('/Message').subscribe((messages: any) => {
      console.log(messages);
    })
  }

  postMessage() {
    // Add code to post a message
    const message = {
      content: 'Hello world',
      receiverUsername: 'Admin'
    };
    this.httpService.post('/Message', message).subscribe((response: any) => {
      console.log(response);
    })
  }
}
