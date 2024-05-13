import { Component, OnInit } from '@angular/core';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';

@Component({
  selector: 'app-ngx-extended-pdf-viewer',
  templateUrl: './ngx-extended-pdf-viewer.component.html',
  styleUrl: './ngx-extended-pdf-viewer.component.css'
})
export class NgxExtendedPdfViewerComponent implements OnInit {
  ngOnInit(): void {
    // var innerSRC = localStorage.getItem('pdfSrc');
    // if(innerSRC){

    //   this.pdfSrc =innerSRC ;
    // }
  }

  selectedFile: File | null = null;
  base64String: string = ''; // Initialize the base64 string variable
  pdfSrc: string = '';

  

  onView() {
    this.convertToBase64(this.selectedFile);
  }

  // Convert the selected file to base64
  convertToBase64(file: File | null) {
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        const arrayBuffer = e.target.result;
        this.byteArrayToString(arrayBuffer);
      };
      reader.readAsArrayBuffer(file);
    }
  }

  byteArrayToString(arrayBuffer: ArrayBuffer) {
    const bytes = new Uint8Array(arrayBuffer);
    const file = new Blob([bytes], { type: 'application/pdf' });
    this.pdfSrc = URL.createObjectURL(file);
    localStorage.setItem('pdfSrc',this.pdfSrc)
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }
}
