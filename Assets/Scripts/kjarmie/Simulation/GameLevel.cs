namespace Simulation
{
    public class GameLevel
    {
        Agent[] agents; // this is a list of agents in the game
        Tile[,] grid;   // this is the physical grid. Each tile of the grid is either blank, an agent, or a asteroid

        public GameLevel(int nrAgents, int rows, int cols)
        {
            this.agents = new Agent[nrAgents];
            this.grid = new Tile[rows, cols];
        }

        public void play() {
            // Perform setup

            // Enter the game loop
            while (true)
            {
                // Make player movement




                // Spawn asteroids




                // Move all existing asteroids




                // Check if the player is hit




                // 
            }

        }
    }
}