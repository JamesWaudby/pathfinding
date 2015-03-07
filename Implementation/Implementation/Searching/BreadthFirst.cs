using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Implementation.Searching
{
    class BreadthFirst : Search
    {
        /// <summary>
        /// Queue of vectors to represent the order in which the children
        /// must be visited.
        /// </summary>
        private readonly Queue<Vector2> _parentQueue;

        /// <summary>
        /// The current parent node to be visited.
        /// </summary>
        private Vector2 _currentParent;

        public BreadthFirst(Robot robot) : base(robot)
        {
            _parentQueue = new Queue<Vector2>();
            _currentParent = _robot.GridPosition;
        }

        /// <summary>
        /// Find a path from the current position to the target.
        /// </summary>
        /// <param name="to">Position to path to.</param>
        /// <param name="from"></param>
        /// <returns>An IEnumerable of the represented path.</returns>
        public void QueuePath(Vector2 to, Vector2 from)
        {
            // Get path to root for current.
            IEnumerable<Vector2> currentPath = _robot.LocalGraph.GetPath(from).ToList();

            // Get path to root for parent.
            IEnumerable<Vector2> parentPath = _robot.LocalGraph.GetPath(to).ToList();

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

            // Cache the current position.
            Vector2 position = from;

            // Loop through all of the path.
            foreach (Vector2 next in finalPath)
            {
                // Find the direction needed to move in.
                Vector2 dir = next - position;

                // Check that it's not the place the robot
                // is currently positioned.
                if (dir != Vector2.Zero)
                {
                    // Add direction to the movement queue.
                    Path.Enqueue(dir);
                }

                // Update the position to be the last checked
                // position.
                position = next;
            }
        }

        public override Queue<Vector2> GetPath()
        {
            // Get the neighbours of the current cell.
            IEnumerable<Vector2> neighbours = _robot.LocalGraph.UnvisitedNeighbours(_robot.GridPosition).ToList();

            // Check if there are unvisited neighbours.
            if (neighbours.Any())
            {
                // Temporary position for the robot path planning.
                Vector2 currentPosition = _robot.GridPosition;

                // Add each of the unvisited neighbours to the queue
                // if the robot is at the current parent.
                foreach (Vector2 neighbour in neighbours.Where(neighbour => _robot.GridPosition == _currentParent))
                {
                    // Add the movement to the queue
                    // with the return movement as well.
                    QueuePath(neighbour, currentPosition);

                    // Update the temporary position.
                    currentPosition = neighbour;

                    // Add the newly discovered cell
                    // to the parent queue.
                    _parentQueue.Enqueue(neighbour);
                }

                // Add a way back to the current parent - this is where the weird extra move issue is.
                QueuePath(_currentParent, currentPosition);
            }

            // There are no unvisited neighbours.
            else
            {
                // Are there anymore parent cells to check?
                if (_parentQueue.Any())
                {
                    // Get the next parent in the queue.
                    _currentParent = _parentQueue.Dequeue();

                    if (_robot.LocalGraph.UnvisitedNeighbours(_currentParent).Any())
                    {
                        // Get the path between the current position and the new parent.
                        QueuePath(_currentParent, _robot.GridPosition);
                    }
                }

                // The search must be over - return to the starting position.
                else
                {
                    // Check if the robot is back at the starting position.
                    if (_robot.GridPosition != _robot.StartPosition)
                    {
                        // Get the path between the current position and the new parent.
                        QueuePath(_robot.StartPosition, _robot.GridPosition);
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
    }
}

