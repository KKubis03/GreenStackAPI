namespace GreenStackAPI.Models
{
    public class OptimalChargingWindow
    {
        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public double AverageCleanEnergyPercentage { get; set; }
    }
}
