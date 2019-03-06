using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            /* FUNCTIONS TO DEFINE (for each problem):
            Func<Ind> createIndividual;                                 ==> input is nothing, output is a new individual
            Func<Ind,double> computeFitness;                            ==> input is one individual, output is its fitness
            Func<Ind[],double[],Func<Tuple<Ind,Ind>>> selectTwoParents; ==> input is an array of individuals (population) and an array of corresponding fitnesses, output is a function which (without any input) returns a tuple with two individuals (parents)
            Func<Tuple<Ind, Ind>, Tuple<Ind, Ind>> crossover;           ==> input is a tuple with two individuals (parents), output is a tuple with two individuals (offspring/children)
            Func<Ind, double, Ind> mutation;                            ==> input is one individual and mutation rate, output is the mutated individual
            */

            string indu = createIndividual();
            
            Func<Ind> createIndividual<Ind>() 
            {
                
            }

            GeneticAlgorithm<int> fakeProblemGA = new GeneticAlgorithm<int>(0.0, 0.0, false, 0, 0); // CHANGE THE GENERIC TYPE (NOW IT'S INT AS AN EXAMPLE) AND THE PARAMETERS VALUES
            var solution = fakeProblemGA.Run(createIndividual(), null, null, null, null); 
            Console.WriteLine("Solution: ");
            Console.WriteLine(solution);

        }

        static string createIndividual()
        {
            string output = "";
            Random random = new Random();

            for (int i = 0; i < 5; i++)
            {
                output += random.Next(0,2);
            }
            
            return output;
        }
    }
}
