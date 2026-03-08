import { Component, OnInit } from '@angular/core';
import { CookingClassService } from '../../services/cooking-class.service';
import { CookingClassRequest } from '../../models/cooking-class-request.model';
import { CookingClass } from '../../models/cooking-class.model';
import { forkJoin } from 'rxjs';

@Component({
  templateUrl: './adminviewappliedrequest.component.html'
})
export class AdminviewappliedrequestComponent implements OnInit {
  requests: any[] = []; // enriched with className
  filteredRequests: any[] = [];
  searchText = '';

  constructor(private classService: CookingClassService) {}

  ngOnInit() {
    this.loadRequests();
  }

  loadRequests() {
    forkJoin({
      requests: this.classService.getAllCookingClassRequests(),
      classes: this.classService.getAllCookingClasses()
    }).subscribe(({ requests, classes }) => {
      this.requests = requests.map(req => {
        const cookingClass = classes.find(c => c.CookingClassId === req.CookingClassId);
        return { ...req, ClassName: cookingClass?.ClassName || 'Unknown' };
      });
      this.filteredRequests = [...this.requests];
    });
  }

  filter() {
    this.filteredRequests = this.requests.filter(r =>
      r.ClassName.toLowerCase().includes(this.searchText.toLowerCase())
    );
  }

  updateStatus(requestId: number, status: string) {
    const request = this.requests.find(r => r.CookingClassRequestId === requestId);
    if (request) {
      request.Status = status;
      this.classService.updateCookingClassRequest(requestId.toString(), request).subscribe(() => {
        this.loadRequests(); // refresh
      });
    }
  }
}