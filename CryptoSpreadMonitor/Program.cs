using CryptoSpreadMonitor.Core;
using CryptoSpreadMonitor.Exchanges;
using CryptoSpreadMonitor.Rendering;
using System.Drawing;

namespace CryptoSpreadMonitor;

internal class Program
{
    private static MarketState _marketState;
    private static SpreadEngine _spreadEngine;

    private static async Task Main()
    {
        InitConsole();

        var binanceClient = new BinanceClient();
        var hyperClient = new HyperliquidClient();

        var ctSource = new CancellationTokenSource();

        var binanceSubscribeTask = binanceClient.StartParsingOrderBook("SOLUSDT", ctSource.Token);
        var hyperSubscribeTask = hyperClient.StartParsingOrderBook("SOL", ctSource.Token);

        _marketState = new MarketState();
        _spreadEngine = new SpreadEngine();
        binanceClient.OnOrderBookUpdated += ob =>
        {
            _marketState.ExchangeA = ob;

            if (_marketState.ExchangeB != null)
                _spreadEngine.Update(_marketState.ExchangeA.OrderBook, _marketState.ExchangeB.OrderBook);
        };

        hyperClient.OnOrderBookUpdated += ob =>
        {
            _marketState.ExchangeB = ob;

            if (_marketState.ExchangeA != null)
                _spreadEngine.Update(_marketState.ExchangeA.OrderBook, _marketState.ExchangeB.OrderBook);
        };

        _spreadEngine.OnSpreadUpdated += OnSpreadUpdatedHandler;

        await Task.WhenAll(binanceSubscribeTask, hyperSubscribeTask);

        Console.ReadLine();
    }

    private static void OnSpreadUpdatedHandler()
    {
        var bitmap = ConsoleOrderbook.RenderContent(g =>
        {
            ConsoleOrderbook.DrawText(g, 0, 0, _marketState.ExchangeA.ExchangeName, true);
            if (_marketState.ExchangeA.OrderBook.Bids.Count <= 0
                || _marketState.ExchangeA.OrderBook.Asks.Count <= 0
                || _marketState.ExchangeB.OrderBook.Bids.Count <= 0
                || _marketState.ExchangeB.OrderBook.Asks.Count <= 0
                )
                return;

            var bestBidA = _marketState.ExchangeA.OrderBook.Bids.Keys.Max();
            var bestAskA = _marketState.ExchangeA.OrderBook.Asks.Keys.Min();
            var spreadA = ((bestAskA - bestBidA) / bestBidA) * 100;

            ConsoleOrderbook.DrawText(g, 0, 15, $"Ask: {bestAskA}");
            ConsoleOrderbook.DrawText(g, 0, 30, $"Spread: {spreadA.ToString("0.00")}%");
            ConsoleOrderbook.DrawText(g, 0, 45, $"Bid: {bestBidA}");

            var bestBidB = _marketState.ExchangeB.OrderBook.Bids.Keys.Max();
            var bestAskB = _marketState.ExchangeB.OrderBook.Asks.Keys.Min();
            var spreadB = ((bestAskB - bestBidB) / bestBidB) * 100;

            ConsoleOrderbook.DrawText(g, 0, 70, _marketState.ExchangeB.ExchangeName, true);
            ConsoleOrderbook.DrawText(g, 0, 85, $"Ask: {bestAskB}");
            ConsoleOrderbook.DrawText(g, 0, 100, $"Spread: {spreadB.ToString("0.00")}%");
            ConsoleOrderbook.DrawText(g, 0, 115, $"Bid: {bestBidB}");
        }, 150, 500);

        ConsoleImageRenderer.RenderImage(bitmap, new Size(30, 40), 700, 0);
        ConsoleGraph.Draw(_spreadEngine.History);
    }


    private static void InitConsole()
    {
        Console.CursorVisible = false;
        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
        Console.Title = "Spread Monitor";
    }
}