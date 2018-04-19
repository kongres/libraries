namespace Resources.Location.Models
{
    public class City
    {
        public string Name { get; set; }
        public int OrderNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Timezone { get; set; }

        public string CountryId { get; set; }
        public string StateId { get; set; }
    }
}