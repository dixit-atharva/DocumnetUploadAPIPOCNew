import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DocUploadComponent } from './Pages/doc-upload/doc-upload.component';
import { HttpClientModule } from '@angular/common/http';
import { DocUploadPOCComponent } from './Pages/doc-upload-poc/doc-upload-poc.component';
import { MainPOCComponent } from './Pages/main-poc/main-poc.component';
import { FormsModule, NgForm, ReactiveFormsModule } from '@angular/forms';
import { PdfViewerModule } from 'ng2-pdf-viewer';
import { Ng2PdfjsViewerComponent } from './Pages/ng2-pdfjs-viewer/ng2-pdfjs-viewer.component';
import { PdfJsViewerModule } from 'ng2-pdfjs-viewer';
import { NgxExtendedPdfViewerModule } from 'ngx-extended-pdf-viewer';
import { IonicModule } from '@ionic/angular';
import { NgxExtendedPdfViewerComponentTest } from './Pages/ngx-extended-pdf-viewer/ngx-extended-pdf-viewer.component';
import { RECAPTCHA_V3_SITE_KEY, RecaptchaModule, RecaptchaV3Module } from 'ng-recaptcha';
import { environment } from 'src/environments/environment';



@NgModule({
  declarations: [
    AppComponent,
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
    NgxExtendedPdfViewerModule,
    IonicModule,
    RecaptchaV3Module,
    ReactiveFormsModule,
    RecaptchaModule
  ],
  providers: [{
    provide: RECAPTCHA_V3_SITE_KEY,
    useValue: environment.recaptcha.siteKey,
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }
