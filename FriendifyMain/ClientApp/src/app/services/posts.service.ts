import {Post} from "../models/post";
import {BehaviorSubject, map, Observable, of} from "rxjs";
import {Injectable} from "@angular/core";
import {HttpService} from "./http.service";
import {AuthService} from "./auth.service";

@Injectable({
  providedIn: 'root'
})
export class PostsService {
  private postsSubject = new BehaviorSubject<Post[]>([]);
  public posts$ = this.postsSubject.asObservable();
  private publicPostsSubject = new BehaviorSubject<Post[]>([]);
  public publicPosts$ = this.publicPostsSubject.asObservable();
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

  loadPublicPosts() {

    // clear the posts
    this.publicPostsSubject.next([]);

    let posts : Post[] = [];

    this.httpService.get('/Home/getallposts').subscribe(
      (response: any) => {
        console.log(response as Post[]);
        posts = response;
        this.publicPostsSubject.next(posts);
      },
      (error: any) => {
        console.log(error);
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

  addComment(postId: number, comment: string) {
    this.httpService.post('/Home/' + postId + '/comment?text=' +  comment, null).subscribe(
      (response: any) => {
        console.log(response);
      },
      (error: any) => {
        console.log(error);
      }
    )
  }

  // Load random posts from all users
  loadRandomPosts(): Observable<Post[]> {
    // Create an empty array of posts
    let posts: Post[] = [];

    // Get all posts from the server
    return this.httpService.get('/Home/getallposts').pipe(
      map((response: any) => {
        posts = response as Post[];

        // Shuffle the array of posts
        posts = this.shuffleArray(posts);

        return posts.slice(0, 10);
      })
    );
  }

  // Helper function to shuffle an array
  shuffleArray(array: any[]): any[] {
    for (let i = array.length - 1; i > 0; i--) {
      let j = Math.floor(Math.random() * (i + 1));

      [array[i], array[j]] = [array[j], array[i]];
    }

    return array;
  }


}
