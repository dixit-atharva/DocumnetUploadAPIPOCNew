import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DocUploadComponent } from './Pages/doc-upload/doc-upload.component';
import { DocUploadPOCComponent } from './Pages/doc-upload-poc/doc-upload-poc.component';
import { MainPOCComponent } from './Pages/main-poc/main-poc.component';

const routes: Routes = [
  { path: '', component: DocUploadComponent },
  { path: 'DocUploadPOC', component: DocUploadPOCComponent },
  { path: 'main', component: MainPOCComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
