import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  Renderer2,
  ViewChild,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import interact from 'interactjs';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-pdf-viewer-custom',
  standalone: true,
  imports: [PdfViewerModule, CommonModule, FormsModule],
  templateUrl: './pdf-viewer-custom.component.html',
  styleUrls: ['./pdf-viewer-custom.component.css'],
})
export class PdfViewerCustomComponent implements OnInit {
  @ViewChild('canvas', { static: true }) canvas!: ElementRef<HTMLCanvasElement>;
  canvasBase64: string | null = null;
  textSignText: string | null = null;
  ctx: CanvasRenderingContext2D | null = null;
  isDrawing = false;
  prevX = 0;
  prevY = 0;
  currentColor = 'black';

  activeTab: string = 'Canvas';

  textInput: string = '';
  strokeHistory: {
    startX: number;
    startY: number;
    endX: number;
    endY: number;
    color: string;
  }[] = [];
  selectedColor: string = 'black';
  fonts: string[] = [
    'Pacifico',
    'Kaushan Script',
    //Dancing Script',
    'Cursive',
    //'Shadows Into Light',
    //'Sacramento',
    //'Satisfy',
  ];

  pdfData = `data:application/pdf;base64,`;

  ngOnInit(): void {
    this.ctx = this.canvas.nativeElement.getContext('2d');
    // Bind event listeners

    this.redrawStrokes();

    this.pdfData = `data:application/pdf;base64,${localStorage.getItem(
      'pdfSrc'
    )}`;
  }

  isSignSelected = false;

  onSignSelect() {
    this.isSignSelected = true;
  }
  AddSignatueOnPageLoadOnClick(event: any) {
    if (this.isSignSelected) {
      this.AddSignatueOnPageLoad(event);
    }
  }
  divId = 1;
  handleCloseClick(event: Event): void {
    const clickedElement = event.target as HTMLElement;
    if (clickedElement.classList.contains('close-icon')) {
      const parentDivValue = clickedElement.getAttribute('parentDiv');
      if (parentDivValue) {
        const divToRemove = document.getElementById(parentDivValue);
        if (divToRemove) {
          // Remove the div from the DOM
          divToRemove.remove();
        }
      }
    }
  }

