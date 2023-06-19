import { Injectable } from '@angular/core';
import {BehaviorSubject} from "rxjs";
import {Message} from "../models/message";


@Injectable({
  providedIn: 'root'
})
export class MessagesService {
  // add behavior subject to store messages
  messages$ = new BehaviorSubject<Message[]>([]);


  constructor() { }
}
