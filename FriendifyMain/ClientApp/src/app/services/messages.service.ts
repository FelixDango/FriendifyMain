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
      console.log(messages);
      this.messages$.next(messages);
    })
  }
}
