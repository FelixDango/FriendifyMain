import {Component, Input} from '@angular/core';
import {Message} from "../../models/message";
import {BehaviorSubject} from "rxjs";
import {User} from "../../models/user";
import {AuthService} from "../../services/auth.service";
import {MessagesService} from "../../services/messages.service";

@Component({
  selector: 'app-private-messages',
  templateUrl: './private-messages.component.html',
  styleUrls: ['./private-messages.component.scss']
})
export class PrivateMessagesComponent {
  messages$: BehaviorSubject<Message[]> = new BehaviorSubject<Message[]>([])
  user: User | undefined = undefined;
  m: string | undefined;


  constructor(private authService: AuthService, private messageService: MessagesService) {
    this.authService.user$.subscribe((user: User) => {
      this.user = user;
    })
    this.messageService.messages$.subscribe((messages: any) => {
        console.log('messages',messages);
        this.messages$.next(messages);
      },
      (error: any) => {
        console.log('error',error);
      }
    )
  }

  sendMessage() {
    // Add code to send a message
    console.log('message',this.m);
  }

}
