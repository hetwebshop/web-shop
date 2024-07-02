import { Component, EventEmitter, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatOption } from '@angular/material/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, debounceTime, distinctUntilChanged } from 'rxjs';
import { AdvertisementTypeEnum } from 'src/app/models/enums';
import { AdsPaginationParameters } from 'src/app/models/filterCriteria';
import { City } from 'src/app/models/location';
import { AdvertisementType, JobCategory, JobType } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { LocationService } from 'src/app/services/location.service';
import { FiltersQuery } from 'src/app/store/filters/filters.query';
import { FiltersStore } from 'src/app/store/filters/filters.store';


@Component({
  selector: 'app-user-ads-filter',
  templateUrl: './user-ads-filter.component.html',
  styleUrls: ['./user-ads-filter.component.css']
})
export class UserAdsFilterComponent {
  private subscription: Subscription;
  form: FormGroup;
  formChanged: boolean;
  cities: City[] = [];
  jobTypes: JobType[] = [];
  jobCategories: JobCategory[] = [];
  advertisementTypes: AdvertisementType[] = [];
  AdvertisementTypeEnum = AdvertisementTypeEnum;
  selectedAdType: number;
  filterCriteria: AdsPaginationParameters = {
    pageNumber: 1,
    pageSize: 10,
    advertisementTypeId: 1
  };
  filters$ = this.filtersQuery.selectAll();
  @Output() filterSubmitted = new EventEmitter();

  @ViewChild('allCitiesSelected', { static: true }) private allCitiesSelected: MatOption;
  @ViewChild('allJobCategoriesSelected', { static: true }) private allJobCategoriesSelected: MatOption;
  @ViewChild('allJobTypesSelected', { static: true }) private allJobTypesSelected: MatOption;
  @ViewChild('allAdTypesSelected', { static: true }) private allAdTypesSelected: MatOption;
  selectAllId: number = 0;

  constructor(private jobService: JobService, private locationService: LocationService,
    private route: ActivatedRoute, private fb: FormBuilder, private filtersStore: FiltersStore,
    private filtersQuery: FiltersQuery) {
  }

  ngOnInit(): void {
    this.loadCities();
    this.loadJobTypes();
    this.loadJobCategories();

    this.form = this.createForm();
    
    this.filters$.subscribe((filters) => {
      if (filters && filters.length > 0) {
        this.updateForm(filters[0]);
      }
    });
    this.route.queryParams.subscribe(params => {
      if (params['type']) {
        this.selectedAdType = this.getEnumValue(params['type']);
      }
    });

    this.form.valueChanges.pipe(
      debounceTime(300), // Debounce to avoid frequent API calls
      distinctUntilChanged() // Only emit if the value has changed
    ).subscribe(() => {
      this.formChanged = true;
    });
  }

  getEnumValue(name: string): AdvertisementTypeEnum {
    return AdvertisementTypeEnum[name as keyof typeof AdvertisementTypeEnum];
  }

  private updateForm(filter: AdsPaginationParameters): void {
    this.form.patchValue({
      jobTypeIds: filter.jobTypeIds,
      cityIds: filter.cityIds,
      jobCategoryIds: filter.jobCategoryIds,
      fromDate: filter.fromDate,
      toDate: filter.toDate,
      searchKeyword: filter.searchKeyword
    });
  }

  private createForm(): FormGroup {
    return this.fb.group({
      jobTypeIds: new FormControl(null),
      cityIds: new FormControl(null),
      jobCategoryIds: new FormControl(null),
      fromDate: new FormControl(null),
      toDate: new FormControl(null),
      searchKeyword: new FormControl(null)
    });
  }

  validateForm() {
    const fromDate = this.form.get('fromDate').value;
    const toDate = this.form.get('toDate').value;

    if (fromDate && toDate && toDate < fromDate) {
      this.form.get('toDate').setErrors({ toDateGreaterThanFromDate: true });
      return false;
    } else {
      this.form.get('toDate').setErrors(null);
      return true;
    }
  }

