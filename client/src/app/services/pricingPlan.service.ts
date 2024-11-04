import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { HttpService } from "./http.service";
import { Observable } from "rxjs";
import { PricingPlan } from "../models/pricingPlan";

@Injectable({
    providedIn: 'root',
})
export class PricingPlanService {
    baseUrl = environment.apiUrl + 'pricing/';

    constructor(private http: HttpService) {}

    getAllPricingPlans(): Observable<PricingPlan[]> {
        return this.http.get<PricingPlan[]>(this.baseUrl + "pricingplans").pipe();    
    }
}