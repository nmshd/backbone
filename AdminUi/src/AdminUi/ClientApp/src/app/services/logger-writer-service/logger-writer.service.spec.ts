import { TestBed } from '@angular/core/testing';

import { LoggerWriterService } from './logger-writer.service';

describe('LoggerWriterService', () => {
  let service: LoggerWriterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LoggerWriterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
