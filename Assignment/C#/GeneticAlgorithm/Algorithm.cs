using System;
using System.Linq;

namespace GeneticAlgorithm
{
    public class Algorithm<A>
    {
        double crossoverRate;
        double mutationRate;
        bool elitism;
        int populationSize;
        int numIterations;
        Random r = new Random();

        public Algorithm(double crossoverRate, double mutationRate, bool elitism, int populationSize, int numIterations)
        {
            this.crossoverRate = crossoverRate;
            this.mutationRate = mutationRate;
            this.elitism = elitism;
            this.populationSize = populationSize;
            this.numIterations = numIterations;
        }

        public Tuple<A, double> Run(Func<A> createAnIndividual, Func<A, double> computeFitness,
            Func<A[], double[], Func<Tuple<A, A>>> selectTwoParents,
            Func<Tuple<A, A>, Tuple<A, A>> crossover, Func<A, double, A> mutation)
        {
            // initialize the first population
            var initialPopulation = Enumerable.Range(0, populationSize).Select(i => createAnIndividual()).ToArray();

            var currentPopulation = initialPopulation;

            for (int generation = 0; generation < numIterations; generation++)
            {
                // compute fitness of each individual in the population
                var fitnesses = Enumerable.Range(0, populationSize).Select(i => computeFitness(currentPopulation[i]))
                    .ToArray();

                var nextPopulation = new A[populationSize];

                // apply elitism
                int startAex;
                if (elitism)
                {
                    startAex = 1;
                    var populationWithFitness = currentPopulation.Select((individual, index) =>
                        new Tuple<A, double>(individual, fitnesses[index]));
                    var populationSorted =
                        populationWithFitness.OrderByDescending(tuple => tuple.Item2); // item2 is the fitness
                    var bestAividual = populationSorted.First();
                    nextPopulation[0] = bestAividual.Item1;
                }
                else
                {
                    startAex = 0;
                }

                // initialize the selection function given the current individuals and their fitnesses
                var getTwoParents = selectTwoParents(currentPopulation, fitnesses);

                // create the individuals of the next generation
                for (int newA = startAex; newA < populationSize; newA++)
                {
                    // select two parents
                    var parents = getTwoParents();

                    // do a crossover between the selected parents to generate two children (with a certain probability, crossover does not happen and the two parents are kept unchanged)
                    Tuple<A, A> offspring;
                    if (r.NextDouble() < crossoverRate)
                        offspring = crossover(parents);
                    else
                        offspring = parents;

                    // save the two children in the next population (after mutation)
                    nextPopulation[newA++] = mutation(offspring.Item1, mutationRate);
                    if (newA < populationSize) //there is still space for the second children inside the population
                        nextPopulation[newA] = mutation(offspring.Item2, mutationRate);
                }

                // the new population becomes the current one
                currentPopulation = nextPopulation;

                // in case it's needed, check here some convergence condition to terminate the generations loop earlier
            }

            // recompute the fitnesses on the final population and return the best individual
            var finalFitnesses = Enumerable.Range(0, populationSize).Select(i => computeFitness(currentPopulation[i]))
                .ToArray();
            return new Tuple<A, double>(currentPopulation
                .Select((individual, index) => new Tuple<A, double>(individual, finalFitnesses[index]))
                .OrderByDescending(tuple => tuple.Item2).First().Item1,
                currentPopulation
                    .Select((individual, index) => new Tuple<A, double>(individual, finalFitnesses[index]))
                    .OrderByDescending(tuple => tuple.Item2).First().Item2);
        }
    }
}