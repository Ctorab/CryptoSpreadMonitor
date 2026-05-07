using Binance.Net.Clients;
using Binance.Net.Interfaces;
using CryptoExchange.Net.Objects.Sockets;
using CryptoSpreadMonitor.Core;
using CryptoSpreadMonitor.Exchanges;

internal class BinanceClient : IExchangeParserFeed
{
    public string Name => "Binance";
    private readonly BinanceSocketClient _socketClient = new();

    private readonly Dictionary<decimal, decimal> _localAsks = new();
    private readonly Dictionary<decimal, decimal> _localBids = new();

    public event Action<OrderBookUpdate>? OnOrderBookUpdated;

    public async Task StartParsingOrderBook(string symbol, CancellationToken ct)
    {
        var sub = await _socketClient.UsdFuturesApi.ExchangeData
            .SubscribeToOrderBookUpdatesAsync(symbol, 100, OnOrderBookUpdateReceived, ct);

        if (!sub.Success)
            throw new Exception(sub.Error?.Message);
    }

    private void OnOrderBookUpdateReceived(DataEvent<IBinanceFuturesEventOrderBook> @event)
    {
        var data = @event.Data;

        foreach (var bid in data.Bids)
            if (bid.Quantity == 0)
                _localBids.Remove(bid.Price);
            else
                _localBids[bid.Price] = bid.Quantity;

        foreach (var ask in data.Asks)
            if (ask.Quantity == 0)
                _localAsks.Remove(ask.Price);
            else
                _localAsks[ask.Price] = ask.Quantity;

        var currentAsks = new Dictionary<decimal, decimal>(_localAsks);
        var currentBids = new Dictionary<decimal, decimal>(_localBids);

        var orderBook = new OrderBook(currentAsks, currentBids);
        OnOrderBookUpdated?.Invoke(new(Name, orderBook));
    }
}