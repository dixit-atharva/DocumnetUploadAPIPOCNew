import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DocUploadComponent } from './Pages/doc-upload/doc-upload.component';
import { DocUploadPOCComponent } from './Pages/doc-upload-poc/doc-upload-poc.component';
import { MainPOCComponent } from './Pages/main-poc/main-poc.component';
import { Ng2PdfjsViewerComponent } from './Pages/ng2-pdfjs-viewer/ng2-pdfjs-viewer.component';
import { NgxExtendedPdfViewerComponent } from './Pages/ngx-extended-pdf-viewer/ngx-extended-pdf-viewer.component';

const routes: Routes = [
  { path: '', component: DocUploadComponent },
  { path: 'DocUploadPOC', component: DocUploadPOCComponent },
  { path: 'main', component: MainPOCComponent },
  { path: 'ng2-pdfjs-viewer', component: Ng2PdfjsViewerComponent },
  { path: 'ngx-extended-pdf-viewer', component: NgxExtendedPdfViewerComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
