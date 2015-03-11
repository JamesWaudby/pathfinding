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

    //class PriorityQueue
    //{
    //    private List<Tuple<Vector2, int>> elements = new List<Tuple<Vector2, int>>();

    //    public int Count
    //    {
    //        get { return elements.Count; }
    //    }

    //    public void Enqueue(Vector2 item, int priority)
    //    {
    //        elements.Add(Tuple.Create(item, priority));
    //    }

    //    public Vector2 Dequeue()
    //    {
    //        int bestIndex = 0;

    //        for (int i = 0; i < elements.Count; i++)
    //        {
    //            if (elements[i].Item2 < elements[bestIndex].Item2)
    //                bestIndex = i;
    //        }

    //        Vector2 bestVector = elements[bestIndex].Item1;
    //        elements.RemoveAt(bestIndex);

    //        return bestVector;
    //    }
    //}

    class BreadthFirstAStar : Search
    {
        public BreadthFirstAStar(Robot robot) : base(robot)
        {
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
                foreach (Vector2 neighbour in neighbours)
                {
                    // Add the movement to the queue
                    // with the return movement as well.
                    QueuePath(AStar.GetPath(Robot, neighbour, currentPosition), currentPosition);
                    
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
                        QueuePath(AStar.GetPath(Robot, CurrentParent, Robot.GridPosition), Robot.GridPosition);
                    }
                }

                // The search must be over - return to the starting position.
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

            return Path;
        }

        public override string ToString()
        {
            return "Breadth First A* Search";
        }
    }
}