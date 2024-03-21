import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { LocationInfo } from '../modal/address';
import { HttpService } from './http.service';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  baseUrl = environment.apiUrl + 'admin/';
  adminUrl = this.baseUrl + 'moderate/admin-role';
  moderateTrackUrl = this.baseUrl + 'moderate/track-role';
  moderateStorekUrl = this.baseUrl + 'moderate/store-role';
  trackAdminUrl = this.baseUrl + 'track-role';
  storeAdminUrl = this.baseUrl + 'store-role';

  constructor(private http: HttpService) {}

  searchLocations(name: string, type: string, role: string) {
    return this.http.get<LocationInfo[]>(this.baseUrl + 'search-locations', {
      background: true,
      params: { name, type, for: role },
    });
  }
}
