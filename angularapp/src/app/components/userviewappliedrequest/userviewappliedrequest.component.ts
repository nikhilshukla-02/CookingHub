import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CookingClassService } from '../../services/cooking-class.service';
import { AuthService } from '../../services/auth.service';
import { forkJoin } from 'rxjs';
import { ColDef, GridReadyEvent, GridApi } from 'ag-grid-community';

@Component({
  templateUrl: './userviewappliedrequest.component.html'
})
export class UserviewappliedrequestComponent implements OnInit {
  requests: any[] = [];
  searchText = '';
  userId: number = 0;

  // AG Grid
  private gridApi!: GridApi;
  rowData: any[] = [];

  columnDefs: ColDef[] = [
    { field: 'className', headerName: 'Class Name', sortable: true, filter: true, flex: 1 },
    { field: 'RequestDate', headerName: 'Request Date', sortable: true, filter: true, flex: 1,
      valueFormatter: (params) => params.value ? new Date(params.value).toLocaleDateString() : '' },
    { field: 'Status', headerName: 'Status', sortable: true, filter: true, flex: 1 },
    { field: 'DietaryPreferences', headerName: 'Dietary Preferences', sortable: true, filter: true, flex: 1 },
    { field: 'CookingGoals', headerName: 'Cooking Goals', sortable: true, filter: true, flex: 1 },
    { field: 'Comments', headerName: 'Comments', sortable: true, filter: true, flex: 1 },
    {
      headerName: 'Action',
      flex: 1,
      cellRenderer: (params: any) => {
        const isDisabled = this.isDisabled(params.data.Status);
        const editBtn = `<button class="btn btn-sm ${isDisabled ? 'btn-secondary' : 'btn-warning'} me-1"
          ${isDisabled ? 'disabled' : ''} data-action="edit">Edit</button>`;
        const deleteBtn = `<button class="btn btn-sm ${isDisabled ? 'btn-secondary' : 'btn-danger'}"
          ${isDisabled ? 'disabled' : ''} data-action="delete">Delete</button>`;
        return `<div class="d-flex gap-2">${editBtn}${deleteBtn}</div>`;
      }
    }
  ];

  defaultColDef: ColDef = {
    resizable: true
  };

  pagination = true;
  paginationPageSize = 5;

  // Grid options with cell click handler
  gridOptions = {
    onCellClicked: (params: any) => {
      if (params.colDef.headerName !== 'Action') return;
      const target = params.event.target as HTMLElement;
      const action = target.getAttribute('data-action')
        || target.closest('button')?.getAttribute('data-action');
      if (!action) return;
      if (action === 'edit') this.openEditModal(params.data);
      if (action === 'delete') this.deleteRequest(params.data.CookingClassRequestId);
    }
  };

  // Modal state
  showEditModal = false;
  selectedRequestId: number = 0;
  editForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private classService: CookingClassService,
    private auth: AuthService
  ) {
    this.editForm = this.fb.group({
      RequestDate: ['', Validators.required],
      DietaryPreferences: ['', Validators.required],
      CookingGoals: ['', Validators.required],
      Comments: ['']
    });
  }

  ngOnInit() {
    const currentUser = this.auth.currentUserValue;
    if (currentUser) {
      this.userId = currentUser.id;
      this.loadRequests();
    }
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
  }

  loadRequests() {
    forkJoin({
      requests: this.classService.getCookingClassRequestsByUserId(this.userId.toString()),
      classes: this.classService.getAllCookingClasses()
    }).subscribe(({ requests, classes }) => {
      this.requests = requests.map(req => {
        const cookingClass = classes.find(c => c.CookingClassId === req.CookingClassId);
        return { ...req, className: cookingClass?.ClassName || 'Unknown' };
      });
      this.rowData = [...this.requests];
    });
  }

  onSearchInput() {
    this.gridApi.setQuickFilter(this.searchText);
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

  openEditModal(request: any) {
    const role = this.auth.currentUserValue?.role;
    if (role?.toLowerCase() !== 'user') {
      alert('Only users can edit a cooking class request.');
      return;
    }

    if (this.isDisabled(request.Status)) {
      alert('Cannot edit a request that is already Approved or Rejected.');
      return;
    }

    this.selectedRequestId = request.CookingClassRequestId;
    this.editForm.patchValue({
      RequestDate: request.RequestDate?.substring(0, 10),
      DietaryPreferences: request.DietaryPreferences,
      CookingGoals: request.CookingGoals,
      Comments: request.Comments || ''
    });
    this.showEditModal = true;
  }

  closeEditModal() {
    this.showEditModal = false;
    this.editForm.reset();
  }

  submitEdit() {
    if (this.editForm.invalid) return;

    const currentRequest = this.requests.find(r => r.CookingClassRequestId === this.selectedRequestId);

    const payload = {
      RequestDate: this.editForm.get('RequestDate')?.value,
      DietaryPreferences: this.editForm.get('DietaryPreferences')?.value,
      CookingGoals: this.editForm.get('CookingGoals')?.value,
      Comments: this.editForm.get('Comments')?.value || '',
      Status: currentRequest?.Status
    };

    this.classService.patchCookingClassRequest(this.selectedRequestId.toString(), payload).subscribe({
      next: () => {
        alert('Request updated successfully!');
        this.closeEditModal();
        this.loadRequests();
      },
      error: (err) => {
        console.log('Status:', err.status);
        console.log('Error body:', err.error);
        alert(err?.error?.message || 'Failed to update request.');
      }
    });
  }
}
