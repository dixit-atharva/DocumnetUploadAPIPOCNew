import { Component, ElementRef, ViewChild } from '@angular/core';


@Component({
  selector: 'app-main-poc',
  templateUrl: './main-poc.component.html',
  styleUrls: ['./main-poc.component.css']
})
export class MainPOCComponent {
  constructor() { }
  sign = [{
    "Name": "Dixit Gajjar",
    "Mobile": 9638865899,
    "SignType": "aadhar",
    "Id": "1"
  }, {
    "Name": "Rahul Patel",
    "Mobile": 9638865891,
    "SignType": "aadhar",
    "Id": "2"
    }];
  selectedSign = {}
  
  images: string[] = ['../../../assets/Images/1.jpg', '../../../assets/Images/2.jpg'];
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
  addImagesToPDF(event: MouseEvent) {
    debugger;
     const pdfContainer = document.querySelector('.pdf-container');
  if (!pdfContainer) {
    return;
  }
 
  const mouseX = event.clientX; // Mouse X coordinate
  const mouseY = event.clientY; // Mouse Y coordinate

  // Calculate the position of the mouse relative to the pdf-container
  const relativeX = mouseX + window.scrollX;
  const relativeY = mouseY + window.scrollY;

  // Create a container for the image
    if (this.selectedStaticImage) {
      
    const imageContainer = document.createElement('div');
    imageContainer.classList.add('image-container');

    const newImage = document.createElement('img');
    newImage.src = this.selectedStaticImage;
    newImage.style.position = 'absolute';

    // Set the position of the new image based on the mouse coordinates
    newImage.style.left = `${relativeX}px`;
    newImage.style.top = `${relativeY}px`;

    // Create a remove button
    const removeButton = document.createElement('button');
    removeButton.innerText = '✖';
    removeButton.style.position = 'absolute';
    removeButton.classList.add('remove-button');
    removeButton.style.top = `${relativeY}px`;
    removeButton.style.left = `${relativeX + newImage.width - 35}px`;
    removeButton.onclick = () => {
      pdfContainer.removeChild(imageContainer);
    };

    // Append image and remove button to the container
    imageContainer.appendChild(newImage);
    imageContainer.appendChild(removeButton);

    pdfContainer.appendChild(imageContainer);
    this.selectedStaticImage = '';
  }
    if (this.selectedText) {
      debugger;
      const spanContainer = document.createElement('div');
    spanContainer.classList.add('span-container');

    const div = document.createElement('div');
    
      // Set the position of the new image based on the mouse coordinates
      div.style.position = "absolute";
    div.style.left = `${relativeX}px`;
    div.style.top = `${relativeY}px`;
      div.innerText = this.selectedText;
      div.contentEditable = 'true'; 
    // Append image and remove button to the container
    
    

    pdfContainer.appendChild(div);
    this.selectedText = '';
    }
    debugger;
    if (this.showTool3)
    {
        const spanContainer = document.createElement('div');
    spanContainer.classList.add('span-container');

    const div = document.createElement('div');
    
      // Set the position of the new image based on the mouse coordinates
      div.style.position = "absolute";
    div.style.left = `${relativeX}px`;
    div.style.top = `${relativeY}px`;
      
   
      div.style.width = '20px';
      div.style.height = '20px';
      div.style.backgroundColor = 'White'; 
      div.contentEditable = 'false'; 
        this.makeResizable(div);
    // Append image and remove button to the container
    
    

    pdfContainer.appendChild(div);
    }
    if (this.showTool4) {

    const selectElement = document.createElement('select');

      // Add options to select element
    for (let index = 0; index < this.sign.length; index++) {
      
      const option1 = document.createElement('option');
      option1.text = this.sign[index].Name;
      option1.value = this.sign[index].Id;
      selectElement.add(option1);
    }  
    
    const option2 = document.createElement('option');
    option2.text = 'Option 2';

    
    

    
      // Set the position of the new image based on the mouse coordinates
    selectElement.style.position = "absolute";
    selectElement.style.left = `${relativeX}px`;
    selectElement.style.top = `${relativeY}px`; 
    pdfContainer.appendChild(selectElement);
    }
  }
  makeResizable(element: HTMLElement) {
    debugger;
  let isResizing = false;

  element.style.resize = 'both';
  element.style.overflow = 'auto';

    element.addEventListener('click', () => {
     const elements = document.querySelectorAll('.set-color');

// Loop through each element and remove the class
elements.forEach(element => {
  element.classList.remove('set-color');
});
      element.classList.add('set-color');
  });

    
  }
  applyColor(event: any) {
    const color = event.target.value;
   
    const resizableDivs = document.querySelectorAll('.set-color');
  resizableDivs.forEach(div => {
    const resizableElement = div as HTMLElement;
    resizableElement.style.color = color; // Apply text color
    resizableElement.style.backgroundColor = color; // Apply background color
  });
  }
}

