import { Injectable } from '@angular/core';
import { Observable } from 'rxjs'; // A library that provides observables
import { User } from '../models/user';
import { HttpService } from './http.service';


@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpService) { } // Inject the HttpService


  // A method that returns an observable of all users
  getUsers(): Observable<User[]> {
    return this.http.get('/profile/all'); // Use the get method of the HttpService
  }

  // A method that returns an observable of a single user by their username
  getUser(username: string): Observable<User> {
    return this.http.get(`/profile/${username}/view`); // Use the get method of the HttpService
  }

  // A method that returns an observable of the updated user after updating their /profile
  updateUser(username: string, model: any): Observable<User> {
    return this.http.put(`/profile/${username}/update`, model); // Use the put method of the HttpService
  }

  // A method that returns an observable of a message after deleting a user by their username
  deleteUser(username: string): Observable<any> {
    return this.http.delete(`/profile/${username}`); // Use the delete method of the HttpService
  }

  // A method that returns an observable of a message after suspending a user by their username
  suspendUser(username: string): Observable<any> {
    return this.http.post(`/admin/suspend/${username}`, null); // Use the post method of the HttpService
  }

  // A method that returns an observable of a message after unsuspending a user by their username
  unsuspendUser(username: string): Observable<any> {
    return this.http.post(`/admin/unsuspend/${username}`, null); // Use the post method of the HttpService
  }

  // A method that returns an observable of a message after assigning a role to a user by their username
  assignRole(username: string, role: string): Observable<any> {
    return this.http.post(`/role/assignrole?username=${username}&roleName=${role}`, null); // Use the post method of the HttpService
  }

}
