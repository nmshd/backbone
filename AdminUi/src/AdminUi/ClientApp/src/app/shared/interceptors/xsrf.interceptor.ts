import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { from, lastValueFrom, Observable } from 'rxjs';
import { AuthService } from 'src/app/services/auth-service/auth.service';
import 'cookie-store';

@Injectable()
export class XSRFInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    // convert promise to observable using 'from' operator
    return from(this.handle(req, next))
  }

  async handle(req: HttpRequest<any>, next: HttpHandler) {
    const token = localStorage.getItem("xsrf-token");
    const skipIntercept = req.headers.has('skip');
    if (skipIntercept) {
      req = req.clone({
        headers: req.headers.delete('skip')
      });
    } else if(token){
      req = req.clone({
        withCredentials: true,
        setHeaders: {
          'X-XSRF-TOKEN': token,
        },
      });
    }
    return await lastValueFrom(next.handle(req));
  }
}
