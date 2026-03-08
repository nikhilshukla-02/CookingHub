import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserviewappliedrequestComponent } from './userviewappliedrequest.component';

describe('UserviewappliedrequestComponent', () => {
  let component: UserviewappliedrequestComponent;
  let fixture: ComponentFixture<UserviewappliedrequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserviewappliedrequestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserviewappliedrequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
