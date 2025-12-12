namespace GreenStackAPI.Repositories
{
    using GreenStackAPI.Models;

    public interface IMixApiRepository
    {
        Task<ApiResponse> GetRawEnergyMixDataAsync(DateTime dateFrom, DateTime dateTo);
    }
}
