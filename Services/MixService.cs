namespace GreenStackAPI.Services
{
    using GreenStackAPI.Models;
    using GreenStackAPI.Repositories;

    public class MixService
    {
        // Repository for accessing the Mix API

        private readonly IMixApiRepository _mixApiRepository;

        // Clean energy sources

        private static readonly HashSet<string> _cleanSources =
            new(StringComparer.OrdinalIgnoreCase)
            { "wind", "solar", "hydro", "nuclear", "biomass" };

        // Constants

        private const int DaysForDayAverage = 3;

        private const int DaysForOptimalChargingWindow = 2;

        // Constructor

        public MixService(IMixApiRepository mixApiRepository)
        {
            _mixApiRepository = mixApiRepository;
        }

        // Calculates the average generation mix for the next three days

        public async Task<List<DailyMix>> GetThreeDaysAveragesAsync()
        {
            var result = new List<DailyMix>();

            var from = DateTime.Today;
            var to = DateTime.Today.AddDays(3);

            var allIntervals = await GetAllIntervals(from, to);

            for (int i = 0; i < DaysForDayAverage; i++)
            {
                var currentDate = DateTime.Today.AddDays(i);

                var intervals = FilterIntervalsByDate(allIntervals, currentDate);

                if (!intervals.Any())
                    continue;

                var avgMix = intervals
                    .SelectMany(x => x.GenerationMix)
                    .GroupBy(mix => mix.Fuel)
                    .Select(g => new GenerationMixItem
                    {
                        Fuel = g.Key,
                        Perc = Math.Round(g.Average(x => x.Perc), 2)
                    }).ToList();

                var cleanEnergy = Math.Round(
                    avgMix.Where(m => _cleanSources.Contains(m.Fuel)).Sum(m => m.Perc),
                    2);

                result.Add(new DailyMix
                {
                    Date = currentDate,
                    AverageMix = avgMix,
                    CleanEnergyPercentage = cleanEnergy
                });
            }

            return result.OrderBy(x => x.Date).ToList();
        }

        // Finds the optimal charging window with the highest average clean energy percentage

        public async Task<OptimalChargingWindow> GetOptimalChargingWindowAsync(int windowHours)
        {
            if (windowHours < 1 || windowHours > 6)
                throw new ArgumentOutOfRangeException(nameof(windowHours),
                    "Window must be between 1 and 6 hours.");

            var today = DateTime.Today;
            var allIntervals = await GetAllIntervals(today, today.AddDays(DaysForOptimalChargingWindow));

            allIntervals = allIntervals.OrderBy(x => x.From).ToList();

            int intervalsInWindow = windowHours * 2;
            double maxAvg = double.MinValue;
            int bestStartIdx = -1;

            for (int i = 0; i <= allIntervals.Count - intervalsInWindow; i++)
            {
                var window = allIntervals.Skip(i).Take(intervalsInWindow);
                var avg = CalculateCleanEnergyAverage(window);

                if (avg > maxAvg)
                {
                    maxAvg = avg;
                    bestStartIdx = i;
                }
            }

            if (bestStartIdx == -1)
                return null;

            var bestWindow = allIntervals.Skip(bestStartIdx).Take(intervalsInWindow).ToList();

            return new OptimalChargingWindow
            {
                Start = bestWindow.First().From,
                End = bestWindow.Last().To,
                AverageCleanEnergyPercentage = Math.Round(maxAvg, 2)
            };
        }

        // Helper methods

        private async Task<List<GenerationMixData>> GetAllIntervals(DateTime from, DateTime to)
        {
            var apiResponse = await _mixApiRepository.GetRawEnergyMixDataAsync(from, to);
            return apiResponse.Data?.ToList() ?? new List<GenerationMixData>();
        }

        private static List<GenerationMixData> FilterIntervalsByDate(
            List<GenerationMixData> intervals, DateTime date)
        {
            return intervals.Where(x => x.From.Date == date.Date).ToList();
        }

        private static double CalculateCleanEnergyAverage(IEnumerable<GenerationMixData> window)
        {
            return window.Select(interval =>
                interval.GenerationMix
                    .Where(mix => _cleanSources.Contains(mix.Fuel))
                    .Sum(mix => mix.Perc)
                ).Average();
        }
    }
}
