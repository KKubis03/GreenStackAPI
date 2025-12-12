namespace GreenStackAPI.Models
{
    public class DailyMix
    {
        public DateTime Date { get; set; }

        public List<GenerationMixItem> AverageMix { get; set; }

        public double CleanEnergyPercentage { get; set; }
    }
}
