using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WineClustering
{
    public class Functions
    {
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

        public static List<List<WineChoice>> mapTuple(List<Tuple<string, List<WineChoice>>> wineChoices)
        {
            List<List<WineChoice>> result = new List<List<WineChoice>>();
            foreach (var wineChoice in wineChoices)
            {
                result.Add(wineChoice.Item2);
            }

            return result;
        }

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

        public static List<Tuple<A, B>> zip<A, B>(List<A> list1, List<B> list2)
        {
            List<Tuple<A,B>> zipped = new List<Tuple<A, B>>();

            for (int i = 0; i < list1.Count; i++)
            {
                zipped.Add(new Tuple<A, B>(list1[i], list2[i]));
            }

            return zipped;
        }
        
        public static List<T> KMeans<T>(
            int k,
            int iterations,
            List<T> dataset,
            Func<int, List<T>, List<T>> initialize,
            Func<T,T, double> calculateDistance
//            Func<T, List<T>, T> calculateMeanCentroid
            )
        {
            List<Tuple<T, List<T>>> centroidsMap = new List<Tuple<T, List<T>>>();
            List<T> centroids = initialize(k, dataset);
            List<T> oldCentroids = new List<T>();

            int i = 0;
            while (i < iterations && centroids != oldCentroids)
            {
                i++;
                oldCentroids = centroids;
                
                //Makes sure that the dictonary is empty again.
                centroidsMap.Clear();
                foreach (var centroid in centroids)
                {
                    centroidsMap.Add(new Tuple<T, List<T>>(centroid, new List<T>()));
                }

                foreach (T data in dataset)
                {
                    T nearest = centroidsMap.OrderBy(it => calculateDistance(it.Item1, data)).First().Item1;
                    centroidsMap.ForEach(it =>
                    {
                        if (Equals(it.Item1, nearest))
                        {
                            it.Item2.Add(data);
                        }
                    });
                }

                Console.WriteLine();
            }

            return centroids;
        }
    }
}