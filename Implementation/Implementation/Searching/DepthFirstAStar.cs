using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Implementation.Searching
{
    class DepthFirstAStar : Search
    {
        public DepthFirstAStar(Robot robot) : base(robot)
        {
        }

        /// <summary>
        /// Provides the path constructed through the combined use of BFS and A*.
        /// </summary>
        /// <returns>A queue of vector positions to move to.</returns>
        public override Queue<Vector2> GetPath()
        {
            // Search for the unvisited neighbours.
            IEnumerable<Vector2> neighbours = Robot.LocalGraph.UnvisitedNeighbours(Robot.GridPosition).ToList();

            // Check that there are neighbours.
            if (neighbours.Any())
            {
                // Move to the first unvisited cell.
                Vector2 next = neighbours.First() - Robot.GridPosition;

                // Add the next position to the movement queue - not a DFS queue!
                Path.Enqueue(next);
            }

            // If there are no unvisited cells, move back to the parent of the cell.
            else
            {
                // Get the parent of the current cell.
                Vector2? next = Robot.LocalGraph.Cells[(int) Robot.GridPosition.X, (int) Robot.GridPosition.Y].Parent;

                // Find the first cell to have unvisited neighbours.
                while (next != null && !Robot.LocalGraph.UnvisitedNeighbours((Vector2) next).Any())
                {
                    Vector2 parent = (Vector2)next;
                    next = Robot.LocalGraph.Cells[(int) parent.X, (int) parent.Y].Parent;
                }

                // The robot is not currently back at the starting position.
                if (next != null)
                {
                    QueuePath(AStar.GetPath(Robot, (Vector2) next, Robot.GridPosition), Robot.GridPosition);
                }
                else
                {
                    // Check if the robot is back at the starting position.
                    if (Robot.GridPosition != Robot.StartPosition)
                    {
                        // Get the path between the current position and the new parent.
                        QueuePath(AStar.GetPath(Robot, Robot.StartPosition, Robot.GridPosition), Robot.GridPosition);
                    }
                    else
                    {
                        // There is no path remaining.
                        return null;
                    }
                }
            }
            // Return the path to the new location.
            return Path;
        }

        public override string ToString()
        {
            return "Depth First A* Search";
        }
    }
}
