import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import * as docx from 'docx-preview';

@Component({
  selector: 'app-docx-preview',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './docx-preview.component.html',
  styleUrl: './docx-preview.component.css'
})
export class DocxPreviewComponent {
  docContent: string|undefined;
  placeholders = ['{{Name}}', '{{Address}}', '{{Date}}'];

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        docx.renderAsync(e.target.result, document.body)
          .then(html => this.docContent = html.outerHTML)
          .catch(error => console.log(error));
      };
      reader.readAsArrayBuffer(file);
    }
  }
}
