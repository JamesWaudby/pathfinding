using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Implementation.GridRepresentation;
using Microsoft.Xna.Framework;

namespace Implementation
{
    public class Robot
    {
        /// <summary>
        /// Represents the different search algorithms
        /// that are going to be used.
        /// </summary>
        public enum SearchMode
        {
            Bfs,
            Dfs,
            Bfsa,
            Dfsa
        };

        /// <summary>
        /// The robot's position in grid space.
        /// </summary>
        public Vector2 GridPosition { get; set; }

        /// <summary>
        /// The robot's position in pixels.
        /// </summary>
        public Vector2 WorldPosition { get; set; }

        /// <summary>
        /// Whether or not the robot is currently active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The local representation of the grid that shows the robot's knowledge.
        /// </summary>
        public Graph LocalGraph { get; set; }

        /// <summary>
        /// The full underlying graph structure.
        /// </summary>
        private readonly Graph _worldGraph;

        /// <summary>
        /// Queue of vectors for the robot to move through.
        /// </summary>
        private readonly Queue<Vector2> _movementQueue;

        /// <summary>
        /// Queue of vectors to represent the order in which the children
        /// must be visited.
        /// </summary>
        private readonly Queue<Vector2> _parentQueue;
        
        /// <summary>
        /// The current parent node to be visited.
        /// </summary>
        private Vector2 _currentParent;

        /// <summary>
        /// The position on the grid that the robot starts at.
        /// </summary>
        private readonly Vector2 _startPosition;

        /// <summary>
        /// The position the robot is currently trying to get to.
        /// </summary>
        private Vector2 _targetPosition;

        /// <summary>
        /// How far through a cell the robot has currently travelled.
        /// </summary>
        private float _distance;

        /// <summary>
        /// The robot's moving speed.
        /// </summary>
        private const int Speed = 20;

        /// <summary>
        /// The current search algorithm that is being
        /// used.
        /// </summary>
        public SearchMode Mode { get; set; }

        /// <summary>
        /// Debug code for getting the movement queue.
        /// </summary>
        /// <returns>Movement Queue.</returns>
        public Queue<Vector2> GetMoves()
        {
            return _movementQueue;
        }


        /// <summary>
        /// Creates a robot at the given grid position.
        /// </summary>
        /// <param name="gridPosition">The location on the grid to position the robot.</param>
        /// <param name="worldGraph">The full world representation.</param>
        public Robot(Vector2 gridPosition, Graph worldGraph)
        {
            // Do not start the robot as active.
            Active = false;

            // Set the grid and world position.
            GridPosition = gridPosition;
            WorldPosition = GridPosition* worldGraph.Resolution;

            // Store the starting position of the robot.
            _startPosition = GridPosition;

            // Create the queue for moving.
            _movementQueue = new Queue<Vector2>();

            // Set the world representation.
            _worldGraph = worldGraph;

            // Create the local graph and add the starting position to it.
            LocalGraph = new Graph(_worldGraph.Width, _worldGraph.Height);

            LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y] = new Cell(true)
            {
                Visited = 1
            };

            // Set the search mode.
            Mode = SearchMode.Bfs;

            // Set up the BFS parent queue.
            _parentQueue = new Queue<Vector2>();
            //_parentQueue.Enqueue(GridPosition);
            
            _currentParent = GridPosition;

            // Explore the immediate vicinity.
            ScanCells();

        }

        /// <summary>
        /// Performs the update of the robot.
        /// </summary>
        /// <param name="gameTime">Used to make sure the robot updates at the correct time.</param>
        public void Update(GameTime gameTime)
        {
            // Check if the robot is currently active.
            if (!Active) return;

            // Add neighbours to the local graph if needed.
            ScanCells();

            if (_movementQueue.Count == 0)
            {
                // Check the search type
                switch (Mode)
                {
                    case SearchMode.Bfs:
                        // Perform a breadth-first search.
                        BreadthFirst();
                        break;

                    case SearchMode.Dfs:
                        // Perform a depth-first search.
                        DepthFirst();
                        break;

                    case SearchMode.Bfsa:
                        break;

                    case SearchMode.Dfsa:
                        break;
                }
            }
            else
            {
                // If there are moves to be made, make them.
                Move(gameTime);
            }
        }

