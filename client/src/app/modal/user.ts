import { UserEducation } from "../models/userJobPost";

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  cityId: number;
  userName: string;
  gender: string;
  token: string;
  photoUrl: string;
  roles: string[];
  email: string;
  phoneNumber: string;
  jobTypeId: number;
  jobCategoryId: number;
  dateOfBirth: Date;
  userEducations: UserEducation[];
}

export interface UserInfo {
  id: number;
  name: string;
  userName: string;
  photoUrl: string;
  exist: boolean;
}

export interface UserProfile {
  id: number;
  firstName: string;
  lastName: string;
  cityId: number;
  userName: string;
  gender: string;
  token: string;
  photoUrl: string;
  roles: string[];
  email: string;
  phoneNumber: string;
  jobTypeId: number;
  jobCategoryId: number;
  dateOfBirth: Date;
  position: string;
  biography: string;
  userEducations: UserEducation[];
}
