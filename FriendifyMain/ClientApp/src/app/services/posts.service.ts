import {Post} from "../models/post";
import {BehaviorSubject, Observable, of} from "rxjs";
import {Injectable} from "@angular/core";
import {HttpService} from "./http.service";
import {AuthService} from "./auth.service";

@Injectable({
  providedIn: 'root'
})
export class PostsService {
  private postsSubject = new BehaviorSubject<Post[]>([]);
  public posts$ = this.postsSubject.asObservable();
  private userPostsSubject = new BehaviorSubject<Post[]>([]);
  public userPosts$ = this.userPostsSubject.asObservable();

  constructor(private httpService: HttpService, private authService: AuthService) {}

  loadPosts() {

    // clear the posts
    this.postsSubject.next([]);

    let posts : Post[] = [];

    this.httpService.get('/Home').subscribe(
      (response: any) => {
        posts = response;
        this.postsSubject.next(posts);
      },
      (error: any) => {
        if (error.status === 401) {
          this.authService.logout();
        }
      }
    )
  }

  loadUserPosts(username: string) {

    // clear the posts
    this.userPostsSubject.next([]);

    let posts : Post[] = [];

    this.httpService.get('/Profile/'+username+'/view').subscribe(
      (response: any) => {

        posts = response.posts as Post[];
        this.userPostsSubject.next(posts);
      },
      (error: any) => {
        if (error.status === 401) {
          this.authService.logout();
        }
      }
    )
  }

}
