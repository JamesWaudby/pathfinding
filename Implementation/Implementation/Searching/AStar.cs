using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Implementation.Searching
{
    class Node
    {
        public Vector2 Position { get; set; }

        // The estimated cost of getting from this node
        // to the end node.
        public int H { get; set; }

        // Total cost to get to the node.
        public int G { get; set; }

        // Priority of the node (g + h)
        // The lower the better.
        public int F
        {
            get { return G + H; }
        }

        public Node Parent { get; set; }

        public Node(Vector2 position)
        {
            Position = position;
            H = 0;
            G = 0;
            Parent = null;
        }
    }

    /// <summary>
    /// Provides the static methods needed to perform an A* search.
    /// </summary>
    class AStar
    {
        /// <summary>
        /// Provides the manhattan distance heuristic.
        /// </summary>
        /// <param name="a">First Position.</param>
        /// <param name="b">Second Position.</param>
        /// <returns>The heuristic value for the position.</returns>
        private static int Heuristic(Vector2 a, Vector2 b)
        {
            return (int)(Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y));
        }

        /// <summary>
        /// Produces a path through the use of the A* pathfinding algorithm.
        /// </summary>
        /// <param name="robot">The robot being used.</param>
        /// <param name="to">Where the robot is headed.</param>
        /// <param name="from">Where the robot is moving from.</param>
        /// <returns>An IEnumberable of the path to the desired location.</returns>
        public static IEnumerable<Vector2> GetPath(Robot robot, Vector2 to, Vector2 from)
        {
            List<Node> openList = new List<Node>();
            List<Node> closedList = new List<Node>();

            // Create the grid of nodes to check from.
            Node[,] grid = new Node[robot.LocalGraph.Width, robot.LocalGraph.Height];
            for (int x = 0; x < robot.LocalGraph.Width; x++)
            {
                for (int y = 0; y < robot.LocalGraph.Height; y++)
                {
                    grid[x, y] = new Node(new Vector2(x, y));
                }
            }

            // Add the starting node to the open list.
            openList.Add(grid[(int)from.X, (int)from.Y]);

            // If there are still nodes to search continue.
            while (openList.Count > 0)
            {
                // The current lowest index
                int lowestIndex = 0;

                // Find the lowest f(x)
                for (int i = 0; i < openList.Count; i++)
                {
                    // If this F value is lower than the current lowest.
                    if (openList[i].F < openList[lowestIndex].F)
                    {
                        lowestIndex = i;
                    }
                }

                // Get the lowest node.
                Node current = openList[lowestIndex];

                // Goal state.
                if (current.Position == to)
                {
                    // Rebuild the path taken to get here.
                    Node cur = current;
                    Queue<Vector2> path = new Queue<Vector2>();

                    while (cur.Parent != null)
                    {
                        path.Enqueue(cur.Position);
                        cur = cur.Parent;
                    }

                    return path.Reverse();
                }

                // Normal case.
                openList.Remove(current);
                closedList.Add(current);

                // Look at each of the neighbours and assigned the correct values.
                foreach (Vector2 next in robot.LocalGraph.WalkableNeighbours(current.Position))
                {
                    Node neighbour = grid[(int)next.X, (int)next.Y];

                    // If the node is in the closed list dont proceed.
                    if (closedList.Contains(neighbour))
                    {
                        continue;
                    }

                    // Make sure the algorithm values explored cells over unexplored ones.
                    int g = current.G + 1;

                    if (robot.LocalGraph.Cells[(int) next.X, (int) next.Y].Visited == 0)
                        g += 100;

                    bool isBest = false;

                    // If the current node isn't in the open list.
                    if (!openList.Contains(neighbour))
                    {
                        // First time this node has been visited.
                        isBest = true;

                        // Work out the H cost.
                        neighbour.H = Heuristic(neighbour.Position, to);

                        // Add node to the open list.
                        openList.Add(neighbour);
                    }
                    // The node has been seen but the score is better this time
                    else if (g < neighbour.G)
                    {
                        isBest = true;
                    }

                    // If the best is true we've found the best path to that tile.
                    if (isBest)
                    {
                        neighbour.Parent = current;
                        neighbour.G = g;
                    }
                }
            }
            // No path was found.
            return null;
        }
    }
}
