import {AfterViewInit, Component, Input, OnChanges, OnInit, ViewChild, ElementRef} from '@angular/core';
import {HttpService} from "../../services/http.service";
import {AuthService} from "../../services/auth.service";
import {User} from "../../models/user";
import {Post} from "../../models/post";
import {BehaviorSubject, Observable} from "rxjs";
import {PostsService} from "../../services/posts.service";
import {Like} from "../../models/like";

@Component({
  selector: 'app-user-post',
  templateUrl: './user-post.component.html',
  styleUrls: ['./user-post.component.scss']
})
export class UserPostComponent implements OnInit, AfterViewInit {
  @ViewChild('cardText', { static: true }) cardTextRef!: ElementRef;
  private readonly minFontSize = 3;
  private readonly maxFontSize = 1.5;
  private readonly minCharacters = 20;
  private readonly maxCharacters = 281;
  user: any = this.authService.user$;
  assetsUrl: string = this.httpService.assetsUrl;
  postId: number = 0;
  @Input() postIsPublic: boolean = false;
  // post as behavior subject
  post$: BehaviorSubject<Post>  = new BehaviorSubject<Post>( {} as Post);
  likedByUser$: BehaviorSubject<boolean> = new BehaviorSubject(false);
  @Input() post: Post | undefined; // Change the type as per your data structure
  constructor(private httpService: HttpService, private authService: AuthService) {
    this.user = this.authService.user$;
    this.post$.subscribe((post: Post) => {
      // if user liked the post
      if ( post.likes !== undefined) {
        this.likedByUser$.next(post.likes.some(obj => obj.userId === this.user.id));
      }
    })

    // get logged in user from auth service
    this.authService.user$.subscribe((user: User) => {
      this.user = user;
    })
  }

  // style code
  private adjustFontSize(): void {
    const cardText = this.cardTextRef.nativeElement;
    const characters = cardText.textContent.trim().length;
    if (characters < this.minCharacters) {
      cardText.style.fontSize = `${this.minFontSize}rem`;
    } else if (characters > this.maxCharacters) {
      cardText.style.fontSize = `${this.maxFontSize}rem`;
    } else {
      const fontSize = this.calculateFontSize(characters);
      cardText.style.fontSize = `${fontSize}rem`;
    }
  }

  // style code
  private calculateFontSize(characters: number): number {
    const fontSizeRange = this.maxFontSize - this.minFontSize;
    const charactersRange = this.maxCharacters - this.minCharacters;
    const fontSizeIncrement = fontSizeRange / charactersRange;
    const fontSize = this.minFontSize + (fontSizeIncrement * (characters - this.minCharacters));
    return fontSize;
  }

  // style code
  ngAfterViewInit(): void {
    this.adjustFontSize();
  }

  ngOnInit(): void {
    console.log('user', this.post);
    if (this.post) {
      this.postId = this.post.id;
      this.post$.next(this.post);
    }
  }


  toggleLikePost() {
    console.log('like post', this.postId);
    console.log('liked by user', this.likedByUser$.value);
    if (this.likedByUser$.value) {
      // Unlikelike post
      this.httpService.post('/Home/' + this.postId + '/like', {}).subscribe(
        (response: any) => {
          this.likedByUser$.next(false);
          // remove like by user
          this.post$.value.likes.splice(this.post$.value.likes.findIndex(obj => obj.userId === this.user.id), 1);
        },
        (error: any) => {
          console.log(error);
        }
      );
    } else {
      // like post
      this.httpService.post('/Home/' + this.postId + '/like', {}).subscribe(
        (response: any) => {
          this.likedByUser$.next(true);
          var like: Like = {
            userId: this.user.id,
            postId: this.postId,
            dateTime: new Date(),
            user: this.user,
            post: this.post$.value
          }
          this.post$.value.likes.push(like);
        },
        (error: any) => {
          console.log(error);
        }
      );
    }
  }

}
