using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    internal class Program
    {
        static Random rnd = new Random();
        public static void Main(string[] args)
        {
            Algorithm<string> program = new Algorithm<string>(0.8, 0.4, true, 5, 50);

            /**
             * Creates an individual
             *
             * returns an individual
             */
            Func<string> createIndividual = () => {
                //Create variable that will be returned as new Individual
                string output = "";

                for(int i = 0; i < 5; i++){
                    output += rnd.Next(0,2);
                }

                return output;
            };
            
            /**
             * Calculates the fitness of an individual
             *
             * return the calculated fitness
             */
            Func<string, double> computeFitness = (individual) => {
                double fitness = 0.0;

                double x = Convert.ToInt32(individual, 2);
                fitness = -(Math.Pow(x, 2.0)) + (7*x);

                return fitness;
            };

            /**
             * Select two parents
             *
             * return Tuple of two strings (two parents)
             */
            Func<string[], double[], Func<Tuple<string, string>>> selectTwoParents = (individuals, fitness) => {
                Func<Tuple<string, string>> select = () =>
                {
                    List<double> positiveFitness = new List<double>();
                    
                    foreach (double item in fitness)
                    {
                        positiveFitness.Add(item + Math.Abs(fitness.Min()));
                    }
                    int startPoint = Convert.ToInt32(positiveFitness.Min());
                    int maxValue = Convert.ToInt32(positiveFitness.Sum());

                    Tuple<string, double>[] sortedIndividualsWithFitness = zipSorted(individuals, fitness);

                    string[] parents =
                    {
                        "",
                        ""
                    };
                    
                    for (int i = 0; i < 2; i++)
                    {
                        int res = rnd.Next(startPoint, maxValue);
                        for (int j = 0; j < sortedIndividualsWithFitness.Length; j++)
                        {
                            if (j + 1 == sortedIndividualsWithFitness.Length && parents[i] == "")
                            {
                                if (res >= sortedIndividualsWithFitness[j].Item2)
                                {
                                    parents[i] = sortedIndividualsWithFitness[j].Item1;
                                }
                            }
                            else
                            {
                                if (res >= sortedIndividualsWithFitness[j].Item2 &&
                                    res < sortedIndividualsWithFitness[j + 1].Item2 && parents[i] == "")
                                {
                                    parents[i] = sortedIndividualsWithFitness[j].Item1;
                                }
                            }
                        }
                        
                    }
                    return new Tuple<string, string>(parents[0], parents[1]);
                };

                return select;
            };
            
            /**
             * Creates the childeren from the parents
             *
             * returns the child
             */
            Func<Tuple<string, string>, Tuple<string, string>> crossover = (parents) =>
            {
                int l = parents.Item1.Length;
                int split = rnd.Next(1, l - 1);
                string mother = parents.Item1;
                string father = parents.Item2;
                
                return new Tuple<string, string>(
                    mother.Substring(0, split) + father.Substring(split,  father.Length-split),
                    father.Substring(0, split) + mother.Substring(split, mother.Length-split));
            };

            /**
             * Mutates the child with the chance of mutationRate
             *
             * returns (Mutated)child
             */
            Func<string, double, string> mutation = (child, mutationRate) =>
            {
                string output = "";
                foreach (char c in child)
                {
                    if (mutationRate <= rnd.NextDouble())
                    {
                        if (c == '1')
                        {
                            output += "0";
                        }
                        else
                        {
                            output += "1";
                        }
                    }
                    else
                    {
                        output += c;
                    }
                }

                return output;
            };

            List<string> results = new List<string>();
            for (int i = 0; i < 500; i++)
            {
                results.Add(program.Run(createIndividual, computeFitness, selectTwoParents, crossover, mutation));
            }

            List<int> binaryRes = results.OrderBy(it => it).ToList().Select(it => Convert.ToInt32(it, 2)).ToList();
            int correct = 0;
            foreach (var res in binaryRes)
            {
                if (res == 3 || res == 4)
                {
                    correct++;
                    Console.WriteLine(res);
                }
               
            }
            Console.WriteLine("Correct ones: " + correct + "on 500");

        }

        private static Tuple<A, B>[] zipSorted<A,B>(A[] list1, B[] list2)
        {
            if (list1.Length != list2.Length) return null;
            
            List<Tuple<A, B>> zipped = new List<Tuple<A, B>>();

            for (int i = 0; i < list1.Length; i++)
            {
                zipped.Add(new Tuple<A,B>(list1[i], list2[i]));
            }
            
            return zipped.OrderBy( it => it.Item2 ).ToArray();
        }
    }
}