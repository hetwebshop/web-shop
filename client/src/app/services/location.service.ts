import { Injectable } from "@angular/core";
import { HttpService } from "./http.service";
import { environment } from "src/environments/environment";
import { Observable, of, tap } from "rxjs";
import { City, Country } from "../models/location";
import { CityQuery } from "../store/jobsHelpers/cities.query";
import { CityStore } from "../store/jobsHelpers/cities.store";
import { CountryStore } from "../store/jobsHelpers/countries.store";
import { CountryQuery } from "../store/jobsHelpers/countries.query";

@Injectable({
    providedIn: 'root',
})
export class LocationService {
    baseUrl = environment.apiUrl + 'location/';

    constructor(private http: HttpService, private cityStore: CityStore, private cityQuery: CityQuery,
        private countryStore: CountryStore, private countryQuery: CountryQuery) {}
    
    getCities(): Observable<City[]> {
        const cities = this.cityQuery.getAll();
        if(cities && cities.length > 0){
            return of(cities);
        }

        return this.http.get<City[]>(this.baseUrl + "cities").pipe(
            tap(cities => this.cityStore.set(cities))
        );    
    }
    getCountries(): Observable<Country[]> {
        const countries = this.countryQuery.getAll();
        if (countries && countries.length > 0){
            return of(countries);
        }
        return this.http.get<Country[]>(this.baseUrl + "countries").pipe(
            tap(countries => this.countryStore.set(countries))
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