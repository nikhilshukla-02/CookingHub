import { Component, OnInit } from '@angular/core';
import { CookingClassService } from '../../services/cooking-class.service';
import { AuthService } from '../../services/auth.service';
import { forkJoin } from 'rxjs';
 
@Component({
  templateUrl: './userviewappliedrequest.component.html'
})
export class UserviewappliedrequestComponent implements OnInit {
  requests: any[] = [];
  filteredRequests: any[] = [];
  searchText = '';
  userId: number = 0;
 
  constructor(
    private classService: CookingClassService,
    private auth: AuthService
  ) {}
 
  ngOnInit() {
    const currentUser = this.auth.currentUserValue;
    if (currentUser) {
      this.userId = currentUser.id;
      this.loadRequests();
    }
  }
 
  loadRequests() {
    forkJoin({
      requests: this.classService.getCookingClassRequestsByUserId(this.userId.toString()),
      classes: this.classService.getAllCookingClasses()
    }).subscribe(({ requests, classes }) => {
      this.requests = requests.map(req => {
        const cookingClass = classes.find(c => c.CookingClassId === req.CookingClassId);  // ✅ camelCase
        return { ...req, className: cookingClass?.ClassName || 'Unknown' };               // ✅ camelCase
      });
      this.filteredRequests = [...this.requests];
    });
  }
 
  filter() {
    this.filteredRequests = this.requests.filter(r =>
      r.ClassName.toLowerCase().includes(this.searchText.toLowerCase())  // ✅ camelCase
    );
  }
  
  isDisabled(status: string): boolean {
    return status === 'Approved' || status === 'Rejected';
  }
 
  deleteRequest(requestId: number) {
    if (confirm('Are you sure you want to delete this request?')) {
      this.classService.deleteCookingClassRequest(requestId.toString()).subscribe(() => {
        this.loadRequests();
      });
    }
  }
}