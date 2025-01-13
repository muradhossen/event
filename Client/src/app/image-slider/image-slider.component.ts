import { Component, OnInit } from '@angular/core';
import { PresenceService } from '../_services/presence.service';
import { PhotoMessage } from '../_models/photo-message';

@Component({
  selector: 'app-image-slider',
  templateUrl: './image-slider.component.html',
  styleUrls: ['./image-slider.component.scss']
})
export class ImageSliderComponent implements OnInit {

  photoMessages: PhotoMessage [] = [];
  autoplayInterval: any;

  constructor(public presenceService: PresenceService) { }

  ngOnInit() {

    this.presenceService.pin$.subscribe((res) => {

      if (res?.photoUrl) {
        this.photoMessages.push(res);
      }
    });
  }
 
  
  currentIndex = 0;
  visibleImages = 5;

  moveSlide(direction: number): void {
    this.stopAutoplay(); // Stop autoplay when navigating manually

    if (direction === 1) {
      this.currentIndex = (this.currentIndex + 1) % this.photoMessages.length;
    } else {
      this.currentIndex =
        (this.currentIndex - 1 + this.photoMessages.length) % this.photoMessages.length;
    }

    this.startAutoplay(); // Restart autoplay after manual navigation
  }

  getTransformStyle(): string {
    return `translateX(-${this.currentIndex * (100 / this.visibleImages)}%)`;
  }

  startAutoplay(): void {
    this.autoplayInterval = setInterval(() => {
      this.moveSlide(1);
    }, 1000); 
  }

  stopAutoplay(): void {
    if (this.autoplayInterval) {
      clearInterval(this.autoplayInterval);
      this.autoplayInterval = null;
    }
  }

  ngOnDestroy(): void {
    this.stopAutoplay();
  }
}
