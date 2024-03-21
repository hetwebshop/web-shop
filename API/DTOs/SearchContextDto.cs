using System.Collections.Generic;
using API.Helpers;

namespace API.DTOs
{
    public class SearchContextDto : BaseParams
    {
        public string SearchText { get; set; }
        public Dictionary<string, string> Filters { get; set; }
    }
}
