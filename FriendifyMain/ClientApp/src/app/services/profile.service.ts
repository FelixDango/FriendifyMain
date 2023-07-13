import { Injectable } from '@angular/core';
import {BehaviorSubject, ReplaySubject} from "rxjs";
import {HttpService} from "./http.service";
import {AuthService} from "./auth.service";
import {User} from "../models/user";

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  loadedProfile$ = new BehaviorSubject<User>({} as User);
  currentUserProfile$ = new ReplaySubject<User>( 1);
  loggedInUser: User | undefined = undefined;

  constructor(private httpService: HttpService, private authService: AuthService) {
    this.authService.user$.subscribe((user: User) => {
      if (user) this.loggedInUser = user;
    });

  }

  loadProfile(username: string) {
    this.httpService.get('/Profile/' + username + '/View').subscribe(
      (response: any) => {
        this.loadedProfile$.next(response);
      },
      (error: any) => {
        console.log(error);
      }
    );
  }

}
