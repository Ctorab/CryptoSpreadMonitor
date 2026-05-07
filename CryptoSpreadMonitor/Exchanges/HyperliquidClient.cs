using CryptoExchange.Net.Objects.Sockets;
using CryptoSpreadMonitor.Core;
using HyperLiquid.Net.Clients;
using HyperLiquid.Net.Objects.Models;

namespace CryptoSpreadMonitor.Exchanges
{
    internal class HyperliquidClient : IExchangeParserFeed
    {
        public string Name => "Hyperliquid";

        private HyperLiquidSocketClient _hyperClient = new();

        public event Action<OrderBookUpdate>? OnOrderBookUpdated;

        public async Task StartParsingOrderBook(string symbol, CancellationToken ct)
        {
            var sub = await _hyperClient.FuturesApi.ExchangeData.
                SubscribeToOrderBookUpdatesAsync(symbol, OnOrderBookUpdateReceived, ct: ct);

            if(!sub.Success)
                throw new Exception(sub.Error?.Message);
        }
        private void OnOrderBookUpdateReceived(DataEvent<HyperLiquidOrderBook> @event)
        {
            var asks = new Dictionary<decimal, decimal>();
            var bids = new Dictionary<decimal, decimal>();
            var eventOrderBook = @event.Data;
            foreach(var ask in eventOrderBook.Levels.Asks)
                asks[ask.Price] = ask.Quantity;

            foreach(var bid in  eventOrderBook.Levels.Bids)
                bids[bid.Price] = bid.Quantity;

            var orderBook = new OrderBook(asks, bids);
            OnOrderBookUpdated?.Invoke(new(Name, orderBook));
        }
    }
}
