import { Moment } from "moment";

export interface UserJobPost {
    id: number;
    position: string;
    biography: string;
    price?: number | null;
    updatedAt?: Date;
    jobTypeId: number;
    jobCategoryId: number;
    jobPostStatusId: number;
    cityId: number;
    countryId: number;
    applicantFirstName: string;
    applicantLastName: string;
    applicantEmail: string;
    applicantGender: string;
    applicantDateOfBirth: Date;
    applicantPhoneNumber: string;
    applicantEducations: ApplicantEducation[];
    advertisementTypeId: number;
    cvFile?: File;
    cvFilePath?: string;
    adDuration: number;
    adStartDate: Moment;
    adEndDate: Moment;
    isDeleted?: boolean;
    adTitle: string;
    adAdditionalDescription: string;
    currentUserCredits?: number;
}

export interface AdvertisementType {
    id: number;
    name: string;
}

export interface UserJobSubcategory {
    //userJobPostId: number;
    jobCategoryId: number;
}

export interface JobType {
    id: number;
    name: string;
}

export interface JobCategory {
    id: number;
    name: string;
    parentId?: number;
    subcategories?: JobCategory[];
}

export interface ApplicantEducation {
    degree: string;
    university: string;
    institutionName: string;
    fieldOfStudy: string;
    educationStartYear: number;
    educationEndYear: number;
}

export interface UserEducation extends ApplicantEducation {}