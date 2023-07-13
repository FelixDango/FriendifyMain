import { Component } from '@angular/core';
import {MessagesService} from "../../services/messages.service";
import {Message} from "../../models/message";
import {User} from "../../models/user";
import {Follower} from "../../models/follower";
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  following: Follower[] = [];
  messagedUsernames: string[] = [];
  user: User | undefined = undefined;
  activeMessageTab: string = "";
  followingMessage: Follower | undefined = undefined;


  constructor(private messagesService: MessagesService, private authService: AuthService) {
    this.messagesService.following$.subscribe((following) => {
      this.following = following;
    })
    this.messagesService.messages$.subscribe((messages) => {
      this.messagedUsernames = this.getMessagedUsernames(messages as Message[]);
    })
    this.authService.user$.subscribe((user: User) => {
      this.user = user;
    })
    this.messagesService.activeMessageTab$.subscribe((username: string) => {
      this.activeMessageTab = username;
    }
    )
  }

  ngOnInit(): void {
    this.messagesService.loadFollowers();
  }

  loadMessages(username: string) {
    this.messagesService.getMessagesByUsername(username);
    this.messagesService.activeMessageTab$.next(username);
  }

  getMessagedUsernames(messages: Message[]): string[] {
    const usernames = new Set<string>();
    messages.forEach(item => {
      // add sender usernames
      if (item.username !== this.user?.userName) {
        usernames.add(item.username);
      }
      // add receiver usernames
      if (item.receiverUsername !== this.user?.userName) {
        usernames.add(item.receiverUsername);
      }
    });
    if (usernames.size > 0) {
      this.messagesService.activeMessageTab$.next(Array.from(usernames)[0]);
    }
    return Array.from(usernames);
  }
}
