import {Component, Input} from '@angular/core';
import {Message} from "../../models/message";
import {User} from "../../models/user";
import {AuthService} from "../../services/auth.service";
import {MessagesService} from "../../services/messages.service";
import {async, of} from "rxjs";

@Component({
  selector: 'app-private-messages',
  templateUrl: './private-messages.component.html',
  styleUrls: ['./private-messages.component.scss']
})
export class PrivateMessagesComponent {
  loadedMessages: Message[] = [] as Message[];
  user: User | undefined = undefined;
  message: string | undefined;
  messagedUser: string = '';


  constructor(private authService: AuthService, private messageService: MessagesService) {
    this.authService.user$.subscribe((user: User) => {
      this.user = user;
    })
    this.messageService.loadedMessages$.subscribe((messages: any) => {
        console.log('messages',messages);
        this.loadedMessages = messages;
      },
      (error: any) => {
        console.log('error',error);
      }
    )
    this.messageService.activeMessageTab$.subscribe((username: string) => {
      console.log('username',username);
      this.messagedUser = username;
      this.messageService.getMessagesByUsername(username);
    })
  }

  ngOnInit(): void {
    this.messageService.loadFollowers();
  }

  sendMessage(username: string) {
    // Add code to send a message
    if (this.message != undefined) {
      this.messageService.postMessage(this.message, username);
      this.messageService.getMessagesByUsername(username);
      this.message = '';
    }
  }

  protected readonly async = async;
  protected readonly of = of;
}
