import { Component, OnInit } from '@angular/core';
import { CookingClassService } from '../../services/cooking-class.service';
import { CookingClass } from '../../models/cooking-class.model';
import { Router } from '@angular/router';
 
@Component({
  templateUrl: './userviewclass.component.html'
})
export class UserviewclassComponent implements OnInit {
  classes: CookingClass[] = [];
  filteredClasses: CookingClass[] = [];
  searchText = '';
 
  constructor(private classService: CookingClassService, private router: Router) {}
 
  ngOnInit() {
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
 
  apply(classId: number) {
    this.router.navigate([`/user/apply/${classId}`]);
  }
}
