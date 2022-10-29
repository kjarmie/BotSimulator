
namespace Simulation
{
    /// <summary>
    /// This class is the controller class for the algorithm. It will run simulations of the game as needed
    /// </summary>
    class Simulator
    {
        GameLevel gameLevel;    // this will be the actual game that gets played
        EA trainer;     // this is the EA that will train the NN of each agent

        public Simulator() {
            // First, create a new game level
            gameLevel = new GameLevel(10, 3, 3);



            // Setup the trainer
            trainer = new EA();
        }


        

    }


    public enum Tile {
        Agent = 0,
        Asteroid = 1,
        None = -1
    }
}