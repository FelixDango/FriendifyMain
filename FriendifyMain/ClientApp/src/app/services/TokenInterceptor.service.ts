import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { AuthService } from "./auth.service";


  // Define an HTTP interceptor service
  @Injectable()
export class TokenInterceptor implements HttpInterceptor
  {

    constructor(private authService: AuthService) {}

intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
  // Get the token from the auth service
  const token = this.authService.getToken();

  // Clone and modify the request to add the Authorization header
  request = request.clone({
    setHeaders:
    {
      Authorization: `Bearer ${ token}`
      }
  });

  // Pass the modified request to the next handler
  return next.handle(request);
}
}