  AddSignatueOnPageLoad(event: any) {
    const viewerContainer = document.querySelector('.ng2-pdf-viewer-container');

    if (!viewerContainer) {
      return;
    }

    const pdfViewer = document.querySelector('.pdfViewer');

    const docRender = document.querySelector('.document-render');
    if (!docRender) {
      return;
    }
    if (!pdfViewer) {
      return;
    }

    const pages = pdfViewer.querySelectorAll('.page');
    if (event && this.isSignSelected) {
      var mouseEvent = event as MouseEvent;
      const mouseX = mouseEvent.clientX; // Mouse X coordinate
      const mouseY = mouseEvent.clientY;

      // const relativeX = mouseX + window.scrollX;
      //const relativeY = mouseY + window.scrollY;

      pages.forEach((element) => {
        const scale =
          viewerContainer.clientWidth / element.getBoundingClientRect().width;
        const addSignatureDiv = document.createElement('div');
        addSignatureDiv.id = `div${this.divId}`;
        addSignatureDiv.innerHTML = `Signature will be displayed here <span class="close-icon" parentDiv="div${this.divId}">&times;</span>`;
        this.divId++;
        addSignatureDiv.classList.add('digital-signature--remove');
        addSignatureDiv.style.width = '150pt';
        addSignatureDiv.style.height = '45pt';
        addSignatureDiv.style.border = '2px dashed #007BFF';
        addSignatureDiv.style.borderRadius = '10px';
        addSignatureDiv.style.display = 'flex';
        addSignatureDiv.style.justifyContent = 'center';
        addSignatureDiv.style.alignItems = 'center';
        addSignatureDiv.style.fontSize = '18px';
        addSignatureDiv.style.color = '#007BFF';
        addSignatureDiv.style.backgroundColor = '#fff';
        addSignatureDiv.style.boxShadow = '0 2px 10px rgba(0, 0, 0, 0.1)';
        addSignatureDiv.style.cursor = 'text';
        // addSignatureDiv.innerText = 'Signature will be displayed here';
        addSignatureDiv.style.textAlign = 'center';
        addSignatureDiv.style.position = 'absolute';

        addSignatureDiv.style.left = this.convertToPoint(
          mouseX - element.getBoundingClientRect().x
        );
        addSignatureDiv.style.top = this.convertToPoint(
          mouseY + viewerContainer.scrollTop - 40
        );

        addSignatureDiv.setAttribute('data-pt-x', addSignatureDiv.style.left);
        addSignatureDiv.setAttribute('data-pt-y', addSignatureDiv.style.top);

        element.appendChild(addSignatureDiv);

        interact(addSignatureDiv)
          .resizable({
            edges: { left: true, right: true, bottom: true, top: true },
            listeners: {
              move(event) {
                const target = event.target;
                let x = parseFloat(target.getAttribute('data-x')) || 0;
                let y = parseFloat(target.getAttribute('data-y')) || 0;

                // update the element's style
                target.style.width = (event.rect.width * 3) / 4 + 'pt';
                target.style.height = (event.rect.height * 3) / 4 + 'pt';

                // translate when resizing from top or left edges
                x += (event.deltaRect.left * 3) / 4;
                y += (event.deltaRect.top * 3) / 4;

                target.setAttribute('data-x', x);
                target.setAttribute('data-y', y);
              },
            },
            modifiers: [
              interact.modifiers.restrictEdges({
                outer: 'parent',
              }),
              interact.modifiers.restrictSize({
                min: { width: 100, height: 50 },
              }),
            ],
            inertia: true,
          })
          .draggable({
            inertia: true,
            autoScroll: true,
            modifiers: [
              interact.modifiers.restrictRect({
                restriction: 'parent',
              }),
            ],

            listeners: {
              move: (event) => this.onMove(event),
              //end: (event) => this.onEnd(event),
            },
          });

        this.isSignSelected = false;
      });
    } else {
      if (!event) {
        pages.forEach((element) => {
          const scale =
            viewerContainer.clientWidth / element.getBoundingClientRect().width;

          const addSignatureDiv = document.createElement('div');
          addSignatureDiv.id = `div${this.divId}`;
          addSignatureDiv.innerHTML = `Signature will be displayed here <span class="close-icon" parentDiv="div${this.divId}">&times;</span>`;
          this.divId++;
          addSignatureDiv.classList.add('digital-signature--remove');
          addSignatureDiv.style.width = '150pt';
          addSignatureDiv.style.height = '45pt';
          addSignatureDiv.style.border = '2px dashed #007BFF';
          addSignatureDiv.style.borderRadius = '10px';
          addSignatureDiv.style.display = 'flex';
          addSignatureDiv.style.justifyContent = 'center';
          addSignatureDiv.style.alignItems = 'center';
          addSignatureDiv.style.fontSize = '18px';
          addSignatureDiv.style.color = '#007BFF';
          addSignatureDiv.style.backgroundColor = '#fff';
          addSignatureDiv.style.boxShadow = '0 2px 10px rgba(0, 0, 0, 0.1)';
          addSignatureDiv.style.cursor = 'text';
          //addSignatureDiv.innerText = 'Signature will be displayed here';
          addSignatureDiv.style.position = 'absolute';
          addSignatureDiv.style.left = '27pt';
          addSignatureDiv.style.top = `720pt`;
          addSignatureDiv.style.textAlign = 'center';
          addSignatureDiv.style.resize = 'both';

          const addSignatureDivRight = document.createElement('div');
          addSignatureDivRight.id = `div${this.divId}`;
          addSignatureDivRight.innerHTML = `Signature will be displayed here <span class="close-icon" parentDiv="div${this.divId}">&times;</span>`;
          this.divId++;
          addSignatureDivRight.classList.add('digital-signature--remove');
          addSignatureDivRight.style.width = '150pt';
          addSignatureDivRight.style.height = '45pt';
          addSignatureDivRight.style.border = '2px dashed #007BFF';
          addSignatureDivRight.style.borderRadius = '10px';
          addSignatureDivRight.style.display = 'flex';
          addSignatureDivRight.style.justifyContent = 'center';
          addSignatureDivRight.style.alignItems = 'center';
          addSignatureDivRight.style.fontSize = '18px';
          addSignatureDivRight.style.color = '#007BFF';
          addSignatureDivRight.style.backgroundColor = '#fff';
          addSignatureDivRight.style.boxShadow =
            '0 2px 10px rgba(0, 0, 0, 0.1)';
          addSignatureDivRight.style.cursor = 'text';
          // addSignatureDivRight.innerText = 'Signature will be displayed here';
          addSignatureDivRight.style.position = 'absolute';
          addSignatureDivRight.style.left = '436.5pt';
          addSignatureDivRight.style.top = `720pt`;
          addSignatureDivRight.style.textAlign = 'center';

          addSignatureDiv.setAttribute('data-pt-x', addSignatureDiv.style.left);
          addSignatureDiv.setAttribute('data-pt-y', addSignatureDiv.style.top);

          addSignatureDivRight.setAttribute(
            'data-pt-x',
            addSignatureDivRight.style.left
          );
          addSignatureDivRight.setAttribute(
            'data-pt-y',
            addSignatureDivRight.style.top
          );

          element.appendChild(addSignatureDiv);
          element.appendChild(addSignatureDivRight);

          interact(addSignatureDiv)
            .resizable({
              edges: { left: true, right: true, bottom: true, top: true },
              listeners: {
                move(event) {
                  const target = event.target;
                  let x = parseFloat(target.getAttribute('data-x')) || 0;
                  let y = parseFloat(target.getAttribute('data-y')) || 0;

                  // update the element's style
                  target.style.width = (event.rect.width * 3) / 4 + 'pt';
                  target.style.height = (event.rect.height * 3) / 4 + 'pt';

                  // translate when resizing from top or left edges
                  x += (event.deltaRect.left * 3) / 4;
                  y += (event.deltaRect.top * 3) / 4;

                  target.setAttribute('data-x', x);
                  target.setAttribute('data-y', y);
                },
              },
              modifiers: [
                interact.modifiers.restrictEdges({
                  outer: 'parent',
                }),
                interact.modifiers.restrictSize({
                  min: { width: 100, height: 50 },
                }),
              ],
              inertia: true,
            })

            .draggable({
              inertia: true,
              modifiers: [
                interact.modifiers.restrictRect({
                  restriction: 'parent',
                }),
              ],

              listeners: {
                move: (event) => this.onMove(event),
                //end: (event) => this.onEnd(event),
              },
            });

          interact(addSignatureDivRight)
            .resizable({
              edges: { left: true, right: true, bottom: true, top: true },
              listeners: {
                move(event) {
                  const target = event.target;
                  let x = parseFloat(target.getAttribute('data-x')) || 0;
                  let y = parseFloat(target.getAttribute('data-y')) || 0;

                  // update the element's style
                  target.style.width = (event.rect.width * 3) / 4 + 'pt';
                  target.style.height = (event.rect.height * 3) / 4 + 'pt';

                  // translate when resizing from top or left edges
                  x += (event.deltaRect.left * 3) / 4;
                  y += (event.deltaRect.top * 3) / 4;

                  target.setAttribute('data-x', x);
                  target.setAttribute('data-y', y);
                },
              },
              modifiers: [
                interact.modifiers.restrictEdges({
                  outer: 'parent',
                }),
                interact.modifiers.restrictSize({
                  min: { width: 100, height: 50 },
                }),
              ],
              inertia: true,
            })
            .draggable({
              inertia: true,
              modifiers: [
                interact.modifiers.restrictRect({
                  restriction: 'parent',
                }),
              ],

              listeners: {
                move: (event) => this.onMove(event),
                //end: (event) => this.onEnd(event),
              },
            });
        });
      }
    }
  }
  ptToPx = (pt: any) => pt * 1.333;
  pxToPt = (px: any) => px / 1.333;

