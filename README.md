# CryptoSpreadMonitor

A cryptocurrency spread arbitrage tool designed to monitor and visualize price differences between exchanges in real-time.

## 🚀 Overview
This application connects to multiple exchange WebSockets (Binance & Hyperliquid) to fetch live order book data. It calculates the bid-ask spread and cross-exchange delta, rendering a live graphical representation directly in the Windows Console.

## 🛠 Technical Architecture
* **Core Logic:** Handles state management and the calculation engine for spreads.
* **Exchanges Layer:** Event-driven WebSocket clients using `IExchangeParserFeed` for standardized data fetching.
* **Rendering Engine:** A custom UI layer that converts data into graphical bitmaps and signal plots (ScottPlot) for the CLI.

## ✨ Key Features
* **Real-Time Data:** Low-latency WebSocket integration for all avaliable crypto pairs.
* **CLI Graphics:** Real-time signal graphs and order book visualization inside the terminal.
* **Resolution Scaling:** Uses normalized coordinate systems for consistent rendering.
* **Asynchronous Pipeline:** Fully non-blocking `Task`-based architecture.
