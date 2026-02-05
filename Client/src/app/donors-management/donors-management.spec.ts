import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DonorsManagement } from './donors-management';

describe('DonorsManagement', () => {
  let component: DonorsManagement;
  let fixture: ComponentFixture<DonorsManagement>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DonorsManagement]
    })
      .compileComponents();

    fixture = TestBed.createComponent(DonorsManagement);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
