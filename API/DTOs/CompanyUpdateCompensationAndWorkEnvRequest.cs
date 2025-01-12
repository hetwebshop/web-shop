namespace API.DTOs
{
    public class CompanyUpdateCompensationAndWorkEnvRequest
    {
        public int[] SalaryRange { get; set; }
        public string Benefits { get; set; }
        public string WorkEnvironmentDescription { get; set; }
    }
}
