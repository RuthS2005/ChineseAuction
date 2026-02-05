import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GiftDonation } from './gift-donation'
describe('GiftDonation', () => {
  let component: GiftDonation;
  let fixture: ComponentFixture<GiftDonation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GiftDonation]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GiftDonation );
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
