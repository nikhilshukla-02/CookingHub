import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  error = '';

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.loginForm.invalid) return;

    this.auth.login(this.loginForm.value).subscribe({
      next: () => {
        // Role is decoded & stored inside AuthService.login() via setCurrentUser
        const role = this.auth.getRole(); // returns 'admin' | 'user' | undefined

        if (role === 'admin') {
          this.router.navigate(['/admin/view-class']);
        } else if (role === 'user') {
          this.router.navigate(['/user/view-classes']);
        } else {
          // Fallback if token had no role
          this.router.navigate(['/user/view-classes']);
        }
      },
      error: () => {
        this.error = 'Invalid email or password';
      }
    });
  }
}