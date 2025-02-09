namespace API.DTOs
{
    public class CompanyUpdateCompensationAndWorkEnvRequest
    {
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public string Benefits { get; set; }
        public string WorkEnvironmentDescription { get; set; }
    }
}
