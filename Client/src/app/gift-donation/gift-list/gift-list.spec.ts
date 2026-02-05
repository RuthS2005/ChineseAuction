import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GiftsList } from './gift-list';

describe('GiftList', () => {
  let component: GiftsList;
  let fixture: ComponentFixture<GiftsList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GiftsList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GiftsList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
