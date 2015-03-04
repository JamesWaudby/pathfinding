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

        private readonly Queue<Vector2> _parentQueue;
        private Vector2 _currentParent;

        /// <summary>
        /// The position on the grid that the robot starts at.
        /// </summary>
        private readonly Vector2 _startPosition;

        private Vector2 _targetPosition;

        /// <summary>
        /// How far through a cell the robot has currently travelled.
        /// </summary>
        private float _distance;

        /// <summary>
        /// The robot's moving speed.
        /// </summary>
        private const int Speed = 2;

        /// <summary>
        /// Whether or not the robot is currently moving.
        /// </summary>
        private bool _moving;

        public SearchMode Mode { get; set; }

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

        public void Update(GameTime gameTime)
        {
            // Check if the robot is currently active.
            if (!Active) return;

            // Add neighbours to the local graph if needed.
            ScanCells();

            if (!_moving)
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


                // Allow the robot to move next update.
                _moving = true;
            }
            // Move the robot on the next turn.
            else
            {
                Move(gameTime);
            }
        }

        public void Move(GameTime gameTime)
        {
            // Get the next move to make.
            if(_targetPosition == Vector2.Zero && _movementQueue.Count > 0)
                _targetPosition = _movementQueue.Dequeue();

            // Decide the direction to move in.
            Vector2 worldTarget = (_targetPosition *_worldGraph.Resolution);

            // Get the amount to move.
            Vector2 movement = worldTarget* Speed * (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Update the world position.
            WorldPosition += movement;

            // Update the distance travelled.
            _distance += Math.Abs(movement.X) + Math.Abs(movement.Y);

            // Check if distance travelled is enough.
            if (_distance >= _worldGraph.Resolution)
            {
                Vector2 parent = GridPosition;

                // Reset the distance.
                _distance = 0f;

                // Update the grid position.
                GridPosition += _targetPosition;

                // Snap to the grid.
                WorldPosition = GridPosition*_worldGraph.Resolution;

                // Stop moving.
                _targetPosition = _movementQueue.Count > 0 ? _movementQueue.Dequeue() : Vector2.Zero;

                // Add the parent of the new cell.
                if (LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Parent == null &&
                    GridPosition != _startPosition && LocalGraph.Cells[(int)GridPosition.X, (int)GridPosition.Y].Visited == 0)
                {
                    LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Parent = parent;
                }

                // Increase the visited count.
                LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Visited++;

                //// Add neighbours to the local graph if needed.
                //ScanCells();

                // Check whether there are more moves to make.
                if (_movementQueue.Count <= 0)
                {
                    _moving = false;
                }
            }
        }

        public void DepthFirst()
        {
            // Search for the unvisited neighbours.
            IEnumerable<Vector2> neighbours = GetUnvisitedNeighbours().ToList();

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
            }
        }

        //public void BreadthSearch(GameTime gameTime)
        //{
        //    Queue<Cell> frontier = new Queue<Cell>();
        //    frontier.Enqueue(new Cell(GridPosition));

        //    Cell[,] cameFrom = new Cell[_worldGraph.Width,_worldGraph.Height];
        //    cameFrom[(int)GridPosition.X, (int)GridPosition.Y] = null;

        //    while (frontier.Count > 0)
        //    {
        //        Cell current = frontier.Dequeue();

        //        foreach (Cell next in _worldGraph.Neighbours(current.Position))
        //        {
        //            if (cameFrom[(int) next.Position.X, (int) next.Position.Y] == null)
        //            {
        //                frontier.Enqueue(next);
        //                cameFrom[(int) next.Position.X, (int) next.Position.Y] = current;
        //            }
        //        }
        //    }
        //}

        public void BreadthFirst()
        {
            // Get the neighbours of the current cell.
            IEnumerable<Vector2> neighbours = GetUnvisitedNeighbours().ToList();

            // Check if there are unvisited neighbours.
            if (neighbours.Any())
            {
                Vector2 next = neighbours.First() - GridPosition;

                // If the current position is the current parent queue more moves.
                if (GridPosition == _currentParent)
                {
                    _movementQueue.Enqueue(next);
                    _movementQueue.Enqueue(-next);

                    _parentQueue.Enqueue(neighbours.First());
                }
            }
            // There are no unvisited neighbours.
            else
            {
                if (GridPosition != _startPosition)
                {
                    Vector2? next = LocalGraph.Cells[(int)GridPosition.X, (int)GridPosition.Y].Parent;
                    if (next != null)
                        _currentParent = (Vector2) next;
                }
                else
                {
                    // Get the new parent
                    _currentParent = _parentQueue.Dequeue();
                }

                _movementQueue.Enqueue(_currentParent - GridPosition);
            }
        }

        private IEnumerable<Vector2> GetUnvisitedNeighbours()
        {
            // Search for the unvisited neighbours.
            return LocalGraph.WalkableNeighbours(GridPosition).Where(n => LocalGraph.Cells[(int)n.X, (int)n.Y].Visited == 0);
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
