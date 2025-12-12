namespace GreenStackAPI.Models
{
    public class GenerationMixData
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public List<GenerationMixItem> GenerationMix { get; set; }
    }
}
