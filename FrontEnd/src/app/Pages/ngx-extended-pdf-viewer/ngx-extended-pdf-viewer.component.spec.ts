import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NgxExtendedPdfViewerComponent } from './ngx-extended-pdf-viewer.component';

describe('NgxExtendedPdfViewerComponent', () => {
  let component: NgxExtendedPdfViewerComponent;
  let fixture: ComponentFixture<NgxExtendedPdfViewerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxExtendedPdfViewerComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NgxExtendedPdfViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
