using System;

namespace Simulation
{
    public class NN
    {
        // NN ATTRIBUTES
        protected double[,] w; // the weights for the output neurons
        protected double[,] v; // the weights for the hidden neurons

        protected int K; // total number of output neurons
        protected int P; // total number of input patterns
        protected int J; // total number of hidden neurons
        protected int I; // total number of inputs per pattern

        protected Activation activation;    // represents a specific activation function for the neurons (such as Sigmoid, Linear, etc.)

        // HELPER ATTRIBUTES
        protected Random r;     // the random object that will produce random values for initialization

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nrInputs">The number of input neurons.</param>
        /// <param name="nrHidden">The number of hidden layer neurons (NN assumes 1 hidden layer).</param>
        /// <param name="nrOutputs">The number of output neurons.</param>
        /// <param name="wLower">The minimum starting value for the weights.</param>
        /// <param name="wUpper">The maximum starting value for the weights.</param>
        public NN(int nrInputs, int nrHidden, int nrOutputs, double wLower, double wUpper)
        {
            // Create the data structures

            // Assign the sizes
            this.K = nrOutputs;
            this.P = nrInputs;
            this.J = nrHidden;

            // Create the arrays
            w = new double[K, J + 1];
            v = new double[J, I + 1];

            r = new Random(25256);

            // Initialize the weights
            for (int k = 0; k < K; k++)
            {
                for (int j = 0; j < J + 1; j++)
                {
                    double newWeight = wLower + (wUpper - wLower) * r.NextDouble();
                    w[k, j] = newWeight;
                }
            }
            for (int j = 0; j < K; j++)
            {
                for (int i = 0; i < I + 1; i++)
                {
                    double newWeight = wLower + (wUpper - wLower) * r.NextDouble();
                    v[j, i] = newWeight;
                }
            }
        }

        /// <summary>
        /// This method will produce the array of outputs based on the provided inputs.
        /// </summary>
        /// <param name="inputs">An array of P doubles where P is the number of input neurons.</param>
        /// <returns>An array of K doubles where K is the number of output neurons.</returns>
        private double[] GetOutput(double[] inputs)
        {
            // For each output neuron, get its value using the FFNN formula
            double[] outputs = new double[K];
            double[] hidden = new double[J + 1];

            for (int k = 0; k < K; k++)
            {
                // Get the output for each output neuron

                double netO = 0;

                for (int j = 0; j < J + 1; j++)
                { // for all the hidden neurons

                    if (j < J)
                    { // for all the hidden neurons
                        double netY = 0;

                        for (int i = 0; i < I + 1; i++)
                        {
                            netY += v[j, i] * inputs[i];
                        }

                        // Set the output for the hidden neuron as function of netY
                        hidden[j] = activation.activate(netY, 1);
                    }

                    else
                    { // for the bias
                        hidden[j] = -1;
                    }

                    // Update netO
                    netO += w[k, j] * hidden[j];
                }

                // Set the output of the output neuron as a function of the netO
                outputs[k] = activation.activate(netO, 1);
            }

            return outputs;
        }
    }
}

