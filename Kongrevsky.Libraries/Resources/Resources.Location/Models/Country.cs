namespace Kongrevsky.Resources.Location.Models
{
    public class Country
    {
        #region Properties

        public string Name { get; set; }

        public string Code { get; set; }

        public bool HasStates { get; set; }

        public bool HasCities { get; set; }

        #endregion
    }
}