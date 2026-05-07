namespace CryptoSpreadMonitor.Core
{
    public class MarketState
    {
        public OrderBookUpdate? ExchangeA { get; set; }
        public OrderBookUpdate? ExchangeB { get; set; }
    }
}
