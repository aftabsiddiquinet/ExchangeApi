using ExchangeApi.Controllers;
using ExchangeApi.Dtos;
using ExchangeApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

public class RatesControllerTests
{
    [Fact]
    public void RetrieveExchangeRates_ReturnsNotFound_WhenNoRates()
    {
        // Arrange
        var mockService = new Mock<IRateService>();
        mockService.Setup(service => service.FetchLatestRates(It.IsAny<string>()))
                   .Returns(new List<RateDto>());

        var controller = new RatesController(mockService.Object);

        // Act
        var result = controller.RetrieveExchangeRates("MEX");

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"No exchange rates available for country code: MEX.", notFoundResult.Value);
    }

    [Fact]
    public void RetrieveExchangeRates_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var mockService = new Mock<IRateService>();
        mockService.Setup(service => service.FetchLatestRates(It.IsAny<string>()))
                   .Throws(new Exception("Test exception"));

        var controller = new RatesController(mockService.Object);

        // Act
        var result = controller.RetrieveExchangeRates("MEX");

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Equal("An error occurred while processing your request.", statusCodeResult.Value);
    }
    [Fact]
    public void RetrieveExchangeRates_ReturnsExchangeRates_WhenFound()
    {
        // Arrange
        var mockService = new Mock<IRateService>();
        var sampleRates = new List<RateDto>
    {
        new RateDto
        {
            Currency = "MXN",
            Country = "MEX",
            AdjustedRate = 17.24m,
            PaymentType = "debit",
            DeliveryType = "cash"
        },
        new RateDto
        {
            Currency = "MXN",
            Country = "MEX",
            AdjustedRate = 17.31m,
            PaymentType = "cash",
            DeliveryType = "deposit"
        }
    };

        mockService.Setup(service => service.FetchLatestRates("MEX"))
                   .Returns(sampleRates);

        var controller = new RatesController(mockService.Object);

        // Act
        var result = controller.RetrieveExchangeRates("MEX");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRates = Assert.IsType<List<RateDto>>(okResult.Value);
        Assert.Equal(2, returnedRates.Count);
        Assert.Equal(sampleRates, returnedRates);
    }

}
