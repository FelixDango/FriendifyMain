import { Injectable } from '@angular/core';
import {HttpClient, HttpResponse} from '@angular/common/http';
import {BehaviorSubject, Observable} from 'rxjs';
import {Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7073/api/Account';
  private isLoggedInSubject: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public isLoggedIn$: Observable<boolean> = this.isLoggedInSubject.asObservable();


  constructor(private http: HttpClient, private router: Router) {}
  login(username: string, password: string, rememberMe: boolean): Observable<HttpResponse<any>> {
    const loginData = {
      username: username,
      password: password,
      rememberMe: rememberMe
    };

    return this.http.post<any>(`${this.baseUrl}/login`, loginData, { observe: 'response' });
  }

  logout(): void {
    // Perform any necessary cleanup or logout logic here
    // refresh page
    localStorage.removeItem('token'); // Remove token from local storage
    // Redirect to the login page
    this.router.navigate(['/login']);
  }


  register(registerData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, registerData);
  }

  isLoggedIn(): boolean {
    // Retrieve the current value of isLoggedInSubject
    const isLoggedIn = this.isLoggedInSubject.getValue();

    return isLoggedIn;
  }



}
