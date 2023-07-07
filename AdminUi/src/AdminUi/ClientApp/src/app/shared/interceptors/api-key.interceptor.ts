import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from 'src/app/services/auth-service/auth.service';

@Injectable()
export class ApiKeyInterceptor implements HttpInterceptor {

  isLoggedIn$: Observable<boolean> | undefined;

  constructor(private authService: AuthService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    this.isLoggedIn$ = this.authService.isLoggedIn;
    if (this.isLoggedIn$) {
      request = request.clone({
        setHeaders: {
          'X-API-KEY': this.authService.getApiKey()!,
        },
      });
    }
    return next.handle(request);
  }
}
