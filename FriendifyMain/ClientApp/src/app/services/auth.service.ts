import { Injectable } from '@angular/core';
import {HttpClient, HttpResponse} from '@angular/common/http';
import {BehaviorSubject, Observable} from 'rxjs';
import {Router} from "@angular/router";
import {User} from "../models/user";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7073/api/Account';
  isLoggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public isLoggedIn$: Observable<boolean> = this.isLoggedInSubject.asObservable();
  public user$: Observable<User> = new Observable<User>();


  constructor(private http: HttpClient, private router: Router) {}
  login(username: string, password: string, rememberMe: boolean): Observable<HttpResponse<any>> {
    const loginData = {
      username: username,
      password: password,
      rememberMe: rememberMe
    };
    return this.http.post(`${this.baseUrl}/login`, loginData, { observe: 'response' });
  }

  logout(): void {
    // Perform any necessary cleanup or logout logic here
    localStorage.removeItem('token'); // Remove token from local storage
    // remove user from local storage to log user out
    localStorage.removeItem('user');
    // Redirect to the login page
    this.router.navigate(['/login']);
    // Emit the authentication status
    this.isLoggedInSubject.next(false);
  }


  register(registerData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, registerData);
  }

  isLoggedIn(): boolean {
    // Retrieve the token from local storage
    const token = localStorage.getItem('token');

    // Check if the token is present
    const isLoggedIn = !!token;

    return isLoggedIn;
  }



}