  onMove(event: any): void {
    const target = event.target;
    const initialX = this.ptToPx(
      parseFloat(target.getAttribute('data-pt-x')) || 0
    );
    const initialY = this.ptToPx(
      parseFloat(target.getAttribute('data-pt-y')) || 0
    );

    const x = initialX + event.dx;
    const y = initialY + event.dy;

    const xInPt = x / 1.3333;
    const yInPt = y / 1.3333;

    target.style.left = `${xInPt}pt`;
    target.style.top = `${yInPt}pt`;

    target.setAttribute('data-pt-x', xInPt);
    target.setAttribute('data-pt-y', yInPt);
  }

  onEnd(event: any) {}

  ObjCordinates = {
    Pages: [] as PageCoordinate[],
    base64ToFix: null,
    fileName: '',
  };

  SaveCordinates() {
    this.ObjCordinates.Pages = [];
    this.ObjCordinates.base64ToFix = null;
    const pdfViewer = document.querySelector('.pdfViewer');

    if (!pdfViewer) {
      return;
    }

    const pages = pdfViewer.querySelectorAll('.page');
    if (!pages) {
      return;
    }

    pages.forEach((element) => {
      const getSignBox = element.querySelectorAll('.digital-signature--remove');

      if (!getSignBox) {
        return;
      }
      var pageCordinates: any = [];

      getSignBox.forEach((sign: any) => {
        console.log(sign.getBoundingClientRect());
        pageCordinates.push({
          top: sign.style.top.replace('pt', ''),
          height: sign.style.height.replace('pt', ''),
          left: sign.style.left.replace('pt', ''),
          width: sign.style.width.replace('pt', ''),
        });
      });

      if (pageCordinates.length > 0) {
        this.ObjCordinates.Pages.push({
          pageNumber: element.getAttribute('data-page-number'),
          cordinate: pageCordinates,
          ImageBase64: null,
          SignText: null,
          SignTextFont: null,
        });

        if (this.activeTab == 'Canvas' && this.canvasBase64) {
          for (
            let index = 0;
            index < this.ObjCordinates.Pages.length;
            index++
          ) {
            this.ObjCordinates.Pages[index].ImageBase64 = this.canvasBase64;
          }
        }
        if (this.activeTab == 'Tab2' && this.textInput) {
          for (
            let index = 0;
            index < this.ObjCordinates.Pages.length;
            index++
          ) {
            this.ObjCordinates.Pages[index].SignText = this.textInput;
            this.ObjCordinates.Pages[index].SignTextFont = 'Kaushan Script';
          }
        }
      }
    });

    if (
      this.selectedFile &&
      this.ObjCordinates.Pages.length > 0 &&
      (this.canvasBase64 || this.textInput)
    ) {
      this.ObjCordinates.fileName = this.selectedFile.name;
      const uploadReq = this.http.post<string[]>(
        `${this.apiUrl}/DocUpload/pdfsigned`,
        this.ObjCordinates,
        { reportProgress: true }
      );

      uploadReq.subscribe(
        () => {
          // Handle the response here
          console.log('Response:');
          // Do something with the array of strings
        },
        (error) => {
          console.error('Error:', error);
          // Handle error
        }
      );
    } else {
      alert('Pdf file & signature is required to sign document');
    }
  }

