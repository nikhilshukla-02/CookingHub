import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CookingClassService } from '../../services/cooking-class.service';
import { CookingClass } from '../../models/cooking-class.model';
 
@Component({
  templateUrl: './admineditclass.component.html'
})
export class AdmineditclassComponent implements OnInit {
  classForm: FormGroup;
  classId: string;
  error = '';
 
  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private classService: CookingClassService
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
    this.classId = this.route.snapshot.paramMap.get('id')!;
  }
 
  ngOnInit() {
    this.classService.getCookingClassById(this.classId).subscribe(data => {
      this.classForm.patchValue(data);
    });
  }
 
  onSubmit() {
    if (this.classForm.invalid) return;
    this.classService.updateCookingClass(this.classId, this.classForm.value).subscribe({
      next: () => {
        alert('Cooking class updated successfully!');
        this.router.navigate(['/admin/view-class']);
      },
      error: (err) => {
        this.error = 'Update failed';
      }
    });
  }
 
  goBack() {
    this.router.navigate(['/admin/view-class']);
  }
}