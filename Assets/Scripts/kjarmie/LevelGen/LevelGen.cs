using System.IO;
using UnityEngine;
using FNLite;

namespace LevelGen
{
    class LevelGen
    {
        // The grid that will be generated for the bot
        private Color[,] grid;

        // 

        /// <summary>
        /// This method will generate the grid of tiles using Perlin Noise
        /// </summary>
        public static void GenerateGrid()
        {
                // Create and configure FastNoise object
                var rand = new System.Random();
                FastNoiseLite noise = new FastNoiseLite(rand.Next());
                noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

                // Gather noise data
                int rows = 512, cols = 512;
                float[,] noiseData = new float[rows, cols];
                int index = 0;

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        noiseData[r, c] = noise.GetNoise(c, r);
                    }
                }

                Texture2D texture = new Texture2D(cols, rows);
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < cols; x++)
                    {
                        // int colour = (int) (noiseData[y, x] * 255);
                        float colour = noiseData[y, x];
                        // texture.SetPixel(x, y, new Color32(colour, colour, colour, 255));
                        texture.SetPixel(x, y, new Color(colour, colour, colour, 1));
                    }
                }

                File.WriteAllBytes(@".\Assets\output\test.png", texture.EncodeToPNG());
        }
    }
}



