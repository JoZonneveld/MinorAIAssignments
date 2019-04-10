using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace WineClustering
{
    internal class Program
    {
        private static Random rnd = new Random();
        public static void Main(string[] args)
        {
            List<Wine> wines = Functions.ReadWines();
            List<Transaction> transactions = Functions.ReadTransactions();
            List<Tuple<string, List<WineChoice>>> wineChoicesName = Functions.GetWineChoices(wines, transactions);
            List<List<WineChoice>> wineChoices = Functions.mapTuple(wineChoicesName);

            Func<int, List<List<WineChoice>>, List<List<WineChoice>>> initialize = (k, dataset) =>
            {
                List<List<WineChoice>> result = new List<List<WineChoice>>();
                
                int i = 0;
                while (i < k)
                {
                    List<WineChoice> centroid = new List<WineChoice>();
                    foreach (var data in dataset.First())
                    {
                        centroid.Add(new WineChoice(data.offerId, rnd.NextDouble()));
                    }
                    result.Add(centroid);
                    i++;
                }

                return result;
            };

            Func<List<WineChoice>, List<WineChoice>, double> calculateDistance = (centroid, data) =>
            {
                List<double> centroidValues = centroid.Select(it => it.choice).ToList();
                List<double> dataValues = data.Select(it => it.choice).ToList();

                List<Tuple<double, double>> zipped = Functions.zip(centroidValues, dataValues);
                
                return Math.Sqrt(zipped.Select(item => Math.Pow(item.Item1 - item.Item2, 2)).Sum());
            };

            List<List<WineChoice>> t = initialize(5, wineChoices);
            var c = calculateDistance(t[0], t[1]);

            Functions.KMeans(5, 10000, wineChoices, initialize, calculateDistance);
            Console.WriteLine();

        }
    }
}