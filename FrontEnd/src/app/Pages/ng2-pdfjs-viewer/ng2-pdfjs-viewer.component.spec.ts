import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Ng2PdfjsViewerComponent } from './ng2-pdfjs-viewer.component';

describe('Ng2PdfjsViewerComponent', () => {
  let component: Ng2PdfjsViewerComponent;
  let fixture: ComponentFixture<Ng2PdfjsViewerComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [Ng2PdfjsViewerComponent]
    });
    fixture = TestBed.createComponent(Ng2PdfjsViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
