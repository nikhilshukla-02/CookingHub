import { Component, OnInit } from '@angular/core';
import { CookingClassService } from '../../services/cooking-class.service';
import { CookingClass } from '../../models/cooking-class.model';
import { Router } from '@angular/router';

@Component({
  templateUrl: './adminviewclass.component.html'
})
export class AdminviewclassComponent implements OnInit {
  classes: CookingClass[] = [];
  filteredClasses: CookingClass[] = [];
  searchText = '';

  constructor(private classService: CookingClassService, private router: Router) {}

  ngOnInit() {
    this.loadClasses();
  }

  loadClasses() {
    this.classService.getAllCookingClasses().subscribe(data => {
      this.classes = data;
      this.filteredClasses = data;
    });
  }

  filter() {
    this.filteredClasses = this.classes.filter(c =>
      c.ClassName.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }

  editClass(id: number) {
    this.router.navigate(['/admin/edit-class', id]);
  }

  deleteClass(id: number) {
    if (confirm('Are you sure you want to delete this class?')) {
      this.classService.deleteCookingClass(id.toString()).subscribe(() => {
        this.loadClasses();
      });
    }
  }
}
