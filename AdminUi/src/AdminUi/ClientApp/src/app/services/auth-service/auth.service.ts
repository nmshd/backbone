import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private loggedIn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(this.hasApiKey());

  get isLoggedIn() {
    return this.loggedIn.asObservable();
  }

  constructor(private router: Router) { }

  isCurrentlyLoggedIn(): boolean {
    return this.loggedIn.value;
  }

  hasApiKey(): boolean {
    return !!localStorage.getItem('api-key');
  }

  getApiKey(): string | null {
    return localStorage.getItem('api-key');
  }

  login(apiKey: string): void {
    if (apiKey !== '') {
      localStorage.setItem('api-key', apiKey);
      this.loggedIn.next(true);
      this.router.navigate(['/']);
    }
  }

  logout(): void {
    localStorage.removeItem('api-key');
    this.loggedIn.next(false);
    this.router.navigate(['/login']);
  }
}
