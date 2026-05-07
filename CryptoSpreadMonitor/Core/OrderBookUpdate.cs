using CryptoSpreadMonitor.Exchanges;

namespace CryptoSpreadMonitor.Core
{
    public class OrderBookUpdate
    {
        public string ExchangeName { get; }
        public OrderBook OrderBook { get; }

        public OrderBookUpdate(string name, OrderBook ob)
        {
            ExchangeName = name;
            OrderBook = ob;
        }
    }
}
