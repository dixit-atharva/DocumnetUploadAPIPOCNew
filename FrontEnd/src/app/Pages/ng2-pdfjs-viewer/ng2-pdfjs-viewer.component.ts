import { Component, ViewChild } from '@angular/core';

@Component({
  selector: 'app-ng2-pdfjs-viewer',
  templateUrl: './ng2-pdfjs-viewer.component.html',
  styleUrls: ['./ng2-pdfjs-viewer.component.css']
})
export class Ng2PdfjsViewerComponent {
  selectedFile: File | null = null;
  base64String: string = ''; // Initialize the base64 string variable
  pdfSrc: string = '';
  @ViewChild('pdfViewerOnDemand') pdfViewerOnDemand : any;
  
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
    this.pdfViewerOnDemand.pdfSrc = this.pdfSrc;
    this.pdfViewerOnDemand.refresh(); 
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }
}
