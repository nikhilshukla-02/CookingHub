import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CookingClass } from '../models/cooking-class.model';
import { CookingClassRequest } from '../models/cooking-class-request.model';
import { AuthService } from './auth.service';
 
@Injectable({ providedIn: 'root' })
export class CookingClassService {
  //private apiUrl = 'https://8080-aeecccebfeecdabeebedccecabfaedfdcf.premiumproject.examly.io';
  public apiUrl = 'https://8080-eafbccabcfdcbecdabeebedccecabfaedfdcf.premiumproject.examly.io';


  
  
  constructor(private http: HttpClient, private auth: AuthService) {}
 
  private getHeaders(): HttpHeaders {
    const token = this.auth.getToken();
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }
 
  getAllCookingClasses(): Observable<CookingClass[]> {
    return this.http.get<CookingClass[]>(`${this.apiUrl}/api/cookingClass`, { headers: this.getHeaders() });
  }
 
  getCookingClassById(classId: string): Observable<CookingClass> {
    return this.http.get<CookingClass>(`${this.apiUrl}/api/cookingClass/${classId}`, { headers: this.getHeaders() });
  }
 
  addCookingClass(cooking: CookingClass): Observable<CookingClass> {
    return this.http.post<CookingClass>(`${this.apiUrl}/api/cookingClass`, cooking, { headers: this.getHeaders() });
  }
w

  updateCookingClass(classId: string, cooking: CookingClass): Observable<CookingClass> {
    return this.http.put<CookingClass>(`${this.apiUrl}/api/cookingClass/${classId}`, cooking, { headers: this.getHeaders() });
  }
 
  deleteCookingClass(classId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/cookingClass/${classId}`, { headers: this.getHeaders() });
  }
 
  getAllCookingClassRequests(): Observable<CookingClassRequest[]> {
    return this.http.get<CookingClassRequest[]>(`${this.apiUrl}/api/cookingClassRequest`, { headers: this.getHeaders() });
  }
 
  getCookingClassRequestsByUserId(userId: string): Observable<CookingClassRequest[]> {
    return this.http.get<CookingClassRequest[]>(`${this.apiUrl}/api/cookingClassRequest/user/${userId}`, { headers: this.getHeaders() });
  }
 
  addCookingClassRequest(request: CookingClassRequest): Observable<string> {
    return this.http.post(`${this.apiUrl}/api/cookingClassRequest`, request, {
      headers: this.getHeaders(),
      responseType: 'text'          // ✅ Tells Angular to expect plain text, not JSON
    });
  }
  updateCookingClassRequest(requestId: string, request: CookingClassRequest): Observable<CookingClassRequest> {
    return this.http.put<CookingClassRequest>(`${this.apiUrl}/api/cookingClassRequest/${requestId}`, request, { headers: this.getHeaders() });
  }
 
  deleteCookingClassRequest(requestId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/cookingClassRequest/${requestId}`, { headers: this.getHeaders() });
  }

  sendChatMessage(message: string): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/api/chat`,
      { message },
      { headers: this.getHeaders() }
    );
  }
}
