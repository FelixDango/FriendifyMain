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
    const headers = this.getHeadersWithAuthorization();
    return this.http.get(fullUrl, { headers });
  }

  post(url: string, data: any): Observable<any> {
    const fullUrl = this.baseUrl + url;
    const headers = this.getHeadersWithAuthorization();
    console.log(fullUrl);
    return this.http.post(fullUrl, data, { headers });
  }

  put(url: string, data: any): Observable<any> {
    const fullUrl = this.baseUrl + url;
    const headers = this.getHeadersWithAuthorization();
    return this.http.put(fullUrl, data, { headers });
  }

  delete(url: string): Observable<any> {
    const fullUrl = this.baseUrl + url;
    const headers = this.getHeadersWithAuthorization();
    return this.http.delete(fullUrl, { headers });
  }

  private getHeadersWithAuthorization(): HttpHeaders {
    const token = localStorage.getItem('token');
    if (token) {
      return new HttpHeaders().set('Authorization', `Bearer ${token}`);
    }
    return new HttpHeaders();
  }
}
