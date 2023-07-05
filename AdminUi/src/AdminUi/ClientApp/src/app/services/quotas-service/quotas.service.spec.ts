import { TestBed } from '@angular/core/testing';

import { QuotasService } from './quotas.service';

describe('QuotasService', () => {
  let service: QuotasService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(QuotasService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
