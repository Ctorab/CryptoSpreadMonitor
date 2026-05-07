using CryptoSpreadMonitor.Core;

namespace CryptoSpreadMonitor.Exchanges
{
    public record OrderBook(Dictionary<decimal, decimal> Asks, Dictionary<decimal, decimal> Bids);

    internal interface IExchangeParserFeed
    {
        public string Name { get; }

        event Action<OrderBookUpdate> OnOrderBookUpdated;
        public Task StartParsingOrderBook(string ticker, CancellationToken ct);
    }
}
