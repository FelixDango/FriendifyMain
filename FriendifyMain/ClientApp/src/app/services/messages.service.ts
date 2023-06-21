import { Injectable } from '@angular/core';
import {BehaviorSubject} from "rxjs";
import {Message} from "../models/message";
import {HttpService} from "./http.service";


@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  // add behavior subject to store messages
  messages$ = new BehaviorSubject<Message[]>([]);


  constructor(private httpService: HttpService) { }

  getMessages() {
    this.httpService.get('/Message').subscribe((messages: any) => {
      console.log('res get message',messages);
      this.messages$.next(messages);
    })
  }

  postMessage(message: string, receiverUsername: string) {
    this.httpService.post('/Message', {content: message, ReceiverUsername: receiverUsername}).subscribe((response: any) => {
        console.log('res post message',response);
        this.getMessages();
      }, (error: any) => {
        console.log(error);
      }
    )
  }

  getMessagedUsernames() {

  }
}
