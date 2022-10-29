using System;

namespace Simulation
{
    public interface Activation
    {
        public double activate(double net, double lambda);
    }
}

