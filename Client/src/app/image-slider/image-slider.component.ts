import { Component, OnInit } from '@angular/core';
import { PresenceService } from '../_services/presence.service';
import { PhotoMessage } from '../_models/photo-message';

@Component({
  selector: 'app-image-slider',
  templateUrl: './image-slider.component.html',
  styleUrls: ['./image-slider.component.scss']
})
export class ImageSliderComponent implements OnInit {
  currentIndex: number = 0; // Tracks the current slide group
  totalSlides: number = 0; // Total slides
  slideInterval: any;

  readonly visibleImagesCount = 5; // Number of images to show at a time

  constructor(public presenceService: PresenceService) {}

  ngOnInit(): void {
    this.presenceService.photoThread$.subscribe((photos) => {
      this.totalSlides = photos.length;

      // Start autoplay if there are slides
      if (this.totalSlides > this.visibleImagesCount) {
        this.startAutoPlay();
      }
    });
  }

  getTransformStyle(): string {
    // Translate the slider based on the current group of visible slides
    return `translateX(-${(this.currentIndex * 100) / this.visibleImagesCount}%)`;
  }

  getTransitionStyle(): string {
    // Smooth transition
    return 'transform 0.5s ease-in-out';
  }

  startAutoPlay(): void {
    this.slideInterval = setInterval(() => {
      this.currentIndex = (this.currentIndex + 1) % Math.ceil(this.totalSlides / this.visibleImagesCount);
    }, 3000); // Change slide group every 3 seconds
  }

  ngOnDestroy(): void {
    if (this.slideInterval) {
      clearInterval(this.slideInterval);
    }
  }
}
