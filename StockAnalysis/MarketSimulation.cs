using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis
{
	using System.IO;
	using System.Threading;

	class MarketSimulation
	{
		public double Capital { get; set; }
		public int Stock { get; set; }
		public double PercentageThresholdToSell { get; set; }
		public int MaxDaysOfHolding { get; set; }
		public double DaysCounter { get; set; }
		public double LastBuyPrice { get; set; }
		public double MaxCapital { get; set; }
		public int AttemptedTrades { get; set; }
		public int DoneTrades { get; set; }
		public int ReachedMaxTrades { get; set; }

		StreamWriter sw;

		public MarketSimulation() { }

		public MarketSimulation(double capital, int stock, double percentageToSell, int maxDaysHolding = 0)
		{
			Capital = capital;
			Stock = stock;
			PercentageThresholdToSell = percentageToSell;
			LastBuyPrice = 0;
			DaysCounter = 0;
			MaxCapital = 0;
			AttemptedTrades = 0;
			DoneTrades = 0;
			ReachedMaxTrades = 0;

			if (maxDaysHolding == 0) MaxDaysOfHolding = 999999;
			else MaxDaysOfHolding = maxDaysHolding;

			sw = new StreamWriter($"output/{percentageToSell}_{maxDaysHolding}.csv");
			sw.WriteLine($"date,openingCapital,openingStock,Capital,Stock,LastBuyPrice,openingStockPrice,highStockPrice,closingStockPrice,highStockPrice/LastBuyPrice,DaysCounter");
		}

		public void NewTradingDay(string date, double openingStockPrice, double highStockPrice, double closingStockPrice)
		{
			double openingCapital = Capital;
			double openingStock = Stock;

			// if we don't have any stock, we buy some first
			if (Stock <= 0)
			{
				BuyStock(openingStockPrice);
			}

			// we try to sell our stocks at the established margin
			if ((highStockPrice / LastBuyPrice) >= PercentageThresholdToSell)
			{
				SellStock(LastBuyPrice * PercentageThresholdToSell);
				DoneTrades++;
			}
			// we sell at closing price if we reached max days of holding
			else if (DaysCounter >= MaxDaysOfHolding)
			{
				SellStock(closingStockPrice);
				ReachedMaxTrades++;
			}
			// we did not sell today, we increase the counter
			else
			{
				DaysCounter++;
			}

			AttemptedTrades++;

			// write to file
			sw.WriteLine($"{date},{openingCapital},{openingStock},{Capital},{Stock},{LastBuyPrice},{openingStockPrice},{highStockPrice},{closingStockPrice},{highStockPrice / LastBuyPrice},{DaysCounter}");
		}

		public void BuyStock(double stockPrice)
		{
			LastBuyPrice = stockPrice;
			//Capital -= 10;
			Stock = (int)Math.Floor(Capital / stockPrice);
			Capital -= Stock * stockPrice;
		}

		public void SellStock(double stockPrice)
		{
			Capital += Stock * stockPrice;
			//Capital -= 10;
			Stock = 0;
			DaysCounter = 0;

			if (Capital > MaxCapital) MaxCapital = Capital;
		}

		public override string ToString()
		{
			return $"[MaxCapital: {MaxCapital}; Capital: {Capital}; Stock: {Stock}; DaysCounter: {DaysCounter}; LastBuy: {LastBuyPrice}; Threshold: {PercentageThresholdToSell}; MaxHolding: {MaxDaysOfHolding}]";
        }
	}
}
