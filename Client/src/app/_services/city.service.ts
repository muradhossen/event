import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';
import { City } from '../_models/city';
import { UserPhotoParams } from '../_models/userPhotoParams';

@Injectable({
  providedIn: 'root',
})
export class CityService {
  baseUrl = environment.apiUrl;
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(
    private http: HttpClient,
    private presenceService: PresenceService
  ) {}

  getCities() {
    return this.http.get<City[]>(this.baseUrl + 'Cities');
  }

  uploadImage(cityParams: UserPhotoParams) {
    return this.http.post(this.baseUrl + 'Users/upload-photo', mapToFomData(cityParams));
  }
}
function mapToFomData(cityParams: UserPhotoParams): any {
  const formData = new FormData();
  formData.append('id', cityParams.id.toString());
  formData.append('image', cityParams.image);
  return formData;
}
