import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router, UrlTree } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  constructor(private auth: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    const user = this.auth.currentUserValue;

    // Not logged in → redirect to login
    if (!user || !user.token) {
      return this.router.parseUrl('/login');
    }

    // Check required role if specified on the route
    const requiredRole = route.data?.['role'] as string | undefined;
    if (requiredRole && !this.auth.hasRole(requiredRole)) {
      return this.router.parseUrl('/error');
    }

    return true;
  }
}