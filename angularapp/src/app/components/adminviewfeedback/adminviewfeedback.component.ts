import { Component, OnInit } from '@angular/core';
import { FeedbackService } from '../../services/feedback.service';
import { Feedback } from '../../models/feedback.model';

@Component({
  templateUrl: './adminviewfeedback.component.html'
})
export class AdminviewfeedbackComponent implements OnInit {
  feedbacks: Feedback[] = [];

  constructor(private fbService: FeedbackService) {}

  ngOnInit() {
    this.fbService.getFeedbacks().subscribe(data => {
      this.feedbacks = data;
    });
  }

}