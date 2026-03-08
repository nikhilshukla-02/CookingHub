import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
 
@Component({
  selector: 'app-adminnav',
  templateUrl: './adminnav.component.html'
})
export class AdminnavComponent {
  user: any;
 
  constructor(private auth: AuthService, private router: Router) {
    this.auth.currentUser.subscribe(u => this.user = u);
  }
 
  logout() {
    if (confirm('Are you sure you want to logout?')) {
      this.auth.logout();
      this.router.navigate(['/home']);
    }
  }
}