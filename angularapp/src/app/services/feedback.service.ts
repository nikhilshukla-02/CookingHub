import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Feedback } from '../models/feedback.model';
import { AuthService } from './auth.service';
 
@Injectable({ providedIn: 'root' })
export class FeedbackService {
  // private apiUrl = 'https://8080-aeecccebfeecdabeebedccecabfaedfdcf.premiumproject.examly.io';
  public apiUrl = 'https://8080-eafbccabcfdcbecdabeebedccecabfaedfdcf.premiumproject.examly.io';
 
  constructor(private http: HttpClient, private auth: AuthService) {}
 
  private getHeaders(): HttpHeaders {
    const token = this.auth.getToken();
    return new HttpHeaders().set('Authorization', `Bearer ${token}`);
  }
 
  sendFeedback(feedback: Feedback): Observable<Feedback> {
    return this.http.post<Feedback>(`${this.apiUrl}/api/feedback`, feedback, { headers: this.getHeaders() });
  }
 
  getAllFeedbacksByUserId(userId: number): Observable<Feedback[]> {
    return this.http.get<Feedback[]>(`${this.apiUrl}/api/feedback/user/${userId}`, { headers: this.getHeaders() });
  }
 
  deleteFeedback(feedbackId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/api/feedback/${feedbackId}`, { headers: this.getHeaders() });
  }
 
  getFeedbacks(): Observable<Feedback[]> {
    return this.http.get<Feedback[]>(`${this.apiUrl}/api/feedback`, { headers: this.getHeaders() });
  }
}