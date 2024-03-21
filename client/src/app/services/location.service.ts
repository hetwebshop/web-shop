import { Injectable } from "@angular/core";
import { HttpService } from "./http.service";
import { environment } from "src/environments/environment";
import { Observable, of, tap } from "rxjs";
import { City, Country } from "../models/location";

@Injectable({
    providedIn: 'root',
})
export class LocationService {
    countries: Country[];
    cities: City[];

    baseUrl = environment.apiUrl + 'location/';

    constructor(private http: HttpService) {}

    getCountries(): Observable<Country[]> {
        if (this.countries){
            return of(this.countries);
        }
        return this.http.get<Country[]>(this.baseUrl + "countries").pipe(
            tap(countries => this.countries = countries)
        );
    }

    getCities(): Observable<City[]> {
        if (this.cities){
            return of(this.cities);
        }
        return this.http.get<City[]>(this.baseUrl + "cities").pipe(
            tap(cities => this.cities = cities)
        );    
    }

    // getCantons(): Observable<Canton[]> {
    //     if (this.cantons){
    //         return of(this.cantons);
    //     }
    //     return this.http.get<Canton[]>(this.baseUrl + "cantons").pipe(
    //         tap(cantons => this.cantons = cantons)
    //     );
    // }

    // getMunicipalities(): Observable<Municipality[]> {
    //     if (this.municipalities){
    //         return of(this.municipalities);
    //     }
    //     return this.http.get<Municipality[]>(this.baseUrl + "municipalities").pipe(
    //         tap(municipalities => this.municipalities = municipalities)
    //     );
    // }
}