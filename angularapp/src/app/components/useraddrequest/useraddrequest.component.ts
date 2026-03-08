import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CookingClassService } from '../../services/cooking-class.service';
import { AuthService } from '../../services/auth.service';
import { CookingClassRequest } from '../../models/cooking-class-request.model';

@Component({
  templateUrl: './useraddrequest.component.html'
})
export class UseraddrequestComponent implements OnInit {
  requestForm: FormGroup;
  classId: number;
  userId: number = 0;
  appliedClassIds: number[] = [];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private classService: CookingClassService,
    private auth: AuthService
  ) {
    this.classId = +this.route.snapshot.paramMap.get('classId')!;
    this.requestForm = this.fb.group({
      DietaryPreferences: ['', Validators.required],
      CookingGoals: ['', Validators.required],
      Comments: ['']
    });
  }

  ngOnInit(): void {
    const currentUser = this.auth.currentUserValue;
    if (currentUser) {
      this.userId = currentUser.id;
    }
  }

  onSubmit() {
    if (this.requestForm.invalid || this.userId === 0) return;

    const payload = {
      userId: parseInt(this.userId.toString(), 10),
      cookingClassId: parseInt(this.classId.toString(), 10),
      requestDate: new Date().toISOString(),
      status: 'Pending',
      dietaryPreferences: this.requestForm.get('DietaryPreferences')?.value,
      cookingGoals: this.requestForm.get('CookingGoals')?.value,
      comments: this.requestForm.get('Comments')?.value || ""
    };

    this.classService.addCookingClassRequest(payload as any).subscribe({
      next: () => {
        alert('Successfully Submitted!');
        this.router.navigate(['/user/my-requests']);
      },
      error: () => {
        alert('Failed to submit request.');
      }
    });
  }
}