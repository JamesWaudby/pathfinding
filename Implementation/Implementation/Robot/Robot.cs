using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Implementation.GridRepresentation;
using Implementation.Searching;
using Microsoft.Xna.Framework;

namespace Implementation
{
    public class Robot
    {
        public enum RobotStatus
        {
            Idle, Active, Complete
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
        /// The position on the grid that the robot starts at.
        /// </summary>
        public Vector2 StartPosition { get; set; }

        /// <summary>
        /// The position the robot is currently trying to get to.
        /// </summary>
        private Vector2 _targetPosition;

        public RobotStatus Status { get; set; }

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
        private Queue<Vector2> _movementQueue;
        
        /// <summary>
        /// Debug code for getting the movement queue.
        /// </summary>
        /// <returns>Movement Queue.</returns>
        public Queue<Vector2> GetMoves()
        {
            return _movementQueue;
        }

        /// <summary>
        /// How far through a cell the robot has currently travelled.
        /// </summary>
        private float _distance;

        /// <summary>
        /// The robot's moving speed.
        /// </summary>
        private const int Speed = 5;

        public Search Search { get; set; }

        /// <summary>
        /// Creates a robot at the given grid position.
        /// </summary>
        /// <param name="startPosition">The location on the grid to position the robot.</param>
        /// <param name="worldGraph">The full world representation.</param>
        public Robot(Vector2 startPosition, Graph worldGraph)
        {
            // Store the starting position of the robot.
            StartPosition = startPosition;

            // Set the world representation.
            _worldGraph = worldGraph;

            // Configure the robot.
            Initialize();
        }

        private void Initialize()
        {
            // Do not start the robot as active.
            Status = RobotStatus.Idle;

            // Set the grid and world position.
            GridPosition = StartPosition;
            WorldPosition = GridPosition * _worldGraph.Resolution;

            // Create the queue for moving.
            _movementQueue = new Queue<Vector2>();

            _targetPosition = new Vector2();

            // Create the local graph and add the starting position to it.
            LocalGraph = new Graph(_worldGraph.Width, _worldGraph.Height);

            LocalGraph.Cells[(int)GridPosition.X, (int)GridPosition.Y] = new Cell(true)
            {
                Visited = 1
            };

            // Set the search mode.
            Search = new BreadthFirst(this);

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
            if (Status == RobotStatus.Idle || Status == RobotStatus.Complete) return;

            // Add neighbours to the local graph if needed.
            ScanCells();

            // Check if the queue is empty.
            if (!_movementQueue.Any())
            {
                // Populate the movement queue.
                _movementQueue = Search.GetPath();

                if(_movementQueue == null)
                    Status = RobotStatus.Complete;
            }
            else
            {
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
                if (LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Visited == 0)
                {
                    LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Parent = parent;
                }

                // Increase the visited count.
                LocalGraph.Cells[(int) GridPosition.X, (int) GridPosition.Y].Visited++;
            }

        }

        public void Reset()
        {
            //Initialize();

            // Do not start the robot as active.
            Status = RobotStatus.Idle;

            // Set the grid and world position.
            GridPosition = StartPosition;
            WorldPosition = GridPosition * _worldGraph.Resolution;

            // Create the queue for moving.
            _movementQueue = new Queue<Vector2>();
            _targetPosition = new Vector2();

            // Create the local graph and add the starting position to it.
            LocalGraph = new Graph(_worldGraph.Width, _worldGraph.Height);

            LocalGraph.Cells[(int)GridPosition.X, (int)GridPosition.Y] = new Cell(true)
            {
                Visited = 1
            };

            // Explore the immediate vicinity.
            ScanCells();
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
                
                // Set the parent on a walkable cell.
                // This is pre-emptive and only the first chance
                // to add parents. Will be updated upon first
                // actual visit.
                if(LocalGraph.Cells[x, y].Walkable)
                    LocalGraph.Cells[x, y].Parent = GridPosition;
            }
        }
    }
}
