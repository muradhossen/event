import { Component, OnInit } from '@angular/core';
import { CityService } from '../_services/city.service';
import { PhotoMessage } from '../_models/photo-message';

@Component({
  selector: 'app-slide-image-list',
  templateUrl: './slide-image-list.component.html',
  styleUrls: ['./slide-image-list.component.scss']
})
export class SlideImageListComponent implements OnInit {

  list : PhotoMessage[] = [];
  constructor(private slideImageService : CityService) { }

  ngOnInit() {

    this.slideImageService.GetSlideImageList().subscribe(data => {
       this.list = data;
       console.log(data);
    }, error => {
      console.log(error);
    });
  }

  deleteSlideImage(id : number){
    this.slideImageService.DeleteSlideImage(id).subscribe(data => {
      this.list = this.list.filter(x => x.id != id);
    }, error => {
      console.log(error);
    });
  }

}
