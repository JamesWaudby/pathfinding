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
        public Vector2 GridPosition { get; set; }
        public Vector2 WorldPosition { get; set; }
        public bool Active { get; set; }
        public Graph LocalGraph { get; set; }

        private readonly Graph _worldGraph;
        private readonly Queue<Vector2> _movementQueue;
        private readonly Vector2 _startPosition;

        private Vector2 _targetPosition;
        private float _distance;
        private const int Speed = 2;
        private bool _moving;

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

            // Explore the local vicinity.
            ScanCells();

        }

        public void Update(GameTime gameTime)
        {
            if (Active)
            {
                if (!_moving)
                {
                    // Add neighbours to the local graph if needed.
                    ScanCells();

                    // Perform a depth-first search
                    DepthFirst();

                    // Allow the robot to move next update.
                    _moving = true;
                }
                else
                {
                    Move(gameTime);
                }
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
                if (LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Parent == Vector2.Zero &&
                    GridPosition != _startPosition && LocalGraph.Cells[(int)GridPosition.X, (int)GridPosition.Y].Visited == 0)
                {
                    LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Parent = parent;
                }

                // Increase the visited count.
                LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Visited++;

                _moving = false;
            }
        }

        public void DepthFirst()
        {
            // Search for the unvisited neighbours.
            IEnumerable<Vector2> neighbours = LocalGraph.WalkableNeighbours(GridPosition)
                    .Where(n => LocalGraph.Cells[(int)n.X, (int)n.Y].Visited == 0);

            // Check that there are neighbours.
            if (neighbours.Any())
            {
                // Move to the first unvisited cell.
                Vector2 next = neighbours.First() - GridPosition;
                _movementQueue.Enqueue(next);
            }

            // If there are no unvisited cells, move back to the parent of the cell.
            else
            {
                if (GridPosition != _startPosition)
                    _movementQueue.Enqueue(LocalGraph.Cells[(int)GridPosition.X, (int)GridPosition.Y].Parent - GridPosition);
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


        /// <summary>
        /// Add newly checked cells to the Robot's local grid representation.
        /// </summary>
        public void ScanCells()
        {
            // Loop through each of the neighbours and update where the local cell is null - don't re-update the cells.
            foreach (Vector2 position in _worldGraph.Neighbours(GridPosition).Where(position => LocalGraph.Cells[(int) position.X, (int) position.Y] == null))
            {
                // Cast the vector position.
                int x = (int) position.X;
                int y = (int) position.Y;

                // Add a new cell depending on the walkability.
                LocalGraph.Cells[x, y] = _worldGraph.Cells[x,y].Walkable ? new Cell(true) : new Cell(false);
            }
        }
    }
}
