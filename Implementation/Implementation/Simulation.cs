using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Implementation.GridRepresentation;

namespace Implementation
{
    public class Simulation
    {
        private readonly Robot _robot;

        public Stopwatch Stopwatch { get; set; }

        public Simulation(Game1 game)
        {
            _robot = game.Robot;

            Stopwatch = new Stopwatch();
        }

        /// <summary>
        /// Begin the simulation.
        /// </summary>
        public void Start()
        {
            // If the stopwatch is running, do nothing.
            if (Stopwatch.IsRunning) return;

            // Start the stopwatch.
            Stopwatch.Start();

            // Change the status of the robot.
            _robot.Status = Robot.RobotStatus.Active;
        }

        /// <summary>
        /// Update the simulation to check for completion status.
        /// </summary>
        public void Update()
        {
            // Check if the robot has completed the exploration.
            if (_robot.Status == Robot.RobotStatus.Complete)
            {
                // If so stop the simultion.
                Stop();
            }
        }

        /// <summary>
        /// End the simulation and perform end of test stuff.
        /// </summary>
        public void Stop()
        {                
            // Stop the stopwatch.
            Stopwatch.Stop();

            int finalTileCount = 0;

            // Find final tile number.
            foreach (Cell cell in _robot.LocalGraph.Cells)
            {
                finalTileCount += cell.Visited;
            }

            //  Output the results to a file?
        }
    }
}
