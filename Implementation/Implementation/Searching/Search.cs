using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Implementation.Searching
{
    public abstract class Search
    {
        protected Robot Robot;
        protected Queue<Vector2> Path;

        /// <summary>
        /// Queue of vectors to represent the order in which the children
        /// must be visited.
        /// </summary>
        public Queue<Vector2> ParentQueue { get; protected set; }

        /// <summary>
        /// The current parent node to be visited.
        /// </summary>
        public Vector2 CurrentParent { get; protected set; }

        protected Search(Robot robot)
        {
            Path = new Queue<Vector2>();
            ParentQueue = new Queue<Vector2>();

            Robot = robot;
            CurrentParent = Robot.GridPosition;
        }

        /// <summary>
        /// Provides a path that will explore the grid.
        /// </summary>
        /// <returns>A queue of vectors represent the path to take.</returns>
        public abstract Queue<Vector2> GetPath();

        public void Setup(Robot robot)
        {
            Robot = robot;
            CurrentParent = Robot.GridPosition;
        }

        protected void QueuePath(IEnumerable<Vector2> path, Vector2 currentPosition)
        {
            // Cache the current position.
            Vector2 position = currentPosition;

            if (path != null)
            {
                foreach (Vector2 next in path)
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
        }
    }
}
