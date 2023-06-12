import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HttpService {
  private baseUrl = 'https://localhost:7073/api';
  constructor(private http: HttpClient) {}

  get(url: string): Observable<any> {
    const fullUrl = this.baseUrl + url;
    console.log(fullUrl);
    return this.http.get(fullUrl);
  }

  post(url: string, data: any): Observable<any> {
    const fullUrl = this.baseUrl + url;
    return this.http.post(fullUrl, data);
  }

  // Implement other HTTP methods as needed (put, delete, etc.)
  put(url: string, data: any): Observable<any> {
    const fullUrl = this.baseUrl + url;
    return this.http.put(fullUrl, data);
  }

  delete(url: string): Observable<any> {
    const fullUrl = this.baseUrl + url;
    return this.http.delete(fullUrl);
  }
}
