import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
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
export class DocUploadComponent implements OnInit, AfterViewInit {
  @ViewChild('canvas', { static: true }) canvas!: ElementRef<HTMLCanvasElement>;

  selectedFile: File | null = null;
  uploadProgress = 0;
  uploadMessage = '';
  files: string[] | null = null;
  public apiUrl: string = environment.apiUrl;
  base64String: string = ''; // Initialize the base64 string variable
  pdfSrc: string = '';
  context: CanvasRenderingContext2D | null = null; // Initialize as null
  isDrawing: boolean = false;
  lastX!: number;
  lastY!: number;

  constructor(private http: HttpClient) {}

  ngAfterViewInit() {
    // Ensure canvas is not null before accessing its context
    if (this.canvas && this.canvas.nativeElement) {
      this.context = this.canvas.nativeElement.getContext('2d');
    }
  }

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

      // const uploadReq = new HttpRequest(
      //   'POST',
      //   `${this.apiUrl}/DocUpload`, // Replace with your API endpoint URL
      //   formData,
      //   { reportProgress: true }
      // );

      const uploadReq = this.http.post<string[]>(
        `${this.apiUrl}/DocUpload`,
        formData,
        { reportProgress: true }
      );

      uploadReq.subscribe(
        (images: string[]) => {
          this.files = images;
          // Handle the response here
          console.log('Response:', images);
          // Do something with the array of strings
        },
        (error) => {
          console.error('Error:', error);
          // Handle error
        }
      );
      // .subscribe((event) => {
      //   this.getFileList(this.selectedFile?.name);
      //   if (event.type === HttpEventType.UploadProgress) {
      //   } else if (event instanceof HttpResponse) {

      //     this.selectedFile = null; // Clear selection
      //   }
      //   (images: string[]) => {
      //     this.files = images;
      //     //debugger;
      //     //this.loadImages();
      //   }
      // })
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
  }

  onMouseDown(event: MouseEvent) {
    this.isDrawing = true;
    const rect = this.canvas.nativeElement.getBoundingClientRect();
    this.lastX = event.clientX - rect.left;
    this.lastY = event.clientY - rect.top;
  }

  onMouseMove(event: MouseEvent) {
    if (this.isDrawing) {
      const rect = this.canvas.nativeElement.getBoundingClientRect();
      const x = event.clientX - rect.left;
      const y = event.clientY - rect.top;
      this.draw(this.lastX, this.lastY, x, y);
      this.lastX = x;
      this.lastY = y;
    }
  }

  onMouseUp(event: MouseEvent) {
    this.isDrawing = false;
  }

  draw(x1: number, y1: number, x2: number, y2: number) {
    if (this.context) {
      this.context.beginPath();
      this.context.strokeStyle = 'black';
      this.context.lineWidth = 2;
      this.context.moveTo(x1, y1);
      this.context.lineTo(x2, y2);
      this.context.stroke();
      this.context.closePath();
    }
  }
}
