using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WineClustering
{
    public class Functions
    {
        /**
         * reads wine data from csv
         *
         * return List of wines
         */
        public static List<Wine> ReadWines()
        {
            List<Wine> wines = new List<Wine>();
            using (var reader = new StreamReader(@"../../winedata.csv"))
            {
                reader.ReadLine();
                while (reader.Peek() != -1)
                {
                    string line = reader.ReadLine();
                    string[] values = null;
                    if (line != null)
                    {
                        values = line.Split(',');
                    }

                    if (values != null)
                    {
                        wines.Add(new Wine(values));
                    }
                }
            }

            return wines;
        }
        
        /**
         * reads the transactions
         *
         * return list of transactions
         */
        public static List<Transaction> ReadTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();
            using (var reader = new StreamReader(@"../../transactions.csv"))
            {
                reader.ReadLine();
                while (reader.Peek() != -1)
                {
                    string line = reader.ReadLine();
                    string[] values = null;
                    if (line != null)
                    {
                        values = line.Split(',');
                    }

                    if (values != null)
                    {
                        transactions.Add(new Transaction(values));
                    }
                }
            }

            return transactions;
        }

        /**
         * Select distinc all the names from the transactions
         *
         * return list of names
         */
        private static List<string> GetNames(List<Transaction> transactions)
        {
            List<string> names = new List<string>();
            foreach (Transaction transaction in transactions)
            {
                if (!names.Contains(transaction.getName()))
                {
                    names.Add(transaction.getName());
                }
            }

            return names;
        }

        /**
         * retreives the List<WineChoice> from the List<Tuple>
         *
         * returns a List of a List of wineChoices
         */
        public static List<List<WineChoice>> mapTuple(List<Tuple<string, List<WineChoice>>> wineChoices)
        {
            List<List<WineChoice>> result = new List<List<WineChoice>>();
            foreach (var wineChoice in wineChoices)
            {
                result.Add(wineChoice.Item2);
            }

            return result;
        }

        /**
         * takes list of wines and list of transactions and combines them into the data set needed for the algorithm
         *
         * return List of Tuple of buyer name and List of Winechoices
         */
        public static List<Tuple<string, List<WineChoice>>> GetWineChoices(List<Wine> wines, List<Transaction> transactions)
        {
            List<string> names = GetNames(transactions);
            
            List<Tuple<string, List<WineChoice>>> result = new List<Tuple<string, List<WineChoice>>>();

            foreach (string name in names)
            {
                Tuple<string, List<WineChoice>> tempList = new Tuple<string, List<WineChoice>>(name, new List<WineChoice>());
                foreach (var wine in wines)
                {
                    Transaction t = transactions.Find(transaction => transaction.getName() == name && transaction.getTransactionId() == wine.getOfferId());
                    
                    tempList.Item2.Add(t == null
                        ? new WineChoice(wine.getOfferId(), 0.0)
                        : new WineChoice(wine.getOfferId(), 1.0));
                }
                result.Add(tempList);
            }
            
            return result;
        }

        /**
         * Takes two lists and combines them into a List of Tuple
         *
         * returns Tuple with as Item1 items from list1 and as Item2 items from list2
         */
        public static List<Tuple<A, B>> zip<A, B>(List<A> list1, List<B> list2)
        {
            List<Tuple<A,B>> zipped = new List<Tuple<A, B>>();

            for (int i = 0; i < list1.Count; i++)
            {
                zipped.Add(new Tuple<A, B>(list1[i], list2[i]));
            }

            return zipped;
        }
        
        /**
         * KMeans algorithm
         *
         * returns the final centroids
         */
        public static List<T> KMeans<T>(
            int k,
            int iterations,
            List<T> dataset,
            Func<int, List<T>, List<T>> initialize,
            Func<T,T, double> calculateDistance,
            Func<T, List<T>, T> calculateMeanCentroid
            )
        {
            //value that stores the centroid with it's cluster
            List<Tuple<T, List<T>>> centroidsMap = new List<Tuple<T, List<T>>>();
            //value that stores the centroids
            List<T> centroids = initialize(k, dataset);
            //value that stores the previous centroids
            List<T> prevCentroids = new List<T>();

            int i = 0;
            while (i < iterations && centroids != prevCentroids)
            {
                i++;
                //sets the prevCentroids to the current centroids
                prevCentroids = centroids;
                
                //Makes sure that the centroids with the clusters is empty again.
                centroidsMap.Clear();
                //creates the new list of clusters for the current centroids
                foreach (var centroid in centroids)
                {
                    centroidsMap.Add(new Tuple<T, List<T>>(centroid, new List<T>()));
                }

                //checks for each data row to which centroid it's the closest
                foreach (T data in dataset)
                {
                    //gets the clostest centroid
                    
                    T nearest = centroidsMap.OrderBy(it => calculateDistance(it.Item1, data)).First().Item1;
                    
                    centroidsMap.ForEach(it =>
                    {
                        //check if the nearest centroid is equal tot the centroid in the cluster list and if so add it to that cluster
                        if (Equals(it.Item1, nearest))
                        {
                            it.Item2.Add(data);
                        }
                    });
                }
                
                //value that will store the new centroids
                List<T> newCentroids = new List<T>();
                centroidsMap.ForEach(centroid =>
                {
                    // calculates the new centroid for the next iteration
                    newCentroids.Add(calculateMeanCentroid(centroid.Item1, centroid.Item2));
                });
                
                //sets the centroids to the newly generated centroids
                centroids = newCentroids;
            }

            return centroids;
        }
    }
}