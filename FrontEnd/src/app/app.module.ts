import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DocUploadComponent } from './Pages/doc-upload/doc-upload.component';
import { HttpClientModule } from '@angular/common/http';
import { DocUploadPOCComponent } from './Pages/doc-upload-poc/doc-upload-poc.component';
import { MainPOCComponent } from './Pages/main-poc/main-poc.component';
import { FormsModule } from '@angular/forms';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { Ng2PdfjsViewerComponent } from './Pages/ng2-pdfjs-viewer/ng2-pdfjs-viewer.component';
import { PdfJsViewerModule } from 'ng2-pdfjs-viewer';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { NgxExtendedPdfViewerComponentTest } from './Pages/ngx-extended-pdf-viewer/ngx-extended-pdf-viewer.component';



@NgModule({
  declarations: [
    AppComponent,
    DocUploadComponent,
    DocUploadPOCComponent,
    DocUploadPOCComponent,
    MainPOCComponent,
    NgxExtendedPdfViewerComponentTest,
    Ng2PdfjsViewerComponent    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    PdfViewerModule,
    PdfJsViewerModule,
    NgxExtendedPdfViewerModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
