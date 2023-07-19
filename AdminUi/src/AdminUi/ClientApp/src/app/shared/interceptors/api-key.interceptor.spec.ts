import { TestBed } from '@angular/core/testing';

import { ApiKeyInterceptor } from './api-key.interceptor';

describe('ApiKeyInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      ApiKeyInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: ApiKeyInterceptor = TestBed.inject(ApiKeyInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
