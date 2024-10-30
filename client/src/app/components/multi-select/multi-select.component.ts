import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-multi-select',
  templateUrl: './multi-select.component.html',
  styleUrls: ['./multi-select.component.css'],
})
export class MultiSelectComponent implements OnInit {
  @Input() name: string = '';
  @Input() values: any[] = []; // Update to 'any[]' for an array of objects
  _value: string[] = [];
  searchTerm: string = '';
  filteredValues: any[] = [];

  get value(): string {
    return this._value?.join(',');
  }

  @Input() set value(inpValue: string) {
    this._value = inpValue?.split(',');
    this.filteredValues = this.values; // Initialize filtered values on value change
  }

  @Output() valueChange: EventEmitter<string> = new EventEmitter<string>();

  constructor() {}

  ngOnInit(): void {
    this.filteredValues = this.values; // Initialize filtered values
  }

  onChange = () => {
    this.valueChange.emit(this._value.join(','));
  };

  filterItems() {
    this.filteredValues = this.values.filter(item =>
      item.name.toLowerCase().includes(this.searchTerm.toLowerCase())
    );
  }

  clearSearch() {
    this.searchTerm = '';
    this.filteredValues = this.values; // Reset filtered values to original
  }
}
