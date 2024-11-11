import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NgxExtendedPdfViewerComponentTest } from './ngx-extended-pdf-viewer.component';

describe('NgxExtendedPdfViewerComponentTest', () => {
  let component: NgxExtendedPdfViewerComponentTest;
  let fixture: ComponentFixture<NgxExtendedPdfViewerComponentTest>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NgxExtendedPdfViewerComponentTest]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(NgxExtendedPdfViewerComponentTest);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