        /// <summary>
        /// Makes the robot move a set amount per frame.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Move(GameTime gameTime)
        {
            // Get the next move to make.
            if (_targetPosition == Vector2.Zero)
                _targetPosition = _movementQueue.Peek();

            // Decide the direction to move in.
            Vector2 worldTarget = _targetPosition*_worldGraph.Resolution;

            // Get the amount to move.
            Vector2 movement = worldTarget * Speed * (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Update the world position.
            WorldPosition += movement;

            // Update the distance travelled.
            _distance += Math.Abs(movement.X) + Math.Abs(movement.Y);

                // Check if distance travelled is enough.
            if (_distance >= _worldGraph.Resolution)
            {
                _movementQueue.Dequeue();

                Vector2 parent = GridPosition;

                // Reset the distance.
                _distance = 0f;

                // Update the grid position.
                GridPosition += _targetPosition;

                // Snap to the grid.
                WorldPosition = GridPosition*_worldGraph.Resolution;

                // Stop moving.
                _targetPosition = Vector2.Zero;

                // Add the parent of the new cell.
                if (LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Parent == null &&
                    GridPosition != _startPosition &&
                    LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Visited == 0)
                {
                    LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Parent = parent;
                }

                // Increase the visited count.
                LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Visited++;
            }

        }

        /// <summary>
        /// Perform a Depth First Search.
        /// </summary>
        public void DepthFirst()
        {
            // Search for the unvisited neighbours.
            IEnumerable<Vector2> neighbours = GetUnvisitedNeighbours(GridPosition).ToList();

            // Check that there are neighbours.
            if (neighbours.Any())
            {
                // Move to the first unvisited cell.
                Vector2 next = neighbours.First() - GridPosition;

                // Add the next position to the movement queue - not a DFS queue!
                _movementQueue.Enqueue(next);
            }

            // If there are no unvisited cells, move back to the parent of the cell.
            else
            {
                // Check if we are back at the starting position - this signals the end.
                if (GridPosition != _startPosition)
                {
                    Vector2? next = (LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Parent - GridPosition);

                    if (next != null)
                        _movementQueue.Enqueue((Vector2) next);
                }
                else
                {
                    Active = false;
                }
            }
        }

        /// <summary>
        /// Perform a Breadth First Search.
        /// </summary>
        public void BreadthFirst()
        {
            // Get the neighbours of the current cell.
            IEnumerable<Vector2> neighbours = GetUnvisitedNeighbours(GridPosition).ToList();

            // Check if there are unvisited neighbours.
            if (neighbours.Any())
            {
                // Add each of the unvisited neighbours to the queue.
                foreach (Vector2 neighbour in neighbours)
                {
                    // Get the next position.
                    Vector2 next = neighbour - GridPosition;

                    // If the current position is the current parent queue more moves.
                    if (GridPosition == _currentParent)
                    {
                        // Add the movement to the queue
                        // with the return movement as well.
                        _movementQueue.Enqueue(next);

                        if(GetUnvisitedNeighbours(GridPosition).Any())
                            _movementQueue.Enqueue(-next);

                        // Add the newly discovered cell
                        // to the parent queue.
                        _parentQueue.Enqueue(neighbour);
                    }
                }
            }
            // There are no unvisited neighbours.
            else
            {
                // Are there anymore parent cells to check?
                if (_parentQueue.Any())
                {
                    // Get the next parent in the queue.
                    _currentParent = _parentQueue.Dequeue();

                    if (GetUnvisitedNeighbours(_currentParent).Any())
                    {
                        // Get the path between the current position and the new parent.
                        QueuePath(_currentParent);
                    }
                }

                // The search must be over - return to the starting position.
                else
                {
                    // Check if the robot is back at the starting position.
                    if (GridPosition == _startPosition)
                    {
                        Active = false;
                    }
                    // If not, move there.
                    else
                    {
                        // Get the path between the current position and the new parent.
                        QueuePath(_startPosition);
                    }
                }
            }
        }

        /// <summary>
        /// Get all of the unvisited neighbours of a position.
        /// </summary>
        /// <param name="from">The position to check from.</param>
        /// <returns>An IEnumerable of unvisited neighbours.</returns>
        private IEnumerable<Vector2> GetUnvisitedNeighbours(Vector2 from)
        {
            // Search for the unvisited neighbours.
            return LocalGraph.WalkableNeighbours(from).Where(n => LocalGraph.Cells[(int)n.X, (int)n.Y].Visited == 0);
        }

        /// <summary>
        /// Find a path from the current position to the target.
        /// </summary>
        /// <param name="to">Position to path to.</param>
        /// <returns>An IEnumerable of the represented path.</returns>
        private void QueuePath(Vector2 to)
        {
            // Get path to root for current.
            IEnumerable<Vector2> currentPath = LocalGraph.GetPath(GridPosition).ToList();

            // Get path to root for parent.
            IEnumerable<Vector2> parentPath = LocalGraph.GetPath(to).ToList();

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
            Vector2 position = GridPosition;

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
                    _movementQueue.Enqueue(dir);
                }

                // Update the position to be the last checked
                // position.
                position = next;
            }
        }

        /// <summary>
        /// Add newly checked cells to the Robot's local grid representation.
        /// </summary>
        private void ScanCells()
        {
            // Loop through each of the neighbours and update where the local cell is null - don't re-update the cells.
            foreach (Vector2 position in _worldGraph.Neighbours(GridPosition).Where(position => LocalGraph.Cells[(int)position.X, (int)position.Y] == null))
            {
                // Cast the vector position.
                int x = (int)position.X;
                int y = (int)position.Y;

                // Add a new cell depending on the walkability.
                LocalGraph.Cells[x, y] = _worldGraph.Cells[x, y].Walkable ? new Cell(true) : new Cell(false);
            }
        }
    }
}
