using Microsoft.AspNetCore.Mvc;
using ExchangeApi.Dtos;
using ExchangeApi.Interfaces;

namespace ExchangeApi.Controllers
{
    [ApiController]
    [Route("api/rates")]
    public class RatesController : ControllerBase
    {
        private readonly IRateService _rateService;

        public RatesController(IRateService rateService)
        {
            _rateService = rateService ?? throw new ArgumentNullException(nameof(rateService));
        }

        [HttpGet("exchange")]
        public IActionResult RetrieveExchangeRates([FromQuery] string country)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(country))
                {
                    return BadRequest("Country code must be provided.");
                }

                var rates = _rateService.FetchLatestRates(country.ToUpper());

                if (!rates.Any())
                {
                    return NotFound($"No exchange rates available for country code: {country}.");
                }

                return Ok(rates);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }

        }
    }
}
