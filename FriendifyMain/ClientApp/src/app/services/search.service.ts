import { Injectable } from '@angular/core';
import { HttpService } from "./http.service";
import { Observable } from 'rxjs';
import { User } from "../models/user";
import { Post } from "../models/post";

@Injectable({
  providedIn: 'root'
})
export class SearchService {

  constructor(private httpService: HttpService) { }

  // find users by name
  findUser(name: string): Observable<User[]> {
    return this.httpService.get(`/search/finduser?name=${name}`);
  }

  // find posts by content
  findPost(content: string): Observable<Post[]> {
    return this.httpService.get(`/search/findpost?content=${content}`);
  }
}
