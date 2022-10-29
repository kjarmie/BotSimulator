using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] Alien alienPrefab;   // the prefab of the Asteroid object (obstacle)
        [SerializeField] Spaceship spaceshipPrefab;   // the prefab of the Spaceship object (agent)

        int generation;     // the current generation
        double time;        // the time in seconds the best individual in the current generation has survived

        static int nrAgents;       // the number of agents still alive
        static int totAgents;      // the maximum number of agents


        // TRAINING 
        static EA ea = new EA();   // this EA will train the agents
        static Spaceship[] spaceships;   // this will contain all the spaceship objects
        static Alien[] aliens;   // this will contain all the asteroid objects

        // Start is called before the first frame update
        void Start()
        {
            ArrayList brains = new ArrayList();

            // Create totAgents number of agents. They will have random brains (NN) when initialized
            Vector3 origin = new Vector3(0, 0, -0.1f);
            for (int i = 0; i < totAgents; i++)
            {
                // Create a spaceship
                Spaceship spaceship = Instantiate(spaceshipPrefab, origin, Quaternion.identity);

                // Initialize
                spaceship.Init();

                // Collect the brains and add them to a list
                brains.Add(spaceship.GetBrain());
            }

            // Use the collected brains to start the EA

        }

        public static double[] Simulate(double[,] population)
        {
            // double[] fitness = new double[population.GetUpperBound(0)];
            double[] fitness;

            // Assign each of the agents a new brain using the population
            for (int i = 0; i < totAgents; i++)
            {
                // Get the spaceship
                Spaceship spaceship = spaceships[i];

                // Get the brain
                double[] brain = new double[nrWeights];
                for (int j = 0; j < nrWeights; j++)
                {
                    brain[i] = population[i, j];
                }

                // Set the brain
                spaceship.SetBrain(brain);
            }

            // Run the game


            // Once all the agents are dead, save the times and return to the EA


            return
        }


        // Handle the spawning of the agents
        private void CreateNewGeneration()
        {

        }


        // Handle 





    }

}