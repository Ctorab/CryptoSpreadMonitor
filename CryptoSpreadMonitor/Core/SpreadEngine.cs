using CryptoSpreadMonitor.Exchanges;

namespace CryptoSpreadMonitor.Core
{
    public class SpreadEngine
    {
        private readonly List<decimal> _spreadHistory = new();

        public event Action? OnSpreadUpdated;

        public void Update(OrderBook orderBookA, OrderBook orderBookB)
        {
            if (orderBookA.Bids.Count <= 0 || orderBookA.Asks.Count <= 0)
                return;

            if (orderBookB.Bids.Count <= 0 || orderBookB.Asks.Count <= 0)
                return;

            var bestABid = orderBookA.Bids.Keys.Max();
            var bestAAsk = orderBookA.Asks.Keys.Min();

            var bestBBid = orderBookB.Bids.Keys.Max();
            var bestBAsk = orderBookB.Asks.Keys.Min();

            var spread1 = (bestABid - bestBAsk) / bestBAsk * 100m;
            var spread2 = (bestBBid - bestAAsk) / bestAAsk * 100m;

            var spread = Math.Max(spread1, spread2);

            _spreadHistory.Add(spread);
            if (_spreadHistory.Count > 1000)
                _spreadHistory.RemoveAt(0);

            OnSpreadUpdated?.Invoke();
        }

        public IReadOnlyList<decimal> History => _spreadHistory;
    }
}
