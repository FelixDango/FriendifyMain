import { Injectable } from '@angular/core';
import {BehaviorSubject, ReplaySubject} from "rxjs";
import {Message} from "../models/message";
import {HttpService} from "./http.service";
import {User} from "../models/user";
import {AuthService} from "./auth.service";
import {Follower} from "../models/follower";


@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  // add behavior subject to store messages
  messages$ = new BehaviorSubject<Message[]>([]);
  following$ = new BehaviorSubject<Follower[]>([]);
  loadedMessages$ = new BehaviorSubject<Message[]>([])
  activeMessageTab$ = new ReplaySubject<string>(1);
  user: User | undefined = undefined;


  constructor(private httpService: HttpService, private authService: AuthService) {
    this.authService.user$.subscribe((user: User) => {
      this.user = user;
    })
  }


  getMessages() {
    this.httpService.get('/Message').subscribe((messages: any) => {
      this.messages$.next(messages);
    })
  }

  loadFollowers() {
    if (this.user !== undefined) {
      this.httpService.get('/Profile/' + this.user?.userName + '/view').subscribe((response: any) => {
        this.following$.next(response.following);
      })
    }
  }

  postMessage(message: string, receiverUsername: string) {
    const m = {
      "id": 0,
      "userId": 0,
      "receiverId": 0,
      "content": message,
      "username": receiverUsername,
      "receiverUsername": receiverUsername
    } as Message;
    this.httpService.post('/Message', {content: message, ReceiverUsername: receiverUsername}).subscribe((response: any) => {
      this.activeMessageTab$.next(receiverUsername);
      this.messages$.next([response, ...this.messages$.value]);
      }, (error: any) => {
        console.log(error);
      }
    )
  }


  getMessagesByUsername(username: string) {
    this.httpService.get('/Message/' + username + '/messages').subscribe((messages: any) => {
      this.loadedMessages$.next(messages);
    })
  }
}
