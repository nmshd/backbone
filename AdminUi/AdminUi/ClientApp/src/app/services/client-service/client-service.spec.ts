import { TestBed } from '@angular/core/testing';

import { ClientServiceService } from './client-service';

describe('ClientServiceService', () => {
  let service: ClientServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClientServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
