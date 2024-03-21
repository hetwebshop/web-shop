import { Component } from '@angular/core';
import { UserJobPost } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-my-ads',
  templateUrl: './my-ads.component.html',
  styleUrls: ['./my-ads.component.css']
})
export class MyAdsComponent {
  allJobs: UserJobPost[];
  constructor(private jobService: JobService, utility: UtilityService) {
    utility.setTitle('My ads');
  }

  ngOnInit(): void {
    this.jobService.getMyAds().subscribe((response) => {
      this.allJobs = response;
    });
  }
}
