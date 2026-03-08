import { Component, OnInit } from '@angular/core';
import { FeedbackService } from '../../services/feedback.service';
import { AuthService } from '../../services/auth.service';
import { Feedback } from '../../models/feedback.model';
 
@Component({
  templateUrl: './userviewfeedback.component.html'
})
export class UserviewfeedbackComponent implements OnInit {
  feedbacks: Feedback[] = [];
  userId: number = 0;
 
  constructor(
    private fbService: FeedbackService,
    private auth: AuthService
  ) {}
 
  ngOnInit() {
    const currentUser = this.auth.currentUserValue;
    if (currentUser) {
      this.userId = currentUser.id;
      this.loadFeedbacks();
    }
  }
 
  loadFeedbacks() {
    this.fbService.getAllFeedbacksByUserId(this.userId).subscribe(data => {
      this.feedbacks = data;
    });
  }
 
  deleteFeedback(id: number) {
    if (confirm('Are you sure you want to delete this feedback?')) {
      this.fbService.deleteFeedback(id).subscribe(() => {
        this.loadFeedbacks();
      });
    }
  }
}