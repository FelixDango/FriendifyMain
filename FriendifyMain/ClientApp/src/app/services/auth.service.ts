import { Injectable } from '@angular/core';
import {HttpClient, HttpResponse} from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:7073/api/Account';

  constructor(private http: HttpClient) {}
  login(username: string, password: string, rememberMe: boolean): Observable<HttpResponse<any>> {
    const loginData = {
      username: username,
      password: password,
      rememberMe: rememberMe
    };

    return this.http.post<any>(`${this.baseUrl}/login`, loginData, { observe: 'response' });
  }


  register(registerData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/register`, registerData);
  }
}
