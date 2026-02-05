import { TestBed } from '@angular/core/testing';

import { GiftsService } from '../services/gifts';

describe('Gifts', () => {
  let service: GiftsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GiftsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
