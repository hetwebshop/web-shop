import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CompanyMyAdManagerComponent } from './company-my-ad-manager.component';

describe('CompanyMyAdManagerComponent', () => {
  let component: CompanyMyAdManagerComponent;
  let fixture: ComponentFixture<CompanyMyAdManagerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CompanyMyAdManagerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CompanyMyAdManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
