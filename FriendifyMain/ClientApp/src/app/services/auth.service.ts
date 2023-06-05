import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7073/api/Account';

  constructor(private http: HttpClient) {}
  login(username: string, password: string, rememberMe: boolean): Observable<any> {
    const loginData = {
      username: username,
      password: password,
      rememberMe: rememberMe
    };

    return this.http.post(`${this.baseUrl}/login`, loginData);
  }


  register(registerData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, registerData);
  }
}
