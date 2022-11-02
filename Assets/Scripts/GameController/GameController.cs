using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Simulation
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] Alien alienPrefab;   // the prefab of the Asteroid object (obstacle)
        [SerializeField] Spaceship spaceshipPrefab;   // the prefab of the Spaceship object (agent)

        [SerializeField] TextMeshProUGUI txtTime;
        [SerializeField] TextMeshProUGUI txtGenerations;
        [SerializeField] TextMeshProUGUI txtAlive;
        [SerializeField] TextMeshProUGUI txtTimeScale;
        [SerializeField] TextMeshProUGUI txtBest;

        public static int generation = 0;     // the current generation
        public static double time = 0;        // the time in seconds the best individual in the current generation has survived

        public static int nrAgents;        // the number of agents still alive
        public static int totAgents;       // the maximum number of agents
        public static int nrWeights;       // the total number of weights in the NN

        public static int nrAliens;

        public float timeMult = 1;      // the timescale multiplier

        public ArrayList scores;

        // TRAINING 
        public static Spaceship[] spaceships;   // this will contain all the spaceship objects
        public static Alien[] aliens;   // this will contain all the asteroid objects

        public static double leftX = 0;
        public static double rightX = 1920;
        public static double topY = 0;
        public static double bottomY = -1080;


        void Start()
        {
            random = new System.Random();


            RunEA();

            Application.Quit();
        }

        void RunEA()
        {
            // Parameters
            nrAgents = 25;
            totAgents = 25;
            nrAliens = 15;

            int K = 8;  // number of outputs
            int I = 2 + (nrAliens * 1) + 4; // number of inputs
            int J = 18; // number of hidden

            // Set the parameters
            nrWeights = (K * (J + 1)) + (J * (I + 1)); // inputs + 1 + hidden + 1 + output;

            double[,] initPop = new double[totAgents, nrWeights];

            // Halt time until the simulation starts
            Time.timeScale = 0f;

            // Create totAgents number of agents. They will have random brains (NN) when initialized
            Vector3 origin = new Vector3(960, -540, -0.1f);
            spaceships = new Spaceship[totAgents];
            for (int i = 0; i < totAgents; i++)
            {
                // Create a spaceship
                Spaceship spaceship = Instantiate(spaceshipPrefab, origin, Quaternion.identity);

                // Add to the list
                spaceships[i] = spaceship;

                // Set the name
                spaceship.name = "Spaceship " + i;

                // Initialize
                spaceship.Init(I, J, K, -1, 1);

                // Collect the brains and add them to a list
                double[] brain = spaceship.GetBrain();
                for (int j = 0; j < nrWeights; j++)
                {
                    initPop[i, j] = brain[j];
                }
            }
            spaceshipPrefab.gameObject.SetActive(false);

            // Create the aliens
            aliens = new Alien[nrAliens];
            for (int i = 0; i < nrAliens; i++)
            {
                // Create an alien
                Alien alien = Instantiate(alienPrefab, alienPrefab.transform.position, Quaternion.identity);

                // Set the name
                alien.name = "Alien " + i;

                // Add to the list
                aliens[i] = alien;

                // Initialize
                alien.Init();
            }
            alienPrefab.gameObject.SetActive(false);

            // Use the collected brains to start the training
            StartCoroutine(Train(initPop, 100, totAgents, nrWeights, -1, 1, 0.4, 0.50, 0.5));
            StartCoroutine(UpdateScoreBoard());
        }

        public void Simulate(double[,] population)
        {
            txtGenerations.SetText("Generation: " + generation);

            double[] fitness = new double[population.GetUpperBound(0) + 1];
            // double[] fitness;

            // Assign each of the agents a new brain using the population
            for (int i = 0; i < totAgents; i++)
            {
                // Get the spaceship
                Spaceship spaceship = spaceships[i];

                // Reset its position and make it active
                spaceship.transform.position = new Vector3(960, -540, -0.1f);
                spaceship.gameObject.SetActive(true);

                // Get the brain
                double[] brain = new double[nrWeights];
                for (int j = 0; j < nrWeights; j++)
                {
                    brain[j] = population[i, j];
                }

                // Set the brain
                spaceship.SetBrain(brain);
            }
            for (int i = 0; i < nrAliens; i++)
            {
                // Get the alien
                Alien alien = aliens[i];

                // Re-init
                alien.Init();
            }

            // Run the game
            Time.timeScale = timeMult;    // restart time
            nrAgents = totAgents;   // set the number of agents still alive as the total
            time = 0;
        }


        IEnumerator UpdateScoreBoard()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.01f);

                time += 0.01f;

                if (txtTime != null)
                    txtTime.SetText(String.Format("Time: {0:0.000}s", time));
                if (txtGenerations != null)
                    // txtGenerations.SetText(($"Generation: {0:0.0}", generation));
                    txtGenerations.SetText(String.Format("Generation {0}", generation));
                if (txtAlive != null)
                    // txtAlive.SetText("Alive: " + nrAgents + "/" + totAgents);
                    txtAlive.SetText(String.Format("Alive {0} / {1}", nrAgents, totAgents));
            }
        }

        private void SaveData(string file_name)
        {
            // Create writer
            string local_dir = Directory.GetCurrentDirectory();
            string new_directory = local_dir + @"\outputs\";
            Directory.CreateDirectory(new_directory);

            string new_file = new_directory + file_name;

            StreamWriter writer = new StreamWriter(new_file);

            for (int i = 0; i < scores.Count; i++)
            {
                // Get the score
                string score = String.Format("{0};{1:0.000}", i, scores[i]);

                // Write
                writer.Write(score + "\n");
            }
            writer.Flush();
            writer.Close();

            writer = new StreamWriter(new_directory + "weights.txt");
            for (int i = 0; i < solution.Length; i++)
            {
                // Get the score
                string score = String.Format("{0:0.000}", solution[i]);

                // Write
                writer.Write(score + "\n");
            }
        }


        private void OnDisable()
        {
            StopCoroutine(UpdateScoreBoard());
        }

        public void btnOne()
        {
            Time.timeScale = 1f;
            timeMult = 1f;
            txtTimeScale.SetText("Timescale: x " + 1);
        }
        public void btnTwo()
        {
            Time.timeScale = 2f;
            timeMult = 2f;
            txtTimeScale.SetText("Timescale: x " + 2);
        }
        public void btnFive()
        {
            Time.timeScale = 5f;
            timeMult = 5f;
            txtTimeScale.SetText("Timescale: x " + 5);
        }
        public void btnMax()
        {
            Time.timeScale = 20f;
            timeMult = 20f;
            txtTimeScale.SetText("Timescale: MAX");
        }

        public void btnPause()
        {
            // Invert the time multiplier
            Time.timeScale = (Time.timeScale == 0f) ? timeMult : 0f;
        }

        public void btnSave()
        {
            // End all co-routines
            StopAllCoroutines();

            // Save the data from the simulation
            SaveData("scores.txt");

            // Load the main menu
            SceneManager.LoadScene("Menu");
        }

        protected double[] fitness;     // holds the fitness of each chromosome in the current population
        protected double[,] population; // holds the fitness of each chromosome in the current population

        protected double[] parent1;          // the location of the parents for the current generation
        protected double[] parent2;

        int maxGenerations;             // the number of generations the algorithm will run for
        protected int popSize;          // the number of chromosomes per generation
        protected int geneSize;         // the number of genes per chromosome
        protected double geneMin;          // the lower bound for the starting value of a gene
        protected double geneMax;          // the upper bound for the starting value of a gene
        protected double selectionProp; // the proportion of the population for tournament selection during parent selection 
        protected double mutChance;     // the chance a gene is mutated
        protected double mutMagnitude;  // the magnitude of the change in a gene

        protected double[] solution;    // the best solution found so far

        protected System.Random random;        // used for getting any random values

        // Will train the model
        public IEnumerator Train(double[,] initPopulation, int maxGenerations, int popSize, int geneSize, double geneMin, double geneMax, double selectionProp, double mutChance, double mutMagnitude)
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
            InitPopulation(initPopulation);

            fitness = new double[totAgents];
            solution = new double[nrWeights];
            scores = new ArrayList();

            // Process
            // while the terminating conditions are not met
            generation = 0;
            while (generation < maxGenerations)
            {
                // Evaluate their fitness - in this specific case, the evaluation is done by simulation
                EvaluateFitnessAll();

                // wait until all have died
                while (nrAgents > 0)
                {
                    yield return new WaitForSeconds(1);
                }

                StopCoroutine(UpdateScoreBoard());
                // Retrieve the fitness of all the agents
                int best = 0;
                for (int i = 0; i < totAgents; i++)
                {
                    // spaceships[i].gameObject.SetActive(true);
                    Spaceship s = spaceships[i];
                    double f = s.fitness;

                    fitness[i] = f;

                    if (fitness[i] > fitness[best])
                        best = i;
                    // spaceships[i].gameObject.SetActive(false);
                }

                solution = new double[nrWeights];
                for (int i = 0; i < geneSize; i++)
                {
                    solution[i] = population[best, i];
                }
                scores.Add(fitness[best]);

                // Print the time of death of each agent in this population
                if (generation == 1 || generation == 5 || generation == 10 || generation == 20 || generation == 100 || generation == 250 || generation == 500)
                {
                    // Create a writer
                    StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + @"\outputs\time_of_death\" + generation + ".txt");

                    // Write all times of death
                    writer.Write("Spaceship;Time Of Death");
                    for (int i = 0; i < totAgents; i++)
                    {
                        string str = String.Format(format: "{0};{1:0.000}", i, spaceships[i].timeOfDeath);
                        writer.Write(str + "\n");
                    }

                    writer.Flush();
                    writer.Close();
                }

                // Change the scoreboard
                txtBest.SetText(String.Format("Best: {0:0.000}s", time));

                // Select parents
                SelectParents();
                // while (p1 == p2)  //TODO: Ensure the arrays are compared properly for component-wise equality
                // {
                // p2 = SelectParent();
                // }

                // parent1 = new double[geneSize];
                // parent2 = new double[geneSize];

                // for (int j = 0; j < geneSize; j++)
                // {
                //     parent1[j] = population[p1, j];
                //     parent2[j] = population[p2, j];
                // }

                // Perform crossover and mutation
                for (int i = 0; i < popSize - 2; i++)
                {
                    // Create new individual with crossover
                    crossover(i);

                    // Perform mutation
                    mutate(i);
                }

                // Add the parents to the front of the population
                for (int j = 0; j < geneSize; j++)
                {
                    population[popSize - 2, j] = parent1[j];
                    population[popSize - 1, j] = parent2[j];
                }

                generation++;
            }

            SaveData("scores.txt");

            // // Get the best individual
            // int b = GetFittest();
            // for (int j = 0; j < geneSize; j++)
            // {
            //     solution[j] = population[b, j];
            // }
        }


        private void EvaluateFitnessAll()
        {
            // This method will create a NN from each of the chromosomes in the population, run a simulation and obtain a result which is the fitness of the 
            // TODO: Implement the simulation as an object that can be run easily 

            // Return the current generation to the GameController so that it can be used for the next generation
            Simulate(population);
        }

        // Will select a parents for the next generation, returns the location of the parent
        private void SelectParents()
        {
            // int p1 = random.Next(popSize);
            // int p2 = random.Next(popSize);
            int p1 = 0;
            int p2 = 1;

            ArrayList set1 = new ArrayList();
            ArrayList set2 = new ArrayList();

            // int bestS1 = random.Next(popSize);     // location of best from s1
            // int bestS2 = random.Next(popSize);     // location of best from s2
            List<int> check = new List<int>();
            for (int i = 0; i < popSize * selectionProp; i++)
            {
                // Get a random location and add to a list
                int loc = random.Next(popSize);

                // Ensure that the location hasn't already been checked
                while (check.Contains(loc))
                {
                    loc = random.Next(popSize);
                }
                check.Add(loc);

                // If it is better than the current best, make it the best
                if (fitness[loc] > fitness[p1])
                {
                    p1 = loc;
                }
            }
            check = new List<int>();
            for (int i = 0; i < popSize * selectionProp; i++)
            {
                // Get a random location and add to a list
                int loc = random.Next(popSize);

                // Ensure that the location hasn't already been checked
                while (check.Contains(loc))
                {
                    loc = random.Next(popSize);
                }
                check.Add(loc);

                // If it is better than the current best, make it the best
                if (fitness[loc] > fitness[p2] && loc != p1)
                {
                    p2 = loc;
                }
            }

            // Now create the parents
            parent1 = new double[geneSize];
            parent2 = new double[geneSize];

            for (int j = 0; j < geneSize; j++)
            {
                parent1[j] = population[p1, j];
                parent2[j] = population[p2, j];
            }



            // int bestLoc = 0;
            // // Select a proportion of the population randomly
            // List<int> check = new List<int>();

            // // Select the first parent
            // for (int i = 0; i < popSize * selectionProp; i++)
            // {
            //     // Get random location
            //     int loc = random.Next(popSize);

            //     // Ensure that the location hasn't already been checked
            //     while (check.Contains(loc))
            //     {
            //         loc = random.Next(popSize);
            //     }
            //     check.Add(loc);

            //     if (fitness[bestLoc] <= fitness[loc])
            //     {
            //         bestLoc = loc;
            //     }
            // }
            // p1 = bestLoc;


            // // Select the second parent
            // check = new List<int>();
            // for (int i = 0; i < popSize * selectionProp; i++)
            // {
            //     // Get random location
            //     int loc = random.Next(popSize);

            //     // Ensure that the location hasn't already been checked
            //     while (check.Contains(loc))
            //     {
            //         loc = random.Next(popSize);
            //     }
            //     check.Add(loc);

            //     if (fitness[bestLoc] <= fitness[loc])
            //     {
            //         bestLoc = loc;
            //     }
            // }
            // p2 = bestLoc;


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
                    // double mutationVal = SampleGaussian(random, mutMagnitude * population[individual, j], mutMagnitude);
                    double mutationVal = SampleGaussian(random, 0, mutMagnitude * population[individual, j]);
                    population[individual, j] += mutationVal;
                }
            }
        }

        void InitPopulation(double[,] initPopulation)
        {
            this.population = initPopulation;
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
        private double SampleGaussian(System.Random random, double mean, double stddev)
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