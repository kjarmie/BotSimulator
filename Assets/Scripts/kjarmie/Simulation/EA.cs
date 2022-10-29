using System;
using System.Collections.Generic;
using System.Collections;

namespace Simulation
{
    class EA
    {
        protected double[] fitness;     // holds the fitness of each chromosome in the current population
        protected double[,] population; // holds the fitness of each chromosome in the current population

        protected double[] parent1;          // the location of the parents for the current generation
        protected double[] parent2;

        int maxGenerations;             // the number of generations the algorithm will run for
        protected int popSize;          // the number of chromosomes per generation
        protected int geneSize;         // the number of genes per chromosome
        protected int geneMin;          // the lower bound for the starting value of a gene
        protected int geneMax;          // the upper bound for the starting value of a gene
        protected double selectionProp; // the proportion of the population for tournament selection during parent selection 
        protected double mutChance;     // the chance a gene is mutated
        protected double mutMagnitude;  // the magnitude of the change in a gene

        protected double[] solution;    // the best solution found so far

        protected Random random;        // used for getting any random values

        /// <summary>
        /// Creates a new EA.
        /// </summary>
        public EA()
        {
            random = new Random();
        }

        // Will train the model
        public void Train(int maxGenerations, int popSize, int geneSize, int geneMin, int geneMax, double selectionProp, double mutChance, double mutMagnitude)
        {
            // Assign variables
            this.maxGenerations = maxGenerations;
            this.popSize = popSize;
            this.geneSize = geneSize;
            this.geneMin = geneMin;
            this.geneMax = geneMax;
            this.selectionProp = selectionProp;
            this.mutChance = mutChance;
            this.mutMagnitude = mutMagnitude;

            // Init population
            InitPopulation();

            // Process
            process();

            // Get the best individual
            int b = GetFittest();
            for (int j = 0; j < geneSize; j++)
            {
                solution[j] = population[b, j];
            }
        }


        private void process()
        {
            // while the terminating conditions are not met
            int curGeneration = 0;
            while (curGeneration < maxGenerations)
            {                
                // Evaluate their fitness - in this specific case, the evaluation is done by simulation
                EvaluateFitnessAll();

                // Select parents
                int p1 = SelectParent();
                int p2 = SelectParent();
                while (p1 == p2)  //TODO: Ensure the arrays are compared properly for component-wise equality
                {
                    p2 = SelectParent();
                }

                for (int j = 0; j < geneSize; j++)
                {
                    parent1[j] = population[p1, j];
                    parent1[j] = population[p2, j];
                }

                // Perform crossover and mutation
                for (int i = 0; i < popSize; i++)
                {
                    // Create new individual with crossover
                    crossover(i);

                    // Perform mutation
                    mutate(i);
                }
                curGeneration++;
            }
        }

        private void EvaluateFitnessAll()
        {
            // This method will create a NN from each of the chromosomes in the population, run a simulation and obtain a result which is the fitness of the 
            // TODO: Implement the simulation as an object that can be run easily 

            // Return the current generation to the GameController so that it can be used for the next generation
            fitness = GameController.Simulate(population);
        }

        // Will select a parents for the next generation, returns the location of the parent
        private int SelectParent()
        {
            int bestLoc = 0;
            // Select a proportion of the population randomly
            List<int> check = new List<int>();
            for (int i = 0; i < popSize * selectionProp; i++)
            {
                // Get random location
                int loc = random.Next(popSize);

                // Ensure that the location hasn't already been checked
                while (check.Contains(loc))
                {
                    loc = random.Next(popSize);
                }
                check.Add(loc);

                if (fitness[bestLoc] < fitness[loc])
                {
                    bestLoc = loc;
                }
            }

            return bestLoc;
        }

        private int GetFittest()
        {
            int best = 0;
            for (int i = 0; i < geneSize; i++)
            {
                if (fitness[i] > fitness[best])
                    best = i;
            }
            return best;
        }

        // Performs uniform crossover
        private void crossover(int chromosome)
        {
            for (int j = 0; j < geneSize; j++)
            {
                double k = random.NextDouble();
                if (k < 0.5)
                {
                    // newOne.data[j] = curParent1.data[j];
                    population[chromosome, j] = parent1[j];
                }
                else
                {
                    // newOne.data[j] = curParent2.data[j];
                    population[chromosome, j] = parent2[j];
                }
            }
        }

        // Takes an individual and mutates it
        private void mutate(int individual)
        {
            for (int j = 0; j < geneSize; j++)
            {
                double chance = random.NextDouble();
                if (chance <= mutChance)
                {
                    //double mutationVal = random.NextGaussian() * (mutMagnitude * individual.data[i]);
                    double mutationVal = SampleGaussian(random, mutMagnitude * population[individual, j], mutMagnitude);
                    population[individual, j] += mutationVal;
                }
            }
        }

        void InitPopulation()
        {
            for (int i = 0; i < popSize; i++)
            {
                for (int j = 0; j < geneSize; j++)
                {
                    population[i, j] = geneMin + (geneMax - geneMin) * random.NextDouble();
                }
            }
        }


        // The following code was provided from:
        // https://gist.github.com/tansey/1444070

        // This is the Box-Muller method for sampling random Gaussian numbers
        private double SampleGaussian(Random random, double mean, double stddev)
        {
            // The method requires sampling from a uniform random of (0,1]
            // but Random.NextDouble() returns a sample of [0,1).
            double x1 = 1 - random.NextDouble();
            double x2 = 1 - random.NextDouble();

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            return y1 * stddev + mean;
        }
    }
}

