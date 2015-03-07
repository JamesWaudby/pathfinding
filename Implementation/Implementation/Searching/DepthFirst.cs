using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Implementation.Searching
{
    class DepthFirst : Search
    {
        public DepthFirst(Robot robot) : base(robot)
        {

        }

        public override Queue<Vector2> GetPath()
        {
            // Search for the unvisited neighbours.
            IEnumerable<Vector2> neighbours = _robot.LocalGraph.UnvisitedNeighbours(_robot.GridPosition).ToList();

            // Check that there are neighbours.
            if (neighbours.Any())
            {
                // Move to the first unvisited cell.
                Vector2 next = neighbours.First() - _robot.GridPosition;

                // Add the next position to the movement queue - not a DFS queue!
                Path.Enqueue(next);
            }

            // If there are no unvisited cells, move back to the parent of the cell.
            else
            {
                // Check if we are back at the starting position - this signals the end.
                if (_robot.GridPosition != _robot.StartPosition)
                {
                    Vector2? next =
                        (_robot.LocalGraph.Cells[(int) _robot.GridPosition.X, (int) _robot.GridPosition.Y].Parent -
                         _robot.GridPosition);

                    if (next != null)
                        Path.Enqueue((Vector2)next);
                }
                else
                {
                    // There is no path remaining.
                    return null;
                }
            }

            return Path;
        }
    }
}
