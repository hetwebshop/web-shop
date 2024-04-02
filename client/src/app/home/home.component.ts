import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { UtilityService } from '../services/utility.service';
import { JobService } from '../services/job.service';
import { UserJobPost } from '../models/userJobPost';
import { AdvertisementTypeEnum } from '../models/enums';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  allJobs: UserJobPost[];
  constructor(private jobService: JobService, utility: UtilityService) {
    utility.setTitle('Home');
  }

  ngOnInit(): void {
    console.log("from home");
    this.jobService.getAds().subscribe((response) => {
      this.allJobs = response.items;
    });
  }
}
