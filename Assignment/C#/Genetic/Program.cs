using System;

namespace Genetic
{
    class Program
    {
        static void Main(string[] args)
        {

            Algorithm<string> program = new Algorithm<string>(0.5, 0.6, false, 5, 50);

            Func<string> createIndividual = () => {
                //Create variable that will be returned as new Individual
                string output = "";


                Random rnd = new Random();

                for(int i = 0; i < 5; i++){
                    output += rnd.Next(0,2);
                }

                return output;
            };

            Func<string, double> computeFitness = (individual) => {
                double fitness = 0.0;

                double x = ((double)(Convert.ToInt32(individual, 2)));
                fitness = -(Math.Pow(x, 2.0)) + (7*x);

                return fitness;
            };

            Func<string[], double[], Func<Tuple<string, string>>> selectTwoParents = (individuals, fitness) => {
                Func<Tuple<string, string>> select = () => {
                    return new Tuple<string, string>("test1", "test2");
                };

                return select;
            };
            
            string[] generatedIndividuals = [createIndividual(), createIndividual()];
            double[] calculatedFitness = [computeFitness(generatedIndividuals[0]), computeFitness(generatedIndividuals[1])];

            var parents = selectTwoParents(generatedIndividuals, calculatedFitness);
        }
    }
}
