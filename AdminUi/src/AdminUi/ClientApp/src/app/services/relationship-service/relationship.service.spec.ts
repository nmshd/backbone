import { TestBed } from '@angular/core/testing';

import { RelationshipService } from './relationship.service';

describe('RelationshipService', () => {
  let service: RelationshipService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RelationshipService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
