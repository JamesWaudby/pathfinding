using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Implementation.Searching
{
    class BreadthFirst : Search
    {
        public BreadthFirst(Robot robot) : base(robot)
        {
        }

        /// <summary>
        /// Find a path from the current position to the target.
        /// </summary>
        /// <param name="to">Position to path to.</param>
        /// <param name="from"></param>
        /// <returns>An IEnumerable of the represented path.</returns>
        public void FindPath(Vector2 to, Vector2 from)
        {
            // Get path to root for current.
            IEnumerable<Vector2> currentPath = Robot.LocalGraph.GetPath(from).ToList();

            // Get path to root for parent.
            IEnumerable<Vector2> parentPath = Robot.LocalGraph.GetPath(to).ToList();
            
            // Which place the paths cross at.
            Vector2 match = new Vector2();

            // Has the path been matched yet.
            bool matched = false;

            // Loop through each member of the parent ienumerable
            // and check whether or not it occurs in the current path
            // as well. Set match to equal the first match found.
            foreach (Vector2 parent in parentPath.TakeWhile(parent => !matched).Where(parent => currentPath.Any(current => current == parent)))
            {
                match = parent;
                matched = true;
            }

            // Join all of the positions before the match
            // from the currentPath with all of the positions
            // from the match onwards from the parentPath. 
            IEnumerable<Vector2> finalPath = currentPath.TakeWhile(current => current != match)
                                                        .Concat(parentPath.Reverse()
                                                        .SkipWhile(current => current != match));

            // Add the path to the queue.
            QueuePath(finalPath, from);
        }

        public override Queue<Vector2> GetPath()
        {
            // Get the neighbours of the current cell.
            IEnumerable<Vector2> neighbours = Robot.LocalGraph.UnvisitedNeighbours(CurrentParent).ToList();

            // Check if there are unvisited neighbours.
            if (neighbours.Any())
            {
                // Temporary position for the robot path planning.
                Vector2 currentPosition = Robot.GridPosition;

                // Add each of the unvisited neighbours to the queue
                // if the robot is at the current parent.
                foreach (Vector2 neighbour in neighbours.Where(neighbour => Robot.GridPosition == CurrentParent))
                {
                    // Add the movement to the queue
                    // with the return movement as well.
                    FindPath(neighbour, currentPosition);

                    // Update the temporary position.
                    currentPosition = neighbour;

                    // Add the newly discovered cell
                    // to the parent queue.
                    ParentQueue.Enqueue(neighbour);
                }
            }

            // There are no unvisited neighbours.
            else
            {
                // Are there anymore parent cells to check?
                if (ParentQueue.Any())
                {
                    // Get the next parent in the queue.
                    CurrentParent = ParentQueue.Dequeue();

                    if (Robot.LocalGraph.UnvisitedNeighbours(CurrentParent).Any())
                    {
                        // Get the path between the current position and the new parent.
                        FindPath(CurrentParent, Robot.GridPosition);
                    }
                }

                // The search must be over - return to the starting position.
                else
                {
                    // Check if the robot is back at the starting position.
                    if (Robot.GridPosition != Robot.StartPosition)
                    {
                        // Get the path between the current position and the new parent.
                        FindPath(Robot.StartPosition, Robot.GridPosition);
                    }
                    else
                    {
                        // There is no path remaining.
                        return null;
                    }
                }
            }

            return Path;
        }

        public override string ToString()
        {
            return "Breadth First Search";
        }
    }
}

