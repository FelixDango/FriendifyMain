import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Post } from '../models/post';
import { HttpService } from './http.service'; 

@Injectable({
  providedIn: 'root'
})
export class PostmanageService {

  
  private apiUrl = '/home'; // The relative URL of the API controller

  constructor(
    private httpService: HttpService // Inject the HttpService
  ) { }

  // A method to get all posts from the API
  getAllPosts(): Observable<Post[]> {
    return this.httpService.get(this.apiUrl + '/getallposts'); // Use the HttpService get method
  }

  // A method to edit a post by its id and data
  editPost(id: number, post: Post): Observable<any> {
    return this.httpService.put(this.apiUrl + '/' + id + '/putedit', post); // Use the HttpService put method
  }

  // A method to delete a post by its id
  deletePost(id: number): Observable<any> {
    return this.httpService.delete(this.apiUrl + '/' + id + '/deletepost'); // Use the HttpService delete method
  }
}
