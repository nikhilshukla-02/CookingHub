import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Subscription } from 'rxjs';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'angularapp';

  constructor(public auth: AuthService) {}

  ngOnInit(): void {
    // Optional: see emissions in console
    this.auth.currentUser.subscribe(u =>
      console.log('[AppComponent] currentUser emission:', u)
    );
  }

  // ✅ Expose localStorage value for templates
  get currentUserFromStorage(): string | null {
    try {
      return localStorage.getItem('currentUser');
    } catch {
      return null;
    }
  }
}
