import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { FeedbackService } from '../../services/feedback.service';
import { AuthService } from '../../services/auth.service';

@Component({
  templateUrl: './useraddfeedback.component.html'
})
export class UseraddfeedbackComponent implements OnInit {
  feedbackForm: FormGroup;
  userId: number = 0;

  constructor(
    private fb: FormBuilder,
    private fbService: FeedbackService,
    private auth: AuthService,
    private router: Router
  ) {
    this.feedbackForm = this.fb.group({
      FeedbackText: ['', Validators.required]
    });
  }

  ngOnInit() {
    const currentUser = this.auth.currentUserValue;
    if (currentUser) {
      this.userId = currentUser.id;
    }
  }

  onSubmit() {
    if (this.feedbackForm.invalid || this.userId === 0) return;
    // ✅ Changed to camelCase to match ASP.NET Core's default JSON serialization
    const feedbackData = {
      userId: this.userId,
      feedbackText: this.feedbackForm.value.FeedbackText,
      date: new Date().toISOString()
    };
    this.fbService.sendFeedback(feedbackData as any).subscribe({
      next: () => {
        alert('Feedback submitted successfully!');
        this.router.navigate(['/user/my-feedbacks']);
      },
      error: (err) => {
        console.error('Feedback error:', err);
        alert('Failed to submit feedback');
      }
    });
  }
}