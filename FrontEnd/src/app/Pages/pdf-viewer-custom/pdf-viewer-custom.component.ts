import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AfterViewInit, Component, ElementRef, OnInit, Renderer2 } from '@angular/core';
import interact from 'interactjs';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-pdf-viewer-custom',
  standalone: true,
  imports: [PdfViewerModule, CommonModule],
  templateUrl: './pdf-viewer-custom.component.html',
  styleUrl: './pdf-viewer-custom.component.css',
})
export class PdfViewerCustomComponent implements OnInit {
  pdfData =
    `data:application/pdf;base64,`;

    ngOnInit(): void {
      this.pdfData = `data:application/pdf;base64,${localStorage.getItem('pdfSrc')}`
    }

  isSignSelected = false;

  onSignSelect() {
    this.isSignSelected = true;
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
        addSignatureDiv.innerText = 'Dixit Gajjar';
        addSignatureDiv.style.position = 'absolute';

        addSignatureDiv.style.left = this.convertToPoint(
          mouseX - element.getBoundingClientRect().x
        );
        addSignatureDiv.style.top = this.convertToPoint(
          mouseY + viewerContainer.scrollTop - 40
        );

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

                target.style.webkitTransform = target.style.transform =
                  'translate(' + x + 'pt,' + y + 'pt)';

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

        this.isSignSelected = false;
      });
    } else {
      if (!event) {
        pages.forEach((element) => {
          const scale =
            viewerContainer.clientWidth / element.getBoundingClientRect().width;

          const addSignatureDiv = document.createElement('div');
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
          addSignatureDiv.innerText = 'Dixit Gajjar';
          addSignatureDiv.style.position = 'absolute';
          addSignatureDiv.style.left = '27pt';
          addSignatureDiv.style.top = `735pt`;

          addSignatureDiv.style.resize = 'both';

          const addSignatureDivRight = document.createElement('div');
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
          addSignatureDivRight.innerText = 'Dixit Gajjar';
          addSignatureDivRight.style.position = 'absolute';
          addSignatureDivRight.style.left = '436.5pt';
          addSignatureDivRight.style.top = `734.25pt`;

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

                  target.style.webkitTransform = target.style.transform =
                    'translate(' + x + 'pt,' + y + 'pt)';

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

                  target.style.webkitTransform = target.style.transform =
                    'translate(' + x + 'pt,' + y + 'pt)';

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
  onMove(event: any): void {
    // const target = event.target;

    const target = event.target;
    const x = this.convertToPoint(
      (parseFloat(target.getAttribute('data-x')) || 0) + event.dx
    );
    const y = this.convertToPoint(
      (parseFloat(target.getAttribute('data-y')) || 0) + event.dy
    );

    target.style.transform = `translate(${x}, ${y})`;
    target.setAttribute('data-x', x);
    target.setAttribute('data-y', y);
  }

  ObjCordinates = {
    Pages: [] as PageCoordinate[],
    base64ToFix: null,
    fileName : ''
  };

  SaveCordinates() {
    this.ObjCordinates.Pages = [];
    this.ObjCordinates.base64ToFix = null;
    const pdfViewer = document.querySelector('.pdfViewer');
    const viewerContainer = document.querySelector('.ng2-pdf-viewer-container');
    const documentcontainer = document.querySelector('.document-container');
    const documentrender = document.querySelector('.document-render');
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
      debugger;
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
        });
      }
    });

    if(this.selectedFile){
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

          if(this.selectedFile){
            this.convertToBase64(this.selectedFile);
          }

        },
        (error) => {
          console.error('Error:', error);
          // Handle error
        }
      )
      
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

}

interface PageCoordinate {
  pageNumber: string | null;
  cordinate: any; // Replace 'any' with the actual type
}


