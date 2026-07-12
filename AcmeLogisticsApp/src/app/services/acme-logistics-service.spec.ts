import { TestBed } from '@angular/core/testing';

import { AcmeLogisticsService } from './acme-logistics-service';

describe('AcmeLogisticsService', () => {
  let service: AcmeLogisticsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AcmeLogisticsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
