import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { CityService } from '../_services/city.service';
import { City } from '../_models/city';
import { UserPhotoParams } from '../_models/userPhotoParams';
import { environment } from 'src/environments/environment';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-upload-image',
  templateUrl: './upload-image.component.html',
  styleUrls: ['./upload-image.component.scss']
})
export class UploadImageComponent implements OnInit {

  userPhotoParams : UserPhotoParams = new UserPhotoParams() ;
  @ViewChild('form') form: NgForm;
  @ViewChild('fileInput') fileInput: ElementRef;

  constructor(
        private cityService : CityService,
         private cdr: ChangeDetectorRef, 
         private toster: ToastrService) { }

  cities : City[] = [];
  image: File;
  imageUrl = environment.defaultItemImagePath;

  ngOnInit() {
    this.cityService.getCities().subscribe({
      next : res => {
        this.cities = res;
      } 
    })
  }

  submitImage(){ 

   this.cityService.uploadImage(this.userPhotoParams).subscribe({

    next : res => { 
      this.form.reset();   
      this.fileInput.nativeElement.value = '';
      this.toster.success('Image uploaded successfully'); 
    }
   });
    
  }


    // Method to handle file selection
    onFileSelected(event: any) {
      if (event.target.files.length > 0) {
        const file = event.target.files[0];
        this.image = file;
        this.userPhotoParams.image = file;
  
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = (e: any) => {
          this.imageUrl = e.target.result;
          this.cdr.detectChanges();
        }; 
  
      }
    }
 

}
