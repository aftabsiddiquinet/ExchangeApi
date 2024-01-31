namespace ExchangeApi.Dtos
{
    public class RateData
    {
        public List<RateDetail> Details { get; set; }
    }

    public class RateDetail
    {
        public string Currency { get; set; }
        public string PaymentType { get; set; }
        public string DeliveryType { get; set; }
        public decimal Rate { get; set; }
        public DateTime DateAcquired { get; set; }
    }
}