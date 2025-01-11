import { Component, OnInit } from '@angular/core';
import { PresenceService } from '../_services/presence.service';
import { PhotoMessage } from '../_models/photo-message';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  districtCoordinates = {
    Bagerhat: { lat: 22.651568, lng: 89.785938 },
    Bandarban: { lat: 22.195327, lng: 92.218377 },
    Barguna: { lat: 22.156984, lng: 90.125132 },
    Barisal: { lat: 22.701002, lng: 90.353451 },
    Bhola: { lat: 22.687146, lng: 90.644397 },
    Bogura: { lat: 24.846522, lng: 89.377755 },
    Brahmanbaria: { lat: 23.957090, lng: 91.111368 },
    Chandpur: { lat: 23.233258, lng: 90.671291 },
    Chattogram: { lat: 22.356851, lng: 91.783182 },
    Chuadanga: { lat: 23.640196, lng: 88.841841 },
    "Cox's Bazar": { lat: 21.427229, lng: 92.005806 },
    Cumilla: { lat: 23.460889, lng: 91.180906 },
    Dhaka: { lat: 23.810331, lng: 90.412521 },
    Dinajpur: { lat: 25.621706, lng: 88.635450 },
    Faridpur: { lat: 23.606119, lng: 89.841387 },
    Feni: { lat: 23.020359, lng: 91.396330 },
    Gaibandha: { lat: 25.328751, lng: 89.528088 },
    Gazipur: { lat: 23.999940, lng: 90.420273 },
    Gopalganj: { lat: 23.005085, lng: 89.826605 },
    Habiganj: { lat: 24.374945, lng: 91.415530 },
    Jamalpur: { lat: 24.937533, lng: 89.937775 },
    Jashore: { lat: 23.169789, lng: 89.213043 },
    Jhalokati: { lat: 22.640575, lng: 90.200626 },
    Jhenaidah: { lat: 23.544817, lng: 89.153921 },
    Joypurhat: { lat: 25.096803, lng: 89.026489 },
    Khagrachhari: { lat: 23.119392, lng: 91.984663 },
    Khulna: { lat: 22.845641, lng: 89.540329 },
    Kishoreganj: { lat: 24.444937, lng: 90.776575 },
    Kurigram: { lat: 25.807282, lng: 89.654916 },
    Kushtia: { lat: 23.901258, lng: 89.120482 },
    Lakshmipur: { lat: 22.942477, lng: 90.841184 },
    Lalmonirhat: { lat: 25.992328, lng: 89.284725 },
    Madaripur: { lat: 23.164557, lng: 90.189763 },
    Magura: { lat: 23.487337, lng: 89.419956 },
    Manikganj: { lat: 23.854109, lng: 90.004784 },
    Meherpur: { lat: 23.763927, lng: 88.631841 },
    Moulvibazar: { lat: 24.482934, lng: 91.777417 },
    Munshiganj: { lat: 23.542074, lng: 90.530290 },
    Mymensingh: { lat: 24.747148, lng: 90.420273 },
    Naogaon: { lat: 24.815634, lng: 88.948017 },
    Narail: { lat: 23.164102, lng: 89.496573 },
    Narayanganj: { lat: 23.622935, lng: 90.499699 },
    Narsingdi: { lat: 23.932233, lng: 90.717989 },
    Natore: { lat: 24.420556, lng: 89.000000 },
    Netrokona: { lat: 24.883497, lng: 90.727481 },
    Nilphamari: { lat: 25.929666, lng: 88.856006 },
    Noakhali: { lat: 22.869563, lng: 91.098772 },
    Pabna: { lat: 23.998524, lng: 89.233645 },
    Panchagarh: { lat: 26.341081, lng: 88.554182 },
    Patuakhali: { lat: 22.359631, lng: 90.329871 },
    Pirojpur: { lat: 22.579672, lng: 89.975035 },
    Rajbari: { lat: 23.757430, lng: 89.644466 },
    Rajshahi: { lat: 24.363588, lng: 88.624135 },
    Rangamati: { lat: 22.637743, lng: 92.203882 },
    Rangpur: { lat: 25.746679, lng: 89.250701 },
    Satkhira: { lat: 22.718562, lng: 89.070844 },
    Shariatpur: { lat: 23.241490, lng: 90.434317 },
    Sherpur: { lat: 25.020493, lng: 90.017535 },
    Sirajganj: { lat: 24.453397, lng: 89.700684 },
    Sunamganj: { lat: 25.065804, lng: 91.395011 },
    Sylhet: { lat: 24.894930, lng: 91.868706 },
    Tangail: { lat: 24.249844, lng: 89.916457 },
    Thakurgaon: { lat: 26.033694, lng: 88.461683 },
    "Chapai Nawabganj": { lat: 24.596503, lng: 88.277512 }
  };
  

  photoMessages: PhotoMessage[] = [];
  allocatedPins: {x : number, y : number}[] = [];


  mapBounds = {
    topLeft: { lat: 26.6319, lon: 88.0844 },
    bottomRight: { lat: 20.6709, lon: 92.6442 },
  };
  mapDimensions = { width: 555, height: 741 };

  constructor(public presenceService: PresenceService) {

  }

  ngOnInit() {
    // this.presenceService.photoThread$.pipe(take(1)).subscribe((res) => {
    //   [... this.photoMessages, res]
    //   console.log("Photo messages ", this.photoMessages);
    // });

    this.presenceService.pin$.subscribe((res) => {
       
      if (res?.city) {
        this.addPin(res.city);
      }
    });
  }

  addPin(city: string) {

    debugger
    console.log('City ', city);
    const mapImage = document.getElementById('mapContainer');
    const districtSelect = city;

    const districtCoords = this.districtCoordinates[districtSelect];

    if (districtCoords) {
      // const pinPosition = calculatePinPosition(districtCoords.lat, districtCoords.lng);
      const pinPosition = this.geoToPixel(
        districtCoords.lat,
        districtCoords.lng
      );

      if (this.allocatedPins.some(p => p.x === pinPosition.x && p.y === pinPosition.y)) {
        
        
        pinPosition.x += Math.random() * 10;
        pinPosition.y += Math.random() * 10;
        
      }

      this.allocatedPins.push(pinPosition);

      // Create an img element for the pin
      const pin = document.createElement('img');
      pin.style.position = 'absolute';
      pin.style.left = `${pinPosition.x + 8}px`;
      pin.style.top = `${pinPosition.y}px`;
      pin.style.width = '15px';
      pin.style.height = '20px';
      // pin.src = '../../assets/locator-icon.png';
      pin.src = 'https://res.cloudinary.com/do7pdjcnd/image/upload/v1736489157/Event/locator-icon_nwtxoh.png';

      pin.alt = '.';

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
