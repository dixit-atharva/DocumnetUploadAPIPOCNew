import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MainPOCComponent } from './main-poc.component';

describe('MainPOCComponent', () => {
  let component: MainPOCComponent;
  let fixture: ComponentFixture<MainPOCComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MainPOCComponent]
    });
    fixture = TestBed.createComponent(MainPOCComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
