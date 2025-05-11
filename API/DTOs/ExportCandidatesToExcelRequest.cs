using System.Collections.Generic;

namespace API.DTOs
{
    public class ExportCandidatesToExcelRequest
    {
        public List<int> UserApplicationIds { get; set; }
        public string Timezone { get; set; }    
    }
}
