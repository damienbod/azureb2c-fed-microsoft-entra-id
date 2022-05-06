import { HttpInterceptor, HttpRequest, HttpHandler } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private secureRoutes = ['https://localhost:44395'];

  constructor( private oidcSecurityService: OidcSecurityService,) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ) {
    if (!this.secureRoutes.find((x) => request.url.startsWith(x))) {
      return next.handle(request);
    }

    let token = '';
    this.oidcSecurityService.getAccessToken().subscribe((result) => {
      token = result;
    });

    if (!token) {
      return next.handle(request);
    }

    request = request.clone({
      headers: request.headers.set('Authorization', 'Bearer ' + token),
    });

    return next.handle(request);
  }
}
