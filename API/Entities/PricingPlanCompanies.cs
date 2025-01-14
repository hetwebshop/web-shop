namespace API.Entities
{
    public class PricingPlanCompanies
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AdActiveDays { get; set; }
        public int PriceInCredits { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public int Priority { get; set; }
    }
}
