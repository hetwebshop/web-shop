import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.css'],
})
export class PaginationComponent implements OnInit {
  selected: number;
  range: number[] = [];
  pageSize: number = 10;
  pageSizeOptions: number[] = [10, 20, 50];
  tempListSize: number = 5;

  @Input() listSize: number = 5;
  _totalPages: number;
  @Input() set totalPages(value: number) {
    this._totalPages = value;
    this.setPagination();
  }
  get page(): number {
    return this.selected;
  }
  @Input() set page(value: number) {
    this.selected = value;
    this.setPagination();
  }
  @Output() pageChange: EventEmitter<number> = new EventEmitter<number>();
  @Output() pageSizeChange: EventEmitter<number> = new EventEmitter<number>();

  constructor() {}

  ngOnInit(): void {}

  getRange = (): number[] => {
    console.log("THIIIS", this._totalPages);
    console.log("selected", this.selected);
    console.log("listSize", this.listSize);

    if (!this._totalPages) return [this.selected];

    if (this._totalPages <= this.tempListSize) {
      return Array.from({ length: this._totalPages }, (_, i) => i + 1);
    }
    if (this.selected - Math.floor(this.tempListSize / 2) <= 0) {
      return Array.from({ length: this.tempListSize }, (_, i) => i + 1);
    } else if (
      this.selected + Math.floor(this.tempListSize / 2) >
      this._totalPages
    ) {
      let start = this._totalPages - this.tempListSize;
      return Array.from({ length: this.tempListSize }, (_, i) => i + start + 1);
    }
    let start = this.selected - Math.floor(this.tempListSize / 2);
    return Array.from({ length: this.tempListSize }, (_, i) => i + start);
  };

  setPagination() {
    this.range = this.getRange();
  }

  onClick(page: number) {
    this.selected = page;
    this.setPagination();
    this.pageChange.emit(page);
  }

  onPageSizeChange(pageSize: number) {
    this.pageSize = pageSize;
    this.pageSizeChange.emit(pageSize);
  }
}
