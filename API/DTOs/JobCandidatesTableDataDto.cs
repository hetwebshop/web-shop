using API.Helpers;
using System;

namespace API.DTOs
{
    public class JobCandidatesTableDataDto
    {
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
        public int? AIMatchingResult { get; set; }
        public int? AIMatchingExperience { get; set; }
        public int? AIMatchingSkills { get; set; }
        public int? AIMatchingEducationLevel { get; set; }
        public string AIMatchingDescription { get; set; }
        public int? ConversationId { get; set; }
    }
}
