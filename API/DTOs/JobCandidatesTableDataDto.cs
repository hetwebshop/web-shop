using API.Helpers;
using System;

namespace API.DTOs
{
    public class JobCandidatesTableDataDto
    {
        public int UserId { get; set; }
        public int UserApplicationId { get; set; }
        public string CandidateFullName { get; set; }
        public DateTime CandidateDateOfBirth { get; set; }
        public Gender CandidateGender { get; set; }
        public string CandidateCoverLetter { get; set; }
        public string CandidateCity { get; set; }
        public string CandidateEmail { get; set; }
        public string CandidatePhoneNumber { get; set; }
        public ApplicationStatus ApplicationStatusId { get; set; }
        public string CvFilePath { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? MeetingDateTime { get; set; }
        public bool? IsOnlineMeeting { get; set; }
        public double? AIMatchingResult { get; set; }
        public double? AIMatchingExperience { get; set; }
        public double? AIMatchingSkills { get; set; }
        public double? AIMatchingEducationLevel { get; set; }
        public string AIMatchingDescription { get; set; }
        public string AIAnalysisStatus { get; set; }
        public string AIAnaylsisError { get; set; }
        public int? ConversationId { get; set; }
        public bool DidUserApplyOnPreviousCompanyJobPosts { get; set; }
        public bool AIFeatureUnlocked { get; set; }
    }
}
