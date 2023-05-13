import {Inject, Injectable} from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {}
  login(username: string, password: string): Observable<any> {
    const loginData = {
      username: username,
      password: password
    };

    return this.http.post(this.baseUrl +'/login', loginData);
  }
}
