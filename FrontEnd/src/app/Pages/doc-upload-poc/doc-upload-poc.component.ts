import { Component, ViewChild, ElementRef, OnInit, AfterViewInit } from '@angular/core';

@Component({
  selector: 'app-doc-upload-poc',
  templateUrl: './doc-upload-poc.component.html',
  styleUrls: ['./doc-upload-poc.component.css']
})
export class DocUploadPOCComponent implements AfterViewInit   {
  @ViewChild('canvas') canvas!: ElementRef<HTMLCanvasElement>;
  ctx!: CanvasRenderingContext2D;
  label1: string = '';
  label2: string = '';

  images: string[] = ['../../../assets/Images/1.jpg', '../../../assets/Images/2.jpg'];
  ngAfterViewInit() {
    this.ctx = this.canvas.nativeElement.getContext('2d') ?? ({} as CanvasRenderingContext2D);
     if (!this.ctx) {
      console.error("Canvas context is not available.");
    }
  }

  selectedStaticImage: string = ''; 
  selectedStaticImageHeight: number = 0; 
  selectedStaticImageWidth: number = 0; 
  
  pasteImage(event: MouseEvent) {
    debugger;
     const target = event.target as HTMLImageElement;
   
    this.selectedStaticImage = target.src;
    this.selectedStaticImageHeight = target.offsetHeight;
    this.selectedStaticImageWidth = target.offsetWidth;
  }

  handleCanvasClick(event: MouseEvent) {
    const x = event.offsetX;
    const y = event.offsetY;
    console.log(`Clicked at (${x}, ${y}) on canvas.`);
    
    debugger;
    if (this.selectedStaticImage && this.ctx) {
      debugger;
      const img = new Image();
      img.src = this.selectedStaticImage;
      img.onload = () => {
        console.log("Image loaded.");
         const aspectRatio = img.naturalWidth / img.naturalHeight;
      const newWidth = 100; // Specify the desired width of the image
      const newHeight = newWidth / aspectRatio; // 
          this.ctx.drawImage(img, x, y, img.naturalWidth, img.naturalHeight);
        console.log("Image drawn on canvas.");
      }
    } else {
      console.error("No static image selected or canvas context is not available.");
    }
    // Here you can add logic to handle clicking on canvas if needed
  }

  saveImages() {
    const dataUrl = this.canvas.nativeElement.toDataURL('image/png');
    const link = document.createElement('a');
    link.download = 'canvas-image.png';
    link.href = dataUrl;
    link.click();
  }
}
