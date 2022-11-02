using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Simulation
{
    public class Spaceship : MonoBehaviour
    {
        public NN brain;    // the agent this particular spaceship is representing
        public double[] inputs; // the inputs for the NN to get the outputs
        public double[] outputs;    // the outputs from the NN, which is the (x,y) location

        public double fitness;

        public float sX = 1920;    // the speed in the x direction
        public float sY = 1080;    // the speed in the y direction

        public float dX;        // the magnitude in the x
        public float dY;        // the magnitude in the y

        public int nrInputs;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }

            // Get the inputs for the NN
            GetInputs();

            // Run the movement logic
            double[] outputs = brain.GetOutput(inputs);

            // Calculate the amount to change in both x and y
            dX = (float)outputs[0] * sX;
            dY = (float)outputs[1] * sY;

            // Cap the value of the movement
            float max = 0.5f;
            if (dX < 0)
                dX = (dX < (-1 * max)) ? (-1 * max) : dX;
            else
                dX = (dX > max) ? max : dX;

            if (dY < 0)
                dY = (dY < (-1 * max)) ? (-1 * max) : dY;
            else
                dY = (dY > max) ? max : dY;

            // Update the position of the agent
            Vector3 pos = this.transform.position;
            //this.transform.position = new Vector3(pos.x + (float)dX, pos.y + (float)dY, pos.z);
            //transform.position.Set(transform.position.x + (dX * Time.timeScale), transform.position.y + (dY * Time.timeScale), transform.position.z);
            transform.position = new Vector3(transform.position.x + (dX * Time.timeScale), transform.position.y + (dY * Time.timeScale), transform.position.z);
        }

        public void Init(int nrInputs, int nrHidden, int nrOutputs, double minWeight, double maxWeight)
        {
            // Create the brain
            Func<double, double, double> activation = (net, lambda) =>
            {
                double numerator = Math.Exp(lambda * net) - Math.Exp((-1.0) * lambda * net);
                double denominator = Math.Exp(lambda * net) + Math.Exp((-1.0) * lambda * net);


                return numerator / denominator;
            };

            brain = new NN(nrInputs, nrHidden, nrOutputs, minWeight, maxWeight, activation);

            // Init the inputs  
            inputs = new double[nrInputs + 1];  // +1 for the bias
            this.nrInputs = nrInputs;
        }

        public double[] GetBrain()
        {
            double[] weights = brain.wrap();

            return weights;
        }

        public void SetBrain(double[] weights)
        {
            // TODO: Set the brain for the NN
            brain.unwrap(weights);
        }

        private void GetInputs()
        {
            inputs = new double[nrInputs + 1];
            // Get the x and y coordinates
            Vector3 pos = this.transform.position;
            inputs[0] = pos.x / 1920;
            inputs[1] = pos.y / 1080;

            // Get all the Alien x, y coordinates and x,y direction vectors
            int x = 2;
            if (GameController.aliens == null || GameController.aliens.Length == 0)
            {
                // for (int i = 0; i < GameController.nrAliens; i += 4)
                // {
                //     inputs[i] = 10;         // x
                //     inputs[i + 1] = -10;     // y
                //     inputs[i + 2] = 0;     // dx
                //     inputs[i + 3] = 0;     // dy
                // }
            }
            else
            {
                for (int i = 0; i < GameController.aliens.Length; i++)
                {
                    // Get the Alien
                    Alien alien = GameController.aliens[i];

                    // Get its position
                    double aX = alien.transform.position.x / 1920;
                    double aY = alien.transform.position.y / 1080;

                    // Get its velocity
                    double dX = (alien.dX * Time.timeScale) / 1920;
                    double dY = (alien.dY * Time.timeScale) / 1080;

                    // Set in the input
                    inputs[x] = aX;         // x
                    inputs[x + 1] = aY;      // y
                    inputs[x + 2] = dX;     // dx
                    inputs[x + 3] = dY;     // dy

                    //increment x by 4
                    x = x + 4;
                }
            }

            // Get the locations of the edges
            // x should be 22;
            inputs[x] = GameController.leftX / 1920;
            inputs[x + 1] = GameController.rightX / 1920;
            inputs[x + 2] = GameController.topY / 1080;
            inputs[x + 3] = GameController.bottomY / 1080;

            // Add the bias
            inputs[x + 4] = -1;
        }


        // Collision logic - will die if it collides with the sides or an alien
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Calculate the new vector values
            string name = other.gameObject.name.ToLower();

            // if name contains "alien" or "side"
            if (name.Contains("side") || name.Contains("alien"))
            {
                // Get the fitness
                this.fitness = GameController.time;

                // Deactivate this GameObject
                this.gameObject.SetActive(false);

                // Decrement the number of spaceships still alive
                GameController.nrAgents--;

            }

        }
    }
}
