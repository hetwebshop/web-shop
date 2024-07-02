export interface UserJobPost {
    id: number;
    position: string;
    biography: string;
    //skills: string;
    price?: number | null;
    //tags: string;
    updatedAt?: Date;
    //userId: number;
    jobTypeId: number;
    //jobType: string;
    jobCategoryId: number;
    //jobCategory: string;
    jobPostStatusId: number;
    //jobPostStatus: string;
    //addressId: number;
    //streetName: string;
    //streetNumber: string;
    //city: string;
    //country: string;
    cityId: number;
    countryId: number;
    applicantFirstName: string;
    applicantLastName: string;
    applicantEmail: string;
    applicantGender: string;
    applicantDateOfBirth: Date;
    applicantPhoneNumber: string;
    applicantEducations: ApplicantEducation[];
    //userJobSubcategories: UserJobSubcategory[];
    advertisementTypeId: number;
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