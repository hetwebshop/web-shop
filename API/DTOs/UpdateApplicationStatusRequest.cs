using System;

namespace API.DTOs
{
    public class UpdateApplicationStatusRequest
    {
        public ApplicationStatus ApplicationStatus { get; set; }
        public string Feedback { get; set; }
        public bool IsOnlineMeeting { get; set; }
        public string OnlineMeetingLink { get; set; }
        public DateTime? MeetingDateTime { get; set; }
        public DateTime? MeetingDateTimeDateType { get; set; }
        public int? UserApplicationId { get; set; }
    }
}
