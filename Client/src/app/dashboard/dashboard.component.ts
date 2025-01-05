import { Component, OnInit } from '@angular/core';
import { PresenceService } from '../_services/presence.service';
import { take } from 'rxjs/operators';
import { PhotoMessage } from '../_models/photo-message';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {

  districtCoordinates = {
    "Dhaka": { lat: 23.81033, lng: 90.41252 },
    "Chittagong": { lat: 22.3569, lng: 91.8355 },
    "Joypurhat": { lat: 25.105101, lng: 89.028877 },
  };

  photoMessages : PhotoMessage[] = [];
  
  mapBounds = {
    topLeft: { lat: 26.6319, lon: 88.0844 },
    bottomRight: { lat: 20.6709, lon: 92.6442 }, 
  };
  mapDimensions = { width: 576, height: 751 };

  constructor(public presenceService: PresenceService) {}

  ngOnInit() {
    // this.presenceService.photoThread$.pipe(take(1)).subscribe((res) => { 
 
    //   [... this.photoMessages, res]

    //   console.log("Photo messages ", this.photoMessages);
    // });
  }

  addPin() {
    const mapImage = document.getElementById('mapContainer'); 
    const districtSelect = 'Dhaka';

    const districtCoords =  this.districtCoordinates[districtSelect];

    if (districtCoords) {
      // const pinPosition = calculatePinPosition(districtCoords.lat, districtCoords.lng);
      const pinPosition = this.geoToPixel(districtCoords.lat, districtCoords.lng);
  
      
      const pin = document.createElement('div');
      pin.style.position = 'absolute';
      pin.style.color = 'red';
      pin.style.left = `${pinPosition.x }px`;  
      pin.style.top = `${pinPosition.y }px`; 
      pin.style.width = '10px';
      pin.style.height = '10px';
      pin.style.borderRadius = '50%';
      pin.style.backgroundColor = 'red'; 
  
      mapImage.appendChild(pin); 
    }
  }

  private geoToPixel(lat: number, lon: number): { x: number; y: number } {
    const { topLeft, bottomRight } = this.mapBounds;
    const { width, height } = this.mapDimensions;
  
 
    const x = ((lon - topLeft.lon) / (bottomRight.lon - topLeft.lon)) * width;
    const y = ((topLeft.lat - lat) / (topLeft.lat - bottomRight.lat)) * height;
  
    return { x, y };
  }
}

function calculatePinPosition(lat, lng) {
  const mapWidth = 624;
  const mapHeight = 850;

  const minLat = 20; // Replace with minimum latitude of your map
  const maxLat = 27; // Replace with maximum latitude of your map
  const minLng = 88; // Replace with minimum longitude of your map
  const maxLng = 93; // Replace with maximum longitude of your map

  const x = ((lng - minLng) / (maxLng - minLng)) * mapWidth;
  const y = ((maxLat - lat) / (maxLat - minLat)) * mapHeight; // Invert Y-axis for correct positioning


  return { x: x, y: y };
}
