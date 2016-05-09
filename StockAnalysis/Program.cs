using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalysis
{
	using System.IO;

	class Program
	{
		static void Main(string[] args)
		{
			bool verbose = false;

			var marketSimulators = new List<MarketSimulation>();

			//var simulator = new MarketSimulation(30000, 0, 1.01, 15);
			//marketSimulators.Add(simulator);

			for (int iDays = 0; iDays <= 100; iDays++)
			{
				for (int iThres = 5; iThres <= 100; iThres += 5)
				{
					var simulator = new MarketSimulation(30000, 0, 1 + ((double)iThres / 10000), iDays);
					marketSimulators.Add(simulator);
				}
			}

			using (StreamReader sr = new StreamReader("csv/google1416.csv"))
			{
				// Discard file header
				sr.ReadLine();

				while (!sr.EndOfStream)
				{
					var data = sr.ReadLine().Split(new[] { ',' });
					string date = data[0];
					double openingPrice = double.Parse(data[1]);
					double highPrice = double.Parse(data[2]);
					double lowPrice = double.Parse(data[3]);
					double closePrice = double.Parse(data[4]);

					foreach (var marketSimulation in marketSimulators)
					{
						marketSimulation.NewTradingDay(date, openingPrice, highPrice, closePrice);
					}
				}
			}

			// Write summary file
			using (StreamWriter sw = new StreamWriter("output/summary.csv"))
			{
				// Header
				sw.WriteLine("MaxDaysHolding,Threshold,MaxCapital,AttemptedTrades,DoneTrades,MaxReachedTrades");
				foreach (MarketSimulation marketSimulation in marketSimulators)
				{
					sw.WriteLine($"{marketSimulation.MaxDaysOfHolding},{marketSimulation.PercentageThresholdToSell},{marketSimulation.MaxCapital},{marketSimulation.AttemptedTrades},{marketSimulation.DoneTrades},{marketSimulation.ReachedMaxTrades}");
				}
			}

			//Console.ReadKey();
		}
	}
}
