import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PdfViewerCustomComponent } from './pdf-viewer-custom.component';

describe('PdfViewerCustomComponent', () => {
  let component: PdfViewerCustomComponent;
  let fixture: ComponentFixture<PdfViewerCustomComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PdfViewerCustomComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(PdfViewerCustomComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
