using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Implementation.GridRepresentation;
using Implementation.Searching;
using Implementation.UserInput;
using Microsoft.Xna.Framework;

namespace Implementation
{
    public class Simulation
    {
        private readonly GraphicsDeviceManager _graphics;

        private const int Repeat = 0;
        private int _current;

        private int _searchType = 0;
        private List<Search> _searchTypes; 

        public bool Complete = false;

        public Camera.Camera Camera { get; set; }
        public Graph Graph { get; set; }
        public Robot Robot { get; set; }
        public Stopwatch Stopwatch { get; set; }

        private string _filename;

        public Simulation(GraphicsDeviceManager graphics, int width, int height, int seed, int obstaclePercentage, Vector2 startPosition)
        {
            // Store the graphics device manager for the camera.
            _graphics = graphics;

            Initialize(width, height, seed, obstaclePercentage, startPosition);
        }

        // Needed to generate simulation:
        // width, height, seed, obstacle%, startPosition
        private void Initialize(int width, int height, int seed, int obstaclePercentage, Vector2 startPosition)
        {
            Stopwatch = new Stopwatch();

            // Create the world graph.
            Graph = new Graph(width, height);

            // Create the new graph
            Graph.CreateRoom(width, height, seed, obstaclePercentage, startPosition);

            // Create the robot and set it's starting position.
            Robot = new Robot(startPosition, Graph);

            // Create the camera.
            Camera = new Camera.Camera(_graphics.GraphicsDevice.Viewport.Width,
                                       _graphics.GraphicsDevice.Viewport.Height);

            // Focus the camera on the starting position.
            Camera.CenterOn(startPosition);

            int areaExplored = (int)(width*height/100.0 * obstaclePercentage);

            // Filename for saving the results.
            _filename = width + " x " + height + "  " + seed + "  " + obstaclePercentage + " " + areaExplored + "  " + startPosition.X + " x " + startPosition.Y;
        }

        /// <summary>
        /// Begin the simulation.
        /// </summary>
        public void Start()
        {
            // If the stopwatch is running, do nothing.
            if (Stopwatch.IsRunning) return;

            // Start the stopwatch.
            Stopwatch.Reset();
            Stopwatch.Start();

            // Set the robot's search type
            _searchTypes = new List<Search>
            {
                new BreadthFirst(Robot),
                new BreadthFirstAStar(Robot),
                new DepthFirst(Robot),
                new DepthFirstAStar(Robot)
            };
            Robot.Search = _searchTypes[_searchType];

            // Change the status of the robot.
            if (Robot.Status != Robot.RobotStatus.Complete)
                Robot.Status = Robot.RobotStatus.Active;
        }

        /// <summary>
        /// End the simulation and perform end of test stuff.
        /// </summary>
        public void Stop()
        {
            // Stop the stopwatch.
            Stopwatch.Stop();

            // Output the results to a file.
            SaveResults();

            // Perform the test multiple times.
            if (_current < Repeat)
            {
                _current++;
                Reset();
            }
            else
            {
                // Repeat for all types of search.
                if (_searchType < _searchTypes.Count() - 1)
                {
                    // Change the algorithm type.
                    _searchType++;

                    // Reset the current repeat status.
                    _current = 0;

                    // Reset the simulation.
                    Reset();

                    // Start with the new algorithm.
                    Start();
                }
                else
                {
                    Complete = true;
                    _searchType = 0;
                }
            }
        }

        public void Reset()
        {
            Stopwatch.Reset();
            Robot.Reset();
            Complete = false;
        }

        private void SaveResults()
        {
            int finalTileCount = 0;

            // Find final tile number.
            foreach (Cell cell in Robot.LocalGraph.Cells)
            {
                if (cell != null)
                    finalTileCount += cell.Visited;
            }

            string path = "Results/" + _filename + ".csv";

            if (!Directory.Exists("Results"))
            {
                Directory.CreateDirectory("Results");
            }

            if (!File.Exists(path))
            {
                // Create a file to write to. 
                using (StreamWriter streamWriter = File.CreateText(path))
                {
                    streamWriter.WriteLine(Robot.Search + ", " + Stopwatch.Elapsed.TotalSeconds + ", " + finalTileCount);
                }	
            }

            else
            {
                //  Save the results to a file?
                using (StreamWriter streamWriter = File.AppendText("Results/" + _filename + ".csv"))
                {
                    streamWriter.WriteLine(Robot.Search + ", " + Stopwatch.Elapsed.TotalSeconds + ", " + finalTileCount);
                }
            }

        }

        /// <summary>
        /// Update the simulation to check for completion status.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Update the robot.
            Robot.Update(gameTime);

            // Check if the robot has completed the exploration.
            if (Robot.Status == Robot.RobotStatus.Complete)
            {
                // If so stop the simulation.
                Stop();
            }
        }

    }
}