  onSubmit(): void {
    if (!this.validateForm())
      return;
    //if (this.formChanged) {
      // Send request to backend to fetch filtered items
      this.filterCriteria = {
        advertisementTypeId: 1,
        jobTypeIds: this.form.get('jobTypeIds')?.value,
        jobCategoryIds: this.form.get('jobCategoryIds')?.value,
        cityIds: this.form.get('cityIds')?.value,
        searchKeyword: this.form.get('searchKeyword')?.value,
        fromDate: this.form.get('fromDate')?.value,
        toDate: this.form.get('toDate')?.value
      };
      this.filterSubmitted.emit(this.filterCriteria);
      //this.filtersStore.set([this.filterCriteria]);
      this.formChanged = false; // Reset flag
    //}
  }

  onPrevious() {
    this.filterCriteria.pageNumber--;
    this.formChanged = true;
    this.onSubmit();
  }
  onNext() {
    this.filterCriteria.pageNumber++;
    this.formChanged = true;
    this.onSubmit();
  }

  loadCities(): void {
    this.locationService.getCities()
      .subscribe(cities => {
        this.cities = cities;
      });
  }

  loadJobTypes(): void {
    this.jobService.getJobTypes()
      .subscribe(types => {
        this.jobTypes = types;
      });
  }

  loadJobCategories(): void {
    this.jobService.getJobCategories()
      .subscribe(categories => {
        this.jobCategories = categories.filter(r => r.parentId == null);
      });
  }

  tosslePerOne(controlName: string): boolean {
    const control = this.form.get(controlName);

    if (controlName.includes("advertisementTypeIds")) {
      if (this.allAdTypesSelected.selected) {
        this.allAdTypesSelected.deselect();
        return false;
      }
      if (control.value.length == this.advertisementTypes.length)
        this.allAdTypesSelected.select();
    }
    else if (controlName.includes("cityIds")) {
      if (this.allCitiesSelected.selected) {
        this.allCitiesSelected.deselect();
        return false;
      }
      if (control.value.length == this.cities.length)
        this.allCitiesSelected.select();
    }
    else if (controlName.includes("jobCategoryIds")) {
      if (this.allJobCategoriesSelected.selected) {
        this.allJobCategoriesSelected.deselect();
        return false;
      }
      if (control.value.length == this.jobCategories.length)
        this.allJobCategoriesSelected.select();
    }
    else if (controlName.includes("jobTypeIds")) {
      if (this.allJobTypesSelected.selected) {
        this.allJobTypesSelected.deselect();
        return false;
      }
      if (control.value.length == this.jobTypes.length)
        this.allJobTypesSelected.select();
    }
  }


  toggleAllSelection(controlName: string): void {
    const control = this.form.get(controlName);
    console.log(control);
    if (controlName.includes("advertisementTypeIds")) {
      console.log("from if");
      console.log(this.advertisementTypes);
      if (this.allAdTypesSelected.selected) {
        control.patchValue([...this.advertisementTypes.map(adType => adType.id), this.selectAllId]);
      } else {
        control.patchValue([]);
      }
    }
    else if (controlName.includes("cityIds")) {
      if (this.allCitiesSelected.selected) {
        control.patchValue([...this.cities.map(city => city.id), this.selectAllId]);
      } else {
        control.patchValue([]);
      }
    }
    else if (controlName.includes("jobCategoryIds")) {
      if (this.allJobCategoriesSelected.selected) {
        control.patchValue([...this.jobCategories.map(jobCategory => jobCategory.id), this.selectAllId]);
      } else {
        control.patchValue([]);
      }
    }
    else if (controlName.includes("jobTypeIds")) {
      if (this.allJobTypesSelected.selected) {
        control.patchValue([...this.jobTypes.map(jobType => jobType.id), this.selectAllId]);
      } else {
        control.patchValue([]);
      }
    }
  }
}