  public apiUrl: string = environment.apiUrl;

  convertToPDFCoordinates(
    value: string | number | undefined,
    maxValue: number,
    defaultValue: number,
    imageMaxValue: number
  ): number {
    if (!value) {
      return defaultValue;
    }
    if (typeof value === 'string') {
      if (value.endsWith('%')) {
        return (parseInt(value, 10) / 100) * maxValue;
      } else if (value.endsWith('px')) {
        return parseInt(value, 10) * (maxValue / imageMaxValue);
      } else {
        return parseInt(value, 10);
      }
    } else {
      return value;
    }
  }

  constructor(
    private el: ElementRef,
    private renderer: Renderer2,
    private http: HttpClient
  ) {}

  convertToPoint(value: any) {
    return (value * 3) / 4 + 'pt';
  }

  selectedFile: File | null = null;

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  onView() {
    if (this.selectedFile) {
      const formData = new FormData();
      formData.append('file', this.selectedFile, this.selectedFile.name);

      const uploadReq = this.http.post<string[]>(
        `${this.apiUrl}/DocUpload`,
        formData,
        { reportProgress: true }
      );

      uploadReq.subscribe(
        () => {
          //this.router.navigate(['/pdf-viewer-custom']);
          // Handle the response here
          // Do something with the array of strings

          if (this.selectedFile) {
            this.convertToBase64(this.selectedFile);
          }
        },
        (error) => {
          console.error('Error:', error);
          // Handle error
        }
      );
    } else {
      //this.uploadMessage = 'Please select a file to upload.';
    }
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
    this.pdfData = URL.createObjectURL(file);
  }

  selectTab(tab: string) {
    this.activeTab = tab;
  }
  setColor(color: string) {
    if (this.activeTab === 'Canvas') {
      this.currentColor = color;
      this.redrawStrokes();
    }
  }

