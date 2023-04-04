import { TestBed } from '@angular/core/testing';

import { TierService } from './tier.service';

describe('TierService', () => {
  let service: TierService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TierService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
