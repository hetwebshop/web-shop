import { Component, ElementRef, EventEmitter, HostListener, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatOption } from '@angular/material/core';
import { MatInput } from '@angular/material/input';
import { MatSelect } from '@angular/material/select';
import { ActivatedRoute } from '@angular/router';
import { Subject, Subscription, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
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
export class UserAdsFilterComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
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
  filteredCities = []; 
  isCitiesSearchActive = false; 
  isCategoriesSearchActive = false;
  filteredCategories = []; 
  isJobTypesSearchActive = false;
  filteredJobTypes = []; 
  searchKeyword = "";
  citieSearchKeyword = "";
  jobTypesSearchKeyword = "";
  jobCategoriesSearchKeyword = "";
  selectedCityIds: number[] = [];
  selectedJobCategoryIds: number[] = [];
  selectedJobTypeIds: number[] = [];
  @Output() filterSubmitted = new EventEmitter();

  @ViewChild('allCitiesSelected', { static: false }) private allCitiesSelected: MatOption;
  @ViewChild('allJobCategoriesSelected', { static: false }) private allJobCategoriesSelected: MatOption;
  @ViewChild('allJobTypesSelected', { static: false }) private allJobTypesSelected: MatOption;
  @ViewChild('allAdTypesSelected', { static: false }) private allAdTypesSelected: MatOption;
  @ViewChild('searchInput', { static: false }) private searchInput: ElementRef;
  selectAllId: number = 0;

  constructor(private jobService: JobService, private locationService: LocationService,
    private route: ActivatedRoute, private fb: FormBuilder, private filtersStore: FiltersStore,
    private filtersQuery: FiltersQuery) {
  }

  ngOnInit(): void {
    this.loadCities();
    this.loadJobTypes();
    this.loadJobCategories();

    console.log("JOB CATEGORIES ", this.jobCategories);
    this.form = this.createForm();
    
    this.filters$.pipe(takeUntil(this.destroy$)).subscribe((filters) => {
      if (filters && filters.length > 0) {
        this.updateForm(filters[0]);
      }
    });
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(params => {
      if (params['type']) {
        this.selectedAdType = this.getEnumValue(params['type']);
      }
    });

    this.form.valueChanges.pipe(
      debounceTime(300), // Debounce to avoid frequent API calls
      distinctUntilChanged() // Only emit if the value has changed
    ).pipe(takeUntil(this.destroy$)).subscribe(() => {
      this.formChanged = true;
    });
  }

  onSelectionChange(id: number, formControlName: string): void {
    if (formControlName === "cityIds") {
      this.handleSelectionChange(
        id, 
        this.selectedCityIds, 
        this.cities.map(city => city.id), 
        this.selectAllId, 
        'cityIds'
      );
    } 
    else if (formControlName === "jobCategoryIds") {
      this.handleSelectionChange(
        id, 
        this.selectedJobCategoryIds, 
        this.jobCategories.map(category => category.id), 
        this.selectAllId, 
        'jobCategoryIds'
      );
    } 
    else if (formControlName === "jobTypeIds") {
      this.handleSelectionChange(
        id, 
        this.selectedJobTypeIds, 
        this.jobTypes.map(type => type.id), 
        this.selectAllId, 
        'jobTypeIds'
      );
    }
  }
  
  private handleSelectionChange(
    id: number, 
    selectedIds: number[], 
    allIds: number[], 
    selectAllId: number, 
    controlName: string
  ): void {
    const selectAllSelected = selectedIds.includes(selectAllId);
  
    if (id === selectAllId) {
      // Toggle Select All
      if (selectAllSelected) {
        selectedIds.length = 0; // Clear all selections
      } else {
        selectedIds = [...allIds, selectAllId]; // Select all items including "Select All"
      }
    } else {
      // Handle individual item selection
      const index = selectedIds.indexOf(id);
      if (index !== -1) {
        selectedIds.splice(index, 1);
      } else {
        selectedIds.push(id);
      }
  
      // Check if all items are selected
      const allItemsSelected = allIds.every(itemId => selectedIds.includes(itemId));
  
      if (allItemsSelected && !selectAllSelected) {
        selectedIds.push(selectAllId);
      } else if (!allItemsSelected && selectAllSelected) {
        selectedIds = selectedIds.filter(selectedId => selectedId !== selectAllId);
      }
    }
  
    this.form.get(controlName)?.patchValue(selectedIds);
  }
  
  

  onKey(searchValue: string, formControlName: string) {
    if(formControlName == "cityIds"){
      this.citieSearchKeyword = searchValue;
      this.isCitiesSearchActive = searchValue.length > 0;
      this.filteredCities = this.cities.filter(city =>
        city.name.toLowerCase().includes(searchValue.toLowerCase())
      );
    } 
    else if(formControlName == "jobCategoryIds"){
      this.jobCategoriesSearchKeyword = searchValue;
      this.isCategoriesSearchActive = searchValue.length > 0;
      this.filteredCategories = this.jobCategories.filter(cg =>
        cg.name.toLowerCase().includes(searchValue.toLowerCase())
      );
    } 
    else if(formControlName == "jobTypeIds"){
      this.jobTypesSearchKeyword = searchValue;
      this.isJobTypesSearchActive = searchValue.length > 0;
      this.filteredJobTypes = this.jobTypes.filter(jt =>
        jt.name.toLowerCase().includes(searchValue.toLowerCase())
      );
    } 
  }

  resetSearch(formControlName: string): void {
    this.citieSearchKeyword = "";
    this.jobTypesSearchKeyword = "";
    this.jobCategoriesSearchKeyword = "";
    if(formControlName == "cityIds"){
      this.isCitiesSearchActive = false;
      this.filteredCities = [...this.cities];
    }
    else if(formControlName == "jobCategoryIds"){
      this.isJobTypesSearchActive = false;
      this.filteredCategories = [...this.jobCategories];
    }
    else if(formControlName == "jobTypeIds"){
      this.isJobTypesSearchActive = false;
      this.filteredJobTypes = [...this.jobTypes];
    }
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
    this.locationService.getCities().pipe(takeUntil(this.destroy$))
      .subscribe(cities => {
        this.cities = cities;
        this.filteredCities = cities;
      });
  }

  loadJobTypes(): void {
    this.jobService.getJobTypes().pipe(takeUntil(this.destroy$))
      .subscribe(types => {
        this.jobTypes = types;
        this.filteredJobTypes = types;
      });
  }

  loadJobCategories(): void {
    this.jobService.getJobCategories().pipe(takeUntil(this.destroy$))
      .subscribe(categories => {
        this.jobCategories = categories.filter(r => r.parentId == null);
        this.filteredCategories = this.jobCategories;
      });
  }

  tosslePerOne(controlName: string): boolean {
    const control = this.form.get(controlName);

    if (controlName.includes("advertisementTypeIds")) {
      if (this.allAdTypesSelected?.selected) {
        this.allAdTypesSelected.deselect();
        return false;
      }
      if (control.value.length == this.advertisementTypes?.length)
        this.allAdTypesSelected.select();
    }
    else if (controlName.includes("cityIds")) {
      if (this.allCitiesSelected?.selected) {
        this.allCitiesSelected?.deselect();
        return false;
      }
      if (control.value.length == this.cities.length)
        this.allCitiesSelected?.select();
    }
    else if (controlName.includes("jobCategoryIds")) {
      if (this.allJobCategoriesSelected?.selected) {
        this.allJobCategoriesSelected?.deselect();
        return false;
      }
      if (control.value.length == this.jobCategories.length)
        this.allJobCategoriesSelected?.select();
    }
    else if (controlName.includes("jobTypeIds")) {
      if (this.allJobTypesSelected?.selected) {
        this.allJobTypesSelected?.deselect();
        return false;
      }
      if (control.value.length == this.jobTypes.length)
        this.allJobTypesSelected?.select();
    }
  }

  toggleSingleSelection(cityId: number) {
    console.log("TEES5", cityId)

    const currentSelection = this.form.get('cityIds').value || [];
    console.log('Current Selection:', currentSelection);
    const index = currentSelection.indexOf(cityId);
    console.log('Index:', index);
    if (index === -1) {
      console.log("TEES2112")
      // If not selected, add to the selection
      this.form.get('cityIds').patchValue([...currentSelection, cityId]);
    } else {
      // If already selected, remove from the selection
      currentSelection.splice(index, 1);
      this.form.get('cityIds').setValue(currentSelection);
    }
  }

  toggleAllSelection(controlName: string): void {
    const control = this.form.get(controlName);
    console.log(control);
    if (controlName.includes("cityIds")) {
      if (this.allCitiesSelected?.selected) {
        const allCities = [...this.cities.map(city => city.id), this.selectAllId];
        this.selectedCityIds = allCities
        control.patchValue(allCities);
      } else {
        control.patchValue([]);
        this.selectedCityIds = [];
      }
    }
    else if (controlName.includes("jobCategoryIds")) {
      if (this.allJobCategoriesSelected?.selected) {
        const allCategories = [...this.jobCategories.map(jobCategory => jobCategory.id), this.selectAllId];
        this.selectedJobCategoryIds = allCategories;
        control.patchValue(allCategories);
      } else {
        control.patchValue([]);
        this.selectedJobCategoryIds = [];
      }
    }
    else if (controlName.includes("jobTypeIds")) {
      if (this.allJobTypesSelected?.selected) {
        const allJobTypes = [...this.jobTypes.map(jobType => jobType.id), this.selectAllId];
        this.selectedJobTypeIds = allJobTypes;
        control.patchValue(allJobTypes);
      } else {
        control.patchValue([]);
        this.selectedJobTypeIds = [];
      }
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
