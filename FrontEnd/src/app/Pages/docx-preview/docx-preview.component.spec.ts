import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocxPreviewComponent } from './docx-preview.component';

describe('DocxPreviewComponent', () => {
  let component: DocxPreviewComponent;
  let fixture: ComponentFixture<DocxPreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DocxPreviewComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(DocxPreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
