import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
 
@Injectable({ providedIn: 'root' })
export class AuthService {
  //private apiUrl = 'https://8080-aeecccebfeecdabeebedccecabfaedfdcf.premiumproject.examly.io';
  public apiUrl = 'https://8080-eafbccabcfdcbecdabeebedccecabfaedfdcf.premiumproject.examly.io';

 
  private currentUserSubject: BehaviorSubject<any>;
  public currentUser: Observable<any>;
 
  constructor(private http: HttpClient) {
    const stored = localStorage.getItem('currentUser');
    const parsed = stored ? JSON.parse(stored) : null;
 
    // 🛠️ If old data exists without role, try to repair from token
    const repaired = this.repairUserIfMissingRole(parsed);
 
    // Only persist if we actually have something (avoid writing "null" string)
    if (repaired) {
      localStorage.setItem('currentUser', JSON.stringify(repaired));
    } else {
      localStorage.removeItem('currentUser');
    }
 
    this.currentUserSubject = new BehaviorSubject<any>(repaired);
    this.currentUser = this.currentUserSubject.asObservable();
  }
 
  public get currentUserValue() {
    return this.currentUserSubject.value;
  }
 
  /** ---------------------------
   *   JWT Helpers
   *  --------------------------- */
 
  /** Safely decode JWT payload */
  private decodeJwt(token: string): any | null {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const json = atob(base64);
      return JSON.parse(json);
    } catch (e) {
      console.warn('[AuthService] JWT decode failed:', e);
      return null;
    }
  }

  private extractFromToken(token: string): { role?: string; email?: string; name?: string; sub?: string; userId?: string } {
    const p = this.decodeJwt(token) || {};
    const role =
      (p.role ??
        p.roles ??
        p['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
        p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'] ??
        '')?.toString();

    const email =
      (p.email ??
        p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ??
        '')?.toString();

    const name =
      (p.name ??
        p['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ??
        '')?.toString();

    const sub = (p.sub ?? p.jti ?? '')?.toString();

    const userId = (p.UserId ?? '')?.toString();   // ✅ ADD THIS

    return {
      role: role?.trim(),
      email: email?.trim(),
      name: name?.trim(),
      sub,
      userId: userId?.trim(),   // ✅ ADD THIS
    };
  }
 
  private repairUserIfMissingRole(user: any) {
    if (!user) return null;
    if (user.role && typeof user.role === 'string' && user.role.length > 0) return user;
    if (!user.token) return user;

    const claims = this.extractFromToken(user.token);
    const normalizedRole = claims.role ? claims.role.toLowerCase() : undefined;

    const repaired = {
      id: claims.userId ?? user.id ?? claims.sub ?? null,  // ✅ CHANGED
      email: user.email ?? claims.email ?? null,
      username: user.username ?? claims.name ?? null,
      role: normalizedRole,
      token: user.token,
    };

    console.log('[AuthService] Repaired user from token claims:', repaired);
    return repaired;
  }
 
  /** Persist + emit current user */
  private setCurrentUser(user: any) {
    if (user) {
      localStorage.setItem('currentUser', JSON.stringify(user));
    } else {
      localStorage.removeItem('currentUser');
    }
    this.currentUserSubject.next(user);
  }
 
  /** ---------------------------
   *   Public Auth API
   *  --------------------------- */
 
  /** REGISTER — if API returns a token, auto-login by decoding and storing role */
  register(user: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/api/register`, user).pipe(
      map((response) => {
        // Case A: API returns a token like login → decode and persist user
        if (response && response.token) {
          const token = response.token;
 
          const claims = this.extractFromToken(token);
          const role = claims.role ? claims.role.toLowerCase() : undefined;
 
          const newUser = {
            id: response.userId ?? claims.sub ?? null,
            email: response.email ?? user.Email ?? user.email ?? claims.email ?? null,
            username: response.username ?? claims.name ?? null,
            role, // 'admin' or 'user' or undefined
            token,
          };
 
          console.log('[AuthService] Storing user after register:', newUser);
          this.setCurrentUser(newUser);
        } else {
          // Case B: API does NOT return a token → user stays logged out
          console.warn('[AuthService] Register response had no token. User will not be auto-logged-in.');
        }
 
        return response;
      })
    );
  }
 
  /** LOGIN — decode token; persist role/email/name */
  login(login: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/api/login`, login).pipe(
      map((response) => {
        if (response && response.token) {
          const token = response.token;

          const claims = this.extractFromToken(token);
          const role = claims.role ? claims.role.toLowerCase() : undefined;

          const user = {
            id: claims.userId ?? response.userId ?? claims.sub ?? null,  // ✅ CHANGED: prioritize userId
            email: login.Email ?? login.email ?? claims.email ?? null,
            username: response.username ?? claims.name ?? null,
            role,
            token,
          };

          console.log('[AuthService] Storing user with role:', user);
          this.setCurrentUser(user);
        } else {
          console.warn('[AuthService] Login response missing token:', response);
        }
        return response;
      })
    );
  }
  logout() {
    this.setCurrentUser(null);
  }
 
  getToken(): string | null {
    const u = this.currentUserValue;
    return u ? u.token : null;
  }
 
  /** ---------------------------
   *   Role helpers (for guards/templates)
   *  --------------------------- */
 
  // ✅ Returns true if a user is logged in (has a non-empty token)
  public isLoggedIn(): boolean {
    const u = this.currentUserValue;
    return !!(u && typeof u.token === 'string' && u.token.length > 0);
  }
 
  // ✅ Returns user's normalized role ('admin'/'user') or undefined
  public getRole(): string | undefined {
    const u = this.currentUserValue;
    const role = u?.role;
    return typeof role === 'string' ? role.toLowerCase() : undefined;
  }
 
  // ✅ Checks for a specific role (accepts 'Admin'|'User' or lowercase)
  public hasRole(requiredRole: 'Admin' | 'User' | string): boolean {
    const current = this.getRole(); // 'admin' | 'user' | undefined
    const expected = (requiredRole || '').toString().toLowerCase();
    return !!current && current === expected;
  }
}