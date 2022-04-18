import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-protected',
  templateUrl: './protected.component.html',
  styleUrls: ['./protected.component.css'],
})
export class ProtectedComponent implements OnInit {
  dataFromAzureProtectedApi$: Observable<any>;
  isAuthenticated = false;
  constructor(
    private oidcSecurityService: OidcSecurityService,
    private httpClient: HttpClient
  ) {
    this.dataFromAzureProtectedApi$ = of(null);
  }

  ngOnInit() {

    this.oidcSecurityService.isAuthenticated().subscribe(isAuthenticated => {
      this.isAuthenticated = isAuthenticated;
    });
  }

  callApi() {
    this.dataFromAzureProtectedApi$ = this.httpClient
      .get('https://localhost:44395/UserAccess')
      .pipe(catchError((error) => of(error)));
  }
}


