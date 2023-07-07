import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth-service/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  apiKey: string;
  loading: boolean;

  constructor(private router: Router,
    private authService: AuthService) {
    this.apiKey = '';
    this.loading = false;
  }

  ngOnInit(): void {
    if (this.authService.isCurrentlyLoggedIn()) {
      this.router.navigate(['/']);
    }
  }

  login(): void {
    this.authService.login(this.apiKey);
  }
}