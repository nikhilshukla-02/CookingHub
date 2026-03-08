import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminviewappliedrequestComponent } from './adminviewappliedrequest.component';

describe('AdminviewappliedrequestComponent', () => {
  let component: AdminviewappliedrequestComponent;
  let fixture: ComponentFixture<AdminviewappliedrequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdminviewappliedrequestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdminviewappliedrequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
