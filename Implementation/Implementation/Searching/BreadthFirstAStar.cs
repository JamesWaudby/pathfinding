using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Implementation.Searching
{
    // http://www.redblobgames.com/pathfinding/a-star/implementation.html#sec-1-4 - Tutorial.
    // http://homepages.abdn.ac.uk/f.guerin/pages/teaching/CS1015/practicals/aStarTutorial.htm - Another tutorial.
    // http://www.briangrinstead.com/blog/astar-search-algorithm-in-javascript - Another tutorial.

    class PriorityQueue
    {
        private List<Tuple<Vector2, int>> elements = new List<Tuple<Vector2, int>>();

        public int Count
        {
            get { return elements.Count; }
        }

        public void Enqueue(Vector2 item, int priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public Vector2 Dequeue()
        {
            int bestIndex = 0;

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Item2 < elements[bestIndex].Item2)
                    bestIndex = i;
            }

            Vector2 bestVector = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);

            return bestVector;
        }
    }

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

    class BreadthFirstAStar : Search
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

        //public Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
        //public Dictionary<Vector2, int> costSoFar = new Dictionary<Vector2, int>();

        public BreadthFirstAStar(Robot robot) : base(robot)
        {
            _parentQueue = new Queue<Vector2>();
            _currentParent = _robot.GridPosition;
        }

        private Queue<Vector2> AStar(Vector2 goal, Vector2 start)
        {
            Queue<Vector2> path = new Queue<Vector2>();

            return path;
        } 

        /// <summary>
        /// Manhattan
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int Heuristic(Vector2 a, Vector2 b)
        {
            return (int) (Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y));
        }

        //private Queue<Vector2> AStar(Vector2 to, Vector2 from)
        //{
        //    List<Node> openList = new List<Node>();
        //    List<Node> closedList = new List<Node>();

        //    // Create the grid of nodes to check from.
        //    Node[,] grid = new Node[_robot.LocalGraph.Width, _robot.LocalGraph.Height];
        //    for (int x = 0; x < _robot.LocalGraph.Width; x++)
        //    {
        //        for (int y = 0; y < _robot.LocalGraph.Height; y++)
        //        {
        //            grid[x, y] = new Node(new Vector2(x, y));
        //        }
        //    }

        //    // Add the starting node to the open list.
        //    openList.Add(new Node(from));

        //    // If there are still nodes to search continue.
        //    while (openList.Count > 0)
        //    {
        //        int lowestIndex = 0;

        //        // Find the lowest f(x)
        //        for(int i = 0; i < openList.Count; i++)
        //        {
        //            if (openList[i].F < openList[lowestIndex].F)
        //            {
        //                lowestIndex = i;
        //            }
        //        }

        //        // Get the lowest node.
        //        Node current = openList[lowestIndex];

        //        // Goal state.
        //        if (current.Position == to)
        //        {
        //            Node cur = current;
        //            Queue<Vector2> path = new Queue<Vector2>();

        //            while (cur.Parent != null)
        //            {
        //                path.Enqueue(cur.Position);
        //                cur = current.Parent;
        //            }

        //            return path;
        //        }

        //        // Normal case.
        //        openList.Remove(current);
        //        closedList.Add(current);

        //        foreach (Vector2 next in _robot.LocalGraph.WalkableNeighbours(current.Position))
        //        {
        //            Node neighbour = grid[(int) next.X, (int) next.Y];

        //            // If the node is in the closed list dont proceed.
        //            if (closedList.Contains(neighbour))
        //            {
        //                continue;
        //            }

        //            int g = current.G + 1;
        //            bool isBest = false;

        //            // If the current node isn't in the open list.
        //            if (!openList.Contains(neighbour))
        //            {
        //                // First time this node has been visited.
        //                isBest = true;

        //                // Work out the H cost.
        //                neighbour.H = Heuristic(neighbour.Position, to);

        //                // Add node to the open list.
        //                openList.Add(neighbour);
        //            }
        //            // The node has been seen but the score is better this time
        //            else if (g < neighbour.G)
        //            {
        //                isBest = true;
        //            }

        //            if (isBest)
        //            {
        //                neighbour.Parent = current;
        //                neighbour.G = g;
        //            }
        //        }
        //    }
        //    return null;
        //}

        //private void AStar(Vector2 to, Vector2 from)
        //{
        //    PriorityQueue frontier = new PriorityQueue();
        //    frontier.Enqueue(from, 0);
            
        //    cameFrom.Add(from, from);

        //    if(!costSoFar.ContainsKey(from)) 
        //        costSoFar.Add(from, 0);

        //    while (frontier.Count > 0)
        //    {
        //        Vector2 current = frontier.Dequeue();

        //        if (current.Equals(to))
        //        {
        //            break;
        //        }

        //        foreach (Vector2 next in _robot.LocalGraph.WalkableNeighbours(current))
        //        {
        //            int newCost = costSoFar[current] + 1;

        //            if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
        //            {
        //                costSoFar.Add(next, newCost);
        //                int priority = newCost + Heuristic(next, to);
        //                frontier.Enqueue(next, priority);
        //                cameFrom.Add(next, current);
        //            }
        //        }
        //    }

        //    Path.Enqueue(frontier.Dequeue());
        //}

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
                    Queue<Vector2> path =  AStar(neighbour, currentPosition);

                    // Cache the current position.
                    Vector2 position = _robot.GridPosition;

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

                    // Update the temporary position.
                    currentPosition = neighbour;

                    // Add the newly discovered cell
                    // to the parent queue.
                    _parentQueue.Enqueue(neighbour);
                }

                // Add a way back to the current parent - this is where the weird extra move issue is.
                AStar(_currentParent, currentPosition);
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
                        Queue<Vector2> path = AStar(_currentParent, _robot.GridPosition);

                        foreach (Vector2 next in path)
                        {
                            Path.Enqueue(next);
                        }
                    }
                }

                    // The search must be over - return to the starting position.
                else
                {
                    // Check if the robot is back at the starting position.
                    if (_robot.GridPosition != _robot.StartPosition)
                    {
                        // Get the path between the current position and the new parent.
                        Queue<Vector2> path = AStar(_robot.StartPosition, _robot.GridPosition);

                        foreach (Vector2 next in path)
                        {
                            Path.Enqueue(next);
                        }
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