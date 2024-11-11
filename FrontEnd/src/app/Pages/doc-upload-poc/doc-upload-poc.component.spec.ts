import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DocUploadPOCComponent } from './doc-upload-poc.component';

describe('DocUploadPOCComponent', () => {
  let component: DocUploadPOCComponent;
  let fixture: ComponentFixture<DocUploadPOCComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [DocUploadPOCComponent]
    });
    fixture = TestBed.createComponent(DocUploadPOCComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
