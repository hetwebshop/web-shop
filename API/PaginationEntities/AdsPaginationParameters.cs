using API.Helpers;
using System;
using System.Collections.Generic;

namespace API.PaginationEntities
{
    public class AdsPaginationParameters : PaginationParameters
    {
        public AdsPaginationParameters()
        {
            OrderBy = "CreatedAt";
        }
        public string searchKeyword { get; set; }
        public List<int> cityIds { get; set; }
        public List<int> jobCategoryIds { get; set; }
        public List<int> jobTypeIds { get; set; }
        public int? advertisementTypeId { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public int? UserId { get; set; }
        public int? CompanyId { get; set; }
        public JobPostStatus? adStatus { get; set; }
    }
}
