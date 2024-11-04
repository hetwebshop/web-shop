import { Moment } from "moment";

export interface CompanyJobPost {
    id: number;
    updatedAt?: Date;
    jobTypeId: number;
    jobCategoryId: number;
    jobPostStatusId: number;
    cityId: number;
    countryId: number;
    jobDescription: string;
    adDuration: number;
    pricingPlanName: string;
    adStartDate: Moment;
    adEndDate: Moment;
    isDeleted?: boolean;
    adName: string;
    position: string;
    emailForReceivingApplications: string;
}