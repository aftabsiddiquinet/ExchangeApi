using ExchangeApi.Dtos;
using ExchangeApi.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExchangeApi.Services
{
    public class RateService : IRateService
    {
        private readonly string _ratesFilePath;
        private readonly IDictionary<string, decimal> _baseRates;
        private readonly IDictionary<string, string> _currencyCountryMap;

        public RateService(IConfiguration configuration)
        {
            _ratesFilePath = configuration.GetValue<string>("RatesFilePath");

            _baseRates = new Dictionary<string, decimal>
            {
                ["MEX"] = 0.024m,
                ["PHL"] = 2.437m,
                ["GTM"] = 0.056m,
                ["IND"] = 3.213m
            };

            _currencyCountryMap = new Dictionary<string, string>
            {
                ["MXN"] = "MEX",
                ["PHP"] = "PHL",
                ["GTQ"] = "GTM",
                ["INR"] = "IND"
            };
        }

        public IEnumerable<RateDto> FetchLatestRates(string country)
        {
            var fileData = File.ReadAllText(_ratesFilePath);
            var ratesData = JsonConvert.DeserializeObject<RateData>(fileData);

            var recentRates = ratesData.Details
                .Where(detail => _currencyCountryMap[detail.Currency] == country)
                .GroupBy(detail => new { detail.PaymentType, detail.DeliveryType })
                .Select(group => group.OrderByDescending(detail => detail.DateAcquired).First())
                .Select(detail => new RateDto
                {
                    Currency = detail.Currency,
                    Country = _currencyCountryMap[detail.Currency],
                    AdjustedRate = AdjustRate(detail.Rate, detail.Currency),
                    PaymentType = detail.PaymentType,
                    DeliveryType = detail.DeliveryType
                });

            return recentRates;
        }

        private decimal AdjustRate(decimal rate, string currency)
        {
            if (_baseRates.TryGetValue(currency, out var baseRate))
            {
                return Math.Round(rate + baseRate, 2, MidpointRounding.AwayFromZero);
            }

            throw new KeyNotFoundException($"Base rate not found for currency: {currency}");
        }
    }
}
