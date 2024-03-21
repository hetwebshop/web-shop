import { Component } from '@angular/core';
import { UserJobPost } from 'src/app/models/userJobPost';
import { JobService } from 'src/app/services/job.service';
import { UtilityService } from 'src/app/services/utility.service';

@Component({
  selector: 'app-user-jobs',
  templateUrl: './user-jobs.component.html',
  styleUrls: ['./user-jobs.component.css']
})
export class UserJobsComponent {
  allJobs: UserJobPost[];
  constructor(private jobService: JobService, utility: UtilityService) {
    utility.setTitle('Users');
  }

  ngOnInit(): void {
    this.jobService.getAllUserJobs().subscribe((response) => {
      this.allJobs = response;
    });
  }
}
