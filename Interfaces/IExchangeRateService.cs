using ExchangeApi.Dtos;

namespace ExchangeApi.Interfaces
{
    public interface IRateService
    {
        IEnumerable<RateDto> FetchLatestRates(string country);
    }
}
