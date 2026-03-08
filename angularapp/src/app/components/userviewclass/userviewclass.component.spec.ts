import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserviewclassComponent } from './userviewclass.component';

describe('UserviewclassComponent', () => {
  let component: UserviewclassComponent;
  let fixture: ComponentFixture<UserviewclassComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserviewclassComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserviewclassComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
