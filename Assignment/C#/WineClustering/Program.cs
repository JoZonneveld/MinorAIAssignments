using System;
using System.Collections.Generic;
using System.Linq;

namespace WineClustering
{
    internal class Program
    {
        private static Random rnd = new Random();
        public static void Main(string[] args)
        {
            //Read and convert all data to the right sets
            List<Wine> wines = Functions.ReadWines();
            List<Transaction> transactions = Functions.ReadTransactions();
            List<Tuple<string, List<WineChoice>>> wineChoicesName = Functions.GetWineChoices(wines, transactions);
            List<List<WineChoice>> wineChoices = Functions.mapTuple(wineChoicesName);

            /**
             *  Creates new random centroids
             *  @param k = int for how much clusters
             *  @param dataset = Dataset to create new random Centroids for the amount of k
             *
             * returns list of random centroids
             */
            Func<int, List<List<WineChoice>>, List<List<WineChoice>>> initialize = (k, dataset) =>
            {
                List<List<WineChoice>> result = new List<List<WineChoice>>();
                
                int i = 0;
                while (i < k)
                {
                    List<WineChoice> centroid = new List<WineChoice>();
                    //creates the new centroid with a List<WineChoice> but with random choice value
                    foreach (var data in dataset.First())
                    {
                        centroid.Add(new WineChoice(data.offerId, rnd.NextDouble()));
                    }
                    result.Add(centroid);
                    i++;
                }

                return result;
            };

            /**
             * Calculate the distance between the centroid and the data
             *
             * @param centroid = centroid of the cluster
             * @param data = checks how far the data is from the centroid
             *
             * return distance between data and centroid
             */
            Func<List<WineChoice>, List<WineChoice>, double> calculateDistance = (centroid, data) =>
            {
                //get the choice values of the centroid and the data
                List<double> centroidValues = centroid.Select(it => it.choice).ToList();
                List<double> dataValues = data.Select(it => it.choice).ToList();
                
                //creates a Tuple with the centroidValue as item1 and dataValue as item2
                List<Tuple<double, double>> zipped = Functions.zip(centroidValues, dataValues);
                
                //Calculates the distance between the the centroid and the data
                return Math.Sqrt(zipped.Select(item => Math.Pow(item.Item1 - item.Item2, 2)).Sum());
            };

            /**
             * Calculate the mean in the cluster and make that the new centroid
             *
             * @param centroid = current centroid
             * @param cluster = cluster from the current centroid
             *
             * return new centroid
             */
            Func<List<WineChoice>, List<List<WineChoice>>, List<WineChoice>> calculateMeanCentroid =
                (centroid, cluster) =>
                {
                    List<WineChoice> result = new List<WineChoice>();
                    
                    //Loop thru 32 offers of wines
                    for (int i = 0; i < centroid.Count; i++)
                    {
                        double sum = 0.0;
                        //Use the cluster to calculate the mean of the offer
                        for (int j = 0; j < cluster.Count; j++)
                        {
                            sum += cluster[j][i].choice;
                        }
                        //check if the choice is not NaN if so make it 0.0
                        double choice = double.IsNaN(sum / cluster.Count) ? 0.0 : sum / cluster.Count;
                        //add new mean of the custer as an object of the centroid
                        result.Add(new WineChoice(centroid[i].offerId, choice));
                    }

                    return result;
                };

            var res = Functions.KMeans(5, 10000, wineChoices, initialize, calculateDistance, calculateMeanCentroid);

            
            //generates output for stackedit.io for easy read
            for (int i = 0; i < res.Count; i++)
            {
                int clusterCount = i + 1;
                Console.WriteLine("# Cluster : " + clusterCount);
                Console.WriteLine("|Offer #|Choice|Wine|");
                Console.WriteLine("|:-|:-|:-|");

                var ordered = res[i].OrderByDescending(r => r.choice).ToList();            
                
                ordered.ForEach(it =>
                {
                    Wine wineName = wines.Find(wine => wine.getOfferId() == it.offerId);

                    int amount = Convert.ToInt32(wines.Count * it.choice);
                    
                    Console.WriteLine("|" + it.offerId + "|" + amount + "|" + wineName.getVarietal() + "|");
                });

            }
        }
    }
}