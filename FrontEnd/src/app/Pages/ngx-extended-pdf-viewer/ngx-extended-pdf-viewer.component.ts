import { Component, DebugElement, OnInit, ViewChild } from '@angular/core';
import {
  EditorAnnotation,
  FreeTextEditorAnnotation,
  NgxExtendedPdfViewerComponent,
  NgxExtendedPdfViewerService,
  pdfDefaultOptions,
} from 'ngx-extended-pdf-viewer';

@Component({
  selector: 'app-ngx-extended-pdf-viewer',
  templateUrl: './ngx-extended-pdf-viewer.component.html',
  styleUrl: './ngx-extended-pdf-viewer.component.css',
})
export class NgxExtendedPdfViewerComponentTest implements OnInit {
  sidebarVisible = false;
  @ViewChild(NgxExtendedPdfViewerComponent, { static: false })
  pdfViewer!: NgxExtendedPdfViewerComponent;
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
  constructor(private pdfService: NgxExtendedPdfViewerService) {}
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
    localStorage.setItem('pdfSrc', this.pdfSrc);
  }

  onFileSelected(event: any) {
    this.selectedFile = event.target.files[0];
  }

  ////
  sign = [
    {
      Name: 'Dixit Gajjar',
      Mobile: 9638865899,
      SignType: 'aadhar',
      Id: '1',
    },
    {
      Name: 'Rahul Patel',
      Mobile: 9638865891,
      SignType: 'aadhar',
      Id: '2',
    },
  ];
  selectedSign = {};

  images: string[] = [
    '../../../assets/Images/1.jpg',
    '../../../assets/Images/2.jpg',
  ];
  showTool1: boolean = false;
  showTool2: boolean = false;
  showTool3: boolean = false;
  showTool4: boolean = false;
  showTool5: boolean = false;
  selectedColor: string = '#000000';
  showToolSection(toolNumber: number) {
    this.showTool1 = false;
    this.showTool2 = false;
    this.showTool3 = false;
    this.showTool4 = false;
    this.showTool5 = false;
    if (toolNumber === 1) {
      this.showTool1 = true;
    } else if (toolNumber === 2) {
      this.showTool2 = true;
    } else if (toolNumber === 3) {
      this.showTool3 = true;
    } else if (toolNumber === 4) {
      this.showTool4 = true;
    } else if (toolNumber === 5) {
      this.showTool5 = true;
    }
  }
  selectedStaticImage: string = '';
  selectedText: string = '';
  pasteImage(event: MouseEvent) {
    const target = event.target as HTMLImageElement;

    this.selectedStaticImage = target.src;
  }

  pasteText(event: MouseEvent) {
    const target = event.target as HTMLSpanElement;

    this.selectedText = target.innerText;
  }

  pasteSelectedSign(m: {}) {
    this.selectedSign = m;
  }

  async addImagesToPDF(event: MouseEvent) {
    const pdfViewer = document.querySelector('.pdfViewer');
    if (pdfViewer) {
      if (this.selectedStaticImage && this.showTool1) {
        this.pdfService.switchAnnotationEdtorMode(13);

        await this.pdfService.addImageToAnnotationLayer({
          urlOrDataUrl: this.selectedStaticImage,
          page: this.pdfService.currentPageIndex(),
          left: event.clientY,
          top: event.clientX - window.scrollX,
        });

        this.selectedStaticImage = '';
      }
      if (this.showTool2 && this.selectedText) {
        this.pdfService.switchAnnotationEdtorMode(3);
        // Create a FreeTextEditorAnnotation object
        const annotationData: FreeTextEditorAnnotation = {
          value: this.selectedText,
          annotationType: 3, // Example value, replace with appropriate annotation type
          color: [0, 0, 255, 1], // Example color value, replace with appropriate color
          fontSize: 12, // Example font size value, replace with appropriate font size
          pageIndex: 0, // Example page index value, replace with appropriate page index
          // Add other required properties here
          rect: [1, event.clientX, event.clientY, 10], // Example rectangle coordinates, replace with appropriate values
          rotation: 0,
        };
        await this.pdfService.addEditorAnnotation(annotationData);

        this.selectedText = '';
      }

      //  // Mouse Y coordinate
      // const relativeX = mouseX + window.scrollX;
      // const relativeY = mouseY + window.scrollY;
      // const viewerDiv = document.querySelector('.pdfViewer');
      // let pageNumber: any = document.getElementById('pageNumber');
      // const classNameToEnabled = 'annotationEditorLayer';
      // const classNameToDisabled = 'annotationLayer';
      // // Get all elements with the data-page-number attribute within the viewer div
      // if (viewerDiv && pageNumber && this.selectedStaticImage) {
      //   const elementsWithDataPageNumber = viewerDiv.querySelectorAll(
      //     `[data-page-number="${pageNumber.value}"]`
      //   );
      //   elementsWithDataPageNumber.forEach((element) => {
      //     // const elemeentToDisabled = element.querySelectorAll(
      //     //   `.${classNameToDisabled}`
      //     // );
      //     const elementeToEnabed = element.querySelectorAll(
      //       `.${classNameToEnabled}`
      //     );
      //     // elemeentToDisabled.forEach((element) => {
      //     //   element.classList.add('disabled');
      //     // });
      //     elementeToEnabed.forEach((element) => {
      //       element.classList.add('stampEditing');
      //       element.removeAttribute('hidden');
      //       element.classList.remove('disabled');
      //       debugger;
      //       const parentDiv = document.createElement('div');
      //       parentDiv.setAttribute('data-editor-rotation', '0');
      //       parentDiv.classList.add('stampEditor');
      //       parentDiv.classList.add('draggable');
      //       parentDiv.classList.add('selectedEditor');
      //       parentDiv.id = 'pdfjs_internal_editor_0';
      //       parentDiv.tabIndex = 0;
      //       parentDiv.style.zIndex = '1';
      //       const resizer = [
      //         'topLeft',
      //         'topMiddle',
      //         'topRight',
      //         'middleRight',
      //         'bottomRight',
      //         'bottomMiddle',
      //         'bottomLeft',
      //         'middleLeft',
      //       ];
      //       const resizerDiv = document.createElement('div');
      //       resizerDiv.classList.add('resizers');
      //       parentDiv.appendChild(resizerDiv);
      //       for (let index = 0; index < resizer.length; index++) {
      //         const resizerDivInner = document.createElement('div');
      //         resizerDivInner.classList.add('resizer');
      //         resizerDivInner.classList.add(resizer[index]);
      //         resizerDivInner.setAttribute('data-resizer-name', resizer[index]);
      //         resizerDivInner.tabIndex = -1;
      //         resizerDiv.appendChild(resizerDivInner);
      //       }
      //       const editorToolBarDiv = document.createElement('div');
      //       editorToolBarDiv.classList.add('editToolbar');
      //       editorToolBarDiv.setAttribute('role', 'toolbar');
      //       parentDiv.appendChild(editorToolBarDiv);
      //       const buttonDiv = document.createElement('div');
      //       buttonDiv.classList.add('buttons');
      //       const removebutton = document.createElement('button');
      //       removebutton.classList.add('delete');
      //       removebutton.tabIndex = 0;
      //       removebutton.title = 'Remove image';
      //       buttonDiv.appendChild(removebutton);
      //       editorToolBarDiv.appendChild(buttonDiv);
      //       // Inside the ddImagesToPDF method
      //       const canvas = document.createElement('canvas');
      //       canvas.width = 200; // Set your desired width
      //       canvas.height = 200; // Set your desired height
      //       const context = canvas.getContext('2d');
      //       if (context) {
      //         // Load image from URL
      //         const image = new Image();
      //         image.onload = () => {
      //           // Once the image is loaded, draw it onto the canvas
      //           context.drawImage(image, 0, 0, canvas.width, canvas.height);
      //         };
      //         image.src = this.selectedStaticImage;
      //       }
      //       parentDiv.appendChild(canvas);
      //       // const imageContainer = document.createElement('div');
      //       // imageContainer.classList.add('image-container');
      //       // const newImage = document.createElement('img');
      //       // newImage.src = this.selectedStaticImage;
      //       // newImage.style.position = 'absolute';
      //       // newImage.style.left = `${relativeX}px`;
      //       // newImage.style.top = `${relativeY}px`;
      //       // // Create a remove button
      //       // const removeButton = document.createElement('button');
      //       // removeButton.innerText = '✖';
      //       // removeButton.style.position = 'absolute';
      //       // removeButton.classList.add('remove-button');
      //       // removeButton.style.top = `${relativeY}px`;
      //       // removeButton.style.left = `${relativeX + newImage.width - 35}px`;
      //       // removeButton.onclick = () => {
      //       //   element.removeChild(imageContainer);
      //       // };
      //       // // Append image and remove button to the container
      //       // imageContainer.appendChild(newImage);
      //       // imageContainer.appendChild(removeButton);
      //       element.appendChild(parentDiv);
      //       this.selectedStaticImage = '';
      //     });
      //   });
    }
  }

  makeResizable(element: HTMLElement) {
    let isResizing = false;

    element.style.resize = 'both';
    element.style.overflow = 'auto';

    element.addEventListener('click', () => {
      const elements = document.querySelectorAll('.set-color');

      // Loop through each element and remove the class
      elements.forEach((element) => {
        element.classList.remove('set-color');
      });
      element.classList.add('set-color');
    });
  }
  applyColor(event: any) {
    const color = event.target.value;

    const resizableDivs = document.querySelectorAll('.set-color');
    resizableDivs.forEach((div) => {
      const resizableElement = div as HTMLElement;
      resizableElement.style.color = color; // Apply text color
      resizableElement.style.backgroundColor = color; // Apply background color
    });
  }
}
