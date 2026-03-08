import { TestBed } from '@angular/core/testing';

import { CookingClassService } from './cooking-class.service';

describe('CookingClassService', () => {
  let service: CookingClassService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CookingClassService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
