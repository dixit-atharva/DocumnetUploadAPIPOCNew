import { Component, OnInit } from '@angular/core';
import {
  HttpClient,
  HttpEventType,
  HttpRequest,
  HttpResponse,
} from '@angular/common/http';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-doc-upload',
  templateUrl: './doc-upload.component.html',
  styleUrls: ['./doc-upload.component.css'],
})
export class DocUploadComponent implements OnInit {
  selectedFile: File | null = null;
  uploadProgress = 0;
  uploadMessage = '';
  files: string[] | null = null;
  public apiUrl: string = environment.apiUrl;

  constructor(private http: HttpClient) {}

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  getFileList(filename: string | undefined) {
    const getReq = this.http.get<string[]>(
      `${this.apiUrl}/DocUpload/files?filename=${filename}`,
      { reportProgress: true }
    );

    getReq.subscribe(
      (images: string[]) => {
        this.files = images;
        //debugger;
        //this.loadImages();
      },
      (error) => {
        console.error('Error:', error);
      }
    );
  }
  onUpload() {
    if (this.selectedFile) {
      const formData = new FormData();
      formData.append('file', this.selectedFile, this.selectedFile.name);

      const uploadReq = new HttpRequest(
        'POST',
        `${this.apiUrl}/DocUpload`, // Replace with your API endpoint URL
        formData,
        { reportProgress: true }
      );

      this.http.request(uploadReq).subscribe((event) => {
        this.getFileList(this.selectedFile?.name);
        if (event.type === HttpEventType.UploadProgress) {
        } else if (event instanceof HttpResponse) {
          
          this.selectedFile = null; // Clear selection
        }
      });
    } else {
      this.uploadMessage = 'Please select a file to upload.';
    }
  }

  imagesLoaded = 0;

  ngOnInit() {}

  // loadImages(): void {
  //   if (this.filePaths) {
  //     for (const imageUrl of this.filePaths) {
  //       const image = new Image();
  //       image.onload = () => {
  //         this.drawImage(image);
  //         this.imagesLoaded++;
  //         if (this.imagesLoaded === 4) {
  //           console.log('All images loaded');
  //         }
  //       };
  //       image.src = imageUrl;
  //     }
  //   }
  // }

  drawImage(image: HTMLImageElement): void {
    const canvas = document.getElementById('myCanvas') as HTMLCanvasElement;
    const context = canvas.getContext('2d');
    if (context) {
      const imageWidth = image.width;
      const imageHeight = image.height;
      const imageMargin = 10; // Adjust spacing between images
      const x = (imageWidth + imageMargin) * (this.imagesLoaded - 1);
      const y = 0; // Adjust y-coordinate for vertical positioning
      context.drawImage(image, x, y, imageWidth, imageHeight);
    }
  }
}
