namespace ExchangeApi.Dtos
{
    public class RateDto
    {
        public string Currency { get; set; }
        public string Country { get; set; }
        public decimal AdjustedRate { get; set; }
        public string PaymentType { get; set; }
        public string DeliveryType { get; set; }
    }
}