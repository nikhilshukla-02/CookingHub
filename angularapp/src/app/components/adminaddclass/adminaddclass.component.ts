import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { CookingClassService } from '../../services/cooking-class.service';
 
@Component({
  templateUrl: './adminaddclass.component.html'
})
export class AdminaddclassComponent {
  classForm: FormGroup;
  error = '';
 
  constructor(
    private fb: FormBuilder,
    private classService: CookingClassService,
    private router: Router
  ) {
    this.classForm = this.fb.group({
      ClassName: ['', Validators.required],
      CuisineType: ['', Validators.required],
      ChefName: ['', Validators.required],
      Location: ['', Validators.required],
      DurationInHours: ['', [Validators.required, Validators.min(1)]],
      Fee: ['', [Validators.required, Validators.min(0)]],
      IngredientsProvided: ['', Validators.required],
      SkillLevel: ['', Validators.required],
      SpecialRequirements: ['', Validators.required]
    });
  }
 
  onSubmit() {
    if (this.classForm.invalid) {
      alert('All fields are required');
      return;
    }
    this.classService.addCookingClass(this.classForm.value).subscribe({
      next: () => {
        alert('Successfully Added!');
        this.router.navigate(['/admin/view-class']);
      },
      error: (err) => {
        if (err.status === 409) {
          this.error = 'Cooking class with the same name already exists';
        } else {
          this.error = 'Failed to add class';
        }
      }
    });
  }
}