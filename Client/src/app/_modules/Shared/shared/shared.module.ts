import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxGalleryModule } from '@kolkov/ngx-gallery';
import { HttpClientModule } from '@angular/common/http';
import { FileUploadModule } from 'ng2-file-upload';
import {BsDatepickerModule} from 'ngx-bootstrap/datepicker'

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    TabsModule.forRoot(),
    NgxGalleryModule,
    HttpClientModule,
    FileUploadModule,
    BsDatepickerModule.forRoot()
  ],
  exports: [
    TabsModule,
    NgxGalleryModule,
    FileUploadModule,
    BsDatepickerModule
  ]
})
export class SharedModule { }
