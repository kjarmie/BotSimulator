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
        public double timeOfDeath;

        private float sX = 1f;    // the speed in the x direction
        private float sY = 1f;    // the speed in the y direction

        public float dX;        // the magnitude in the x
        public float dY;        // the magnitude in the y

        public int nrInputs;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        // void Update()
        // {
        //     // if (Time.timeScale == 0)
        //     // {
        //     //     return;
        //     // }

        //     // // Get the inputs for the NN
        //     // GetInputs();

        //     // // Run the movement logic
        //     // double[] outputs = brain.GetOutput(inputs);

        //     // // Calculate the amount to change in both x and y
        //     // dX = (float)outputs[0] * sX;
        //     // dY = (float)outputs[1] * sY;

        //     // // Cap the value of the movement
        //     // float max = 0.5f;
        //     // if (dX < 0)
        //     //     dX = (dX < (-1 * max)) ? (-1 * max) : dX;
        //     // else
        //     //     dX = (dX > max) ? max : dX;

        //     // if (dY < 0)
        //     //     dY = (dY < (-1 * max)) ? (-1 * max) : dY;
        //     // else
        //     //     dY = (dY > max) ? max : dY;

        //     // // Update the position of the agent
        //     // Vector3 pos = this.transform.position;
        //     // //this.transform.position = new Vector3(pos.x + (float)dX, pos.y + (float)dY, pos.z);
        //     // //transform.position.Set(transform.position.x + (dX * Time.timeScale), transform.position.y + (dY * Time.timeScale), transform.position.z);
        //     // transform.position = new Vector3(transform.position.x + (dX * Time.timeScale), transform.position.y + (dY * Time.timeScale), transform.position.z);
        // }


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

            // The outputs are in the form up-left, up, up-right, right, down-right, down, down-left, left, no-move

            // The largest value is the one selected and the spaceship moves sX and sY in that direction
            int direction = -1;
            double max = double.MinValue;
            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] > max)
                {
                    max = outputs[i];
                    direction = i;
                }
            }

            switch (direction)
            {
                case 0: // up-left
                    dX = -1;
                    dY = 1;
                    break;
                case 1: // up
                    dX = 0;
                    dY = 1;
                    break;
                case 2: // up-right
                    dX = 1;
                    dY = 1;
                    break;
                case 3: // right
                    dX = 1;
                    dY = 0;
                    break;
                case 4: // down-right
                    dX = 1;
                    dY = -1;
                    break;
                case 5: // down
                    dX = 0;
                    dY = -1;
                    break;
                case 6: // down-left
                    dX = -1;
                    dY = -1;
                    break;
                case 7: // left
                    dX = -1;
                    dY = 0;
                    break;
                case 8: // no move
                    dX = 0;
                    dY = 0;
                    break;
            }

            // switch (direction)
            // {
            //     case 0: // up-left -> down-right
            //         // dX = -1;
            //         // dY = 1;
            //         dX = 1;
            //         dY = -1;
            //         break;
            //     case 1: // up -> down
            //         // dX = 0;
            //         // dY = 1;
            //         dX = 1;
            //         dY = 0;
            //         break;
            //     case 2: // up-right - > down-left
            //         // dX = 1;
            //         // dY = 1;
            //         dX = 1;
            //         dY = 1;
            //         break;
            //     case 3: // right    -> left
            //         // dX = 1;
            //         // dY = 0;
            //         dX = 0;
            //         dY = 1;

            //         break;
            //     case 4: // down-right -> up-left
            //         // dX = 1;
            //         // dY = -1;

            //         dX = -1;
            //         dY = 1;
            //         break;
            //     case 5: // down -> down
            //         // dX = 0;
            //         // dY = -1;

            //         dX = -1;
            //         dY = 0;
            //         break;
            //     case 6: // down-left -> up-right
            //         // dX = -1;
            //         // dY = -1;
            //         dX = -1;
            //         dY = -1;
            //         break;
            //     case 7: // left -> right
            //         // dX = -1;
            //         // dY = 0;
            //         dX = 0;
            //         dY = -1;
            //         break;
            //     case 8: // no move
            //         dX = 0;
            //         dY = 0;
            //         break;
            // }

            // Check that it is not out of bounds
            float newX = transform.position.x + (dX * Time.timeScale * sX);
            float newY = transform.position.y + (dY * Time.timeScale * sY);

            // newX = (newX < 35) ? 35 : newX;
            // newX = (newX > 1885) ? 1885 : newX;

            // newY = (newY > -45) ? -45 : newY;
            // newY = (newY < -1035) ? -1035 : newY;

            transform.position = new Vector3(newX, newY, transform.position.z);
            //transform.position = new Vector3(transform.position.x + (dX * Time.timeScale * sX), transform.position.y + (dY * Time.timeScale * sY), transform.position.z);
        }

        public void Init(int nrInputs, int nrHidden, int nrOutputs, double minWeight, double maxWeight)
        {
            // Create the brain
            // Func<double, double, double> activation = (net, lambda) =>
            // {
            //     double numerator = Math.Exp(lambda * net) - Math.Exp((-1.0) * lambda * net);
            //     double denominator = Math.Exp(lambda * net) + Math.Exp((-1.0) * lambda * net);


            //     return numerator / denominator;
            // };

            Func<double, double, double> outer = (net, lambda) =>
            {
                double numerator = 1;
                double denominator = 1 + Math.Exp((-1.0) * lambda * net);

                return numerator / denominator;
            };

            brain = new NN(nrInputs, nrHidden, nrOutputs, minWeight, maxWeight, outer);

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
            for (int i = 0; i < GameController.aliens.Length; i++)
            {
                // Get the Alien
                Alien alien = GameController.aliens[i];

                // // Get its distance from the spaceship
                // double aX = 1 / Math.Abs((pos.x - alien.transform.position.x) / 1920);
                // double aY = 1 / Math.Abs((pos.y - alien.transform.position.y) / 1080);
                // // double aY = alien.transform.position.y / 1080;

                double distance = Math.Sqrt(Math.Pow(((pos.x - alien.transform.position.x) / 1920), 2) + Math.Pow(((pos.y - alien.transform.position.y) / 1080), 2));



                // // Get its velocity
                // double dX = (alien.dX * Time.timeScale) / 1920;
                // double dY = (alien.dY * Time.timeScale) / 1080;

                // Check if the alien is heading toward or away from the spaceship
                double newDist = Math.Sqrt(Math.Pow(((pos.x - alien.transform.position.x + alien.dX) / 1920), 2) + Math.Pow(((pos.y - alien.transform.position.y + alien.dY) / 1080), 2));
                if (newDist < distance)
                {
                    inputs[x] = 1 / distance;   // heading towards, distance has big impact
                }
                else
                {
                    inputs[x] = 1 / distance;
                }

                // // Set in the input
                // inputs[x] = aX;         // x
                // inputs[x + 1] = aY;      // y
                // inputs[x + 2] = dX;     // dx
                // inputs[x + 3] = dY;     // dy

                // //increment x by 4
                // x = x + 4;
                x++;
            }

            // Get the locations of the edges
            // x should be 22;
            // inputs[x] = GameController.leftX / 1920;
            // inputs[x + 1] = GameController.rightX / 1920;
            // inputs[x + 2] = GameController.topY / 1080;
            // inputs[x + 3] = GameController.bottomY / 1080;

            inputs[x] = 1 / Math.Abs((pos.x - GameController.leftX) / 1920);
            inputs[x + 1] = 1 / Math.Abs((pos.x - GameController.rightX) / 1920);
            inputs[x + 2] = 1 / Math.Abs((pos.y - GameController.topY) / 1080);
            inputs[x + 3] = 1 / Math.Abs((pos.y - GameController.bottomY) / 1080);
        
            // Add the bias
            inputs[x + 4] = -1;
        }


        // Collision logic - will die if it collides with the sides or an alien
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Calculate the new vector values
            string name = other.gameObject.name.ToLower();

            // if name contains "alien" or "side"
            if (name.Contains("alien"))
            {
                // Get the fitness
                this.fitness = GameController.time;
                this.timeOfDeath = GameController.time;

                // Deactivate this GameObject
                this.gameObject.SetActive(false);

                // Decrement the number of spaceships still alive
                GameController.nrAgents--;

            }
            else if (name.Contains("side"))
            {
                // Get the fitness
                this.fitness = Math.Sqrt(GameController.time);
                this.timeOfDeath = GameController.time;

                // Deactivate this GameObject
                this.gameObject.SetActive(false);

                // Decrement the number of spaceships still alive
                GameController.nrAgents--;
            }

        }
    }
}
