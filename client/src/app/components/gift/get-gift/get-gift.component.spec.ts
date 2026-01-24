import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GetGiftComponent } from './get-gift.component';

describe('GetGiftComponent', () => {
  let component: GetGiftComponent;
  let fixture: ComponentFixture<GetGiftComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GetGiftComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GetGiftComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
