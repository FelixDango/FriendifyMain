import { Component } from '@angular/core';
import {MessagesService} from "../../services/messages.service";
import {Message} from "../../models/message";
import {User} from "../../models/user";
import {Follower} from "../../models/follower";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  following: Follower[] = [];
  messagedUsernames: string[] = [];


  constructor(private messagesService: MessagesService) {
    this.messagesService.following$.subscribe((following) => {
      console.log('following', following);
      this.following = following;
    })
    this.messagesService.messages$.subscribe((messages) => {
      console.log('messages', messages);
      this.messagedUsernames = this.getMessagedUsernames(messages as Message[]);
    })
  }

  loadMessages(username: string) {
    this.messagesService.getMessagesByUsername(username);
  }

  getMessagedUsernames(messages: Message[]): string[] {
    const usernames = new Set<string>();
    messages.forEach(item => {
      usernames.add(item.username);
    });
    return Array.from(usernames);
  }


}
