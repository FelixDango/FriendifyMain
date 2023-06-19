import { Injectable } from '@angular/core';
import {HttpClient, HttpResponse} from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, ReplaySubject, tap, throwError } from 'rxjs';
import {Router} from "@angular/router";
import { User } from "../models/user";

// Define an interface for the response object
interface LoginResponse extends HttpResponse<any> {
  token: string;
  user: User;
}

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  getToken() {
    return localStorage.getItem('token');
  }

  getUserName(): string{
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    if (user) return user.userName;
    return '';
  }

  updateUser() {
    const user = JSON.parse(localStorage.getItem('user') || '{}');
    this.user$.next(user);
  }


  private baseUrl = 'https://localhost:7073/api/Account';
  isLoggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public isLoggedIn$: Observable<boolean> = this.isLoggedInSubject.asObservable();
  public user$: ReplaySubject<User> = new ReplaySubject<User>(1); // Replay the last value to new subscribers

  constructor(private http: HttpClient, private router: Router) { }

  login(username: string, password: string, rememberMe: boolean): Observable<LoginResponse> {
    const loginData = {
      username: username,
      password: password,
      rememberMe: rememberMe
    };
    // Use the interface as the generic type parameter
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, loginData, { responseType: 'json' }).pipe(
      tap(response=>{ // Add a colon and specify the type
        // Save token and user data to local storage
        console.log('response', response);
        localStorage.setItem('token', response.token);
        localStorage.setItem('user', JSON.stringify(response.user));
        // Emit the authentication status
        this.isLoggedInSubject.next(true);
        // Emit the user data
        this.user$.next(response.user);
        // Navigate to the home page
        this.router.navigate(['/']);
      }),
      catchError(error => {
        // Display an error message or log the error
        console.error(error);
        alert(error.message);
        // Return an observable with a user-facing error message
        return throwError('Something bad happened; please try again later.');
      })
    );
  }

  logout(): void {
    // Perform any necessary cleanup or logout logic here
    localStorage.removeItem('token'); // Remove token from local storage
    // Remove user data from local storage
    localStorage.removeItem('user');
    // Emit the authentication status
    this.isLoggedInSubject.next(false);
    // Emit null as the user data
    this.user$.next(null as any); // No error here
    // Redirect to the login page
    this.router.navigate(['/login']);
  }

  register(registerData: any): Observable<any> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/register`, registerData, { responseType: 'json' }).pipe(
      tap(response => {
        // Save token to local storage
        localStorage.setItem('token', response.token);
        // Emit the authentication status
        this.isLoggedInSubject.next(true);
        // Emit the user data
        this.user$.next(response.user);
        // Navigate to the home page
        this.router.navigate(['/']);
      }),
      catchError(error => {
        // Display an error message or log the error
        console.error(error);
        alert(error.message);
        // Return an observable with a user-facing error message
        return throwError('Something bad happened; please try again later.');
      })
    );
  }

  // get user id from local storage
  getUserId(): number {
    // Retrieve the token from local storage
    const user = localStorage.getItem('user');
    if (user) return JSON.parse(user).id;
    return 0;
  }

  isLoggedIn(): boolean {
    // Retrieve the token from local storage
    const token = localStorage.getItem('token');

    // Check if the token is present
    const isLoggedIn = !!token;

    this.user$ = new ReplaySubject<User>(1);
    // If the user is logged in, emit the user data
    if (isLoggedIn) {
      this.user$.next(JSON.parse(localStorage.getItem('user') || '{}'));
    } else {
      // If the user is not logged in, emit null
      this.user$.next(null as any);
    }

    return isLoggedIn;
  }
}