  redrawStrokes() {
    if (this.activeTab === 'Canvas' && this.ctx) {
      this.ctx.clearRect(
        0,
        0,
        this.canvas.nativeElement.width,
        this.canvas.nativeElement.height
      );
      for (const line of this.strokeHistory) {
        this.ctx.beginPath();
        this.ctx.moveTo(line.startX, line.startY);
        this.ctx.lineTo(line.endX, line.endY);
        this.ctx.strokeStyle = this.currentColor;
        this.ctx.lineWidth = 2;
        this.ctx.stroke();
      }
    }
  }

  clearCanvas() {
    if (this.activeTab === 'Canvas' && this.ctx) {
      this.ctx.clearRect(
        0,
        0,
        this.canvas.nativeElement.width,
        this.canvas.nativeElement.height
      );
      this.strokeHistory = [];
      this.currentColor = 'black';
    }
  }

  mousedown(event: MouseEvent) {
    if (this.activeTab === 'Canvas') {
      // this.isDrawing = true;
      // this.prevX = event.clientX - this.canvas.nativeElement.offsetLeft;
      // this.prevY = event.clientY - this.canvas.nativeElement.offsetTop;

      this.isDrawing = true;
      // Capture starting coordinates
      this.prevX = event.offsetX;
      this.prevY = event.offsetY;
    }
  }

  mouseup() {
    this.isDrawing = false;
    this.updateCanvasBase64();
  }

  mousemove(event: MouseEvent) {
    if (this.activeTab === 'Canvas' && this.isDrawing) {
      // const currentX = event.clientX - this.canvas.nativeElement.offsetLeft;
      // const currentY = event.clientY - this.canvas.nativeElement.offsetTop;
      // this.draw(this.prevX, this.prevY, currentX, currentY);
      // this.prevX = currentX;
      // this.prevY = currentY;

      const currentX = event.offsetX;
      const currentY = event.offsetY;
      // Draw line from previous coordinates to current coordinates
      this.draw(this.prevX, this.prevY, currentX, currentY);
      // Update previous coordinates
      this.prevX = currentX;
      this.prevY = currentY;
    }
  }

  draw(startX: number, startY: number, endX: number, endY: number) {
    if (this.ctx) {
      this.ctx.beginPath();
      this.ctx.moveTo(startX, startY);
      this.ctx.lineTo(endX, endY);
      this.ctx.strokeStyle = this.currentColor;
      this.ctx.lineWidth = 2;
      this.ctx.stroke();
      this.strokeHistory.push({
        startX,
        startY,
        endX,
        endY,
        color: this.currentColor,
      });
    }
  }

  setColor1(color: string) {
    this.selectedColor = color;
    this.updateCanvasBase64;
  }

  updateCanvasBase64() {
    ///debugger;
    if (!this.ctx) return;

    this.canvasBase64 = this.canvas.nativeElement.toDataURL();
  }

  getIpAddress() {
    // Replace 'http://example.com/api/ip' with your server-side endpoint URL
    return this.http.get<string>('http://example.com/api/ip');
  }

  private drawing = false;

  onTouchStart(event: TouchEvent) {
    event.preventDefault();
    const touch = event.touches[0];
    const rect = this.canvas.nativeElement.getBoundingClientRect();
    if(this.ctx){
    this.ctx.beginPath();
    this.ctx.moveTo(touch.clientX - rect.left, touch.clientY - rect.top);
    this.drawing = true;
    }
  }

  onTouchMove(event: TouchEvent) {
    event.preventDefault();
    if (!this.drawing) return;
    if(this.ctx){
      const touch = event.touches[0];
      const rect = this.canvas.nativeElement.getBoundingClientRect();
      this.ctx.lineTo(touch.clientX - rect.left, touch.clientY - rect.top);
      this.ctx.stroke();
    }
   
  }

  onTouchEnd(event: TouchEvent) {
    event.preventDefault();
    if (!this.drawing) return;
    if(this.ctx){
    this.drawing = false;
    this.ctx.closePath();
    }
  }
}

interface PageCoordinate {
  pageNumber: string | null;
  ImageBase64: string | null;
  SignText: string | null;
  SignTextFont: string | null;
  cordinate: any; // Replace 'any' with the actual type
}
