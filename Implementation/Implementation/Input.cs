using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Implementation.GridRepresentation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Implementation
{
    class Input
    {
        private readonly Camera _camera;
        private readonly Robot _robot;
        private readonly Simulation _simulation;

        public Input(Game1 game)
        {
            _camera = game.Camera;
            _robot = game.Robot;
            _simulation = game.Simulation;
        }

        public void HandleInput()
        {
            KeyboardState state = Keyboard.GetState();

            Vector2 cameraMovement = Vector2.Zero;

            // Move left.
            if (state.IsKeyDown(Keys.A))
            {
                cameraMovement.X = -1;
            }
            // Move right.
            else if (state.IsKeyDown(Keys.D))
            {
                cameraMovement.X = 1;
            }
            // Move up.
            if (state.IsKeyDown(Keys.W))
            {
                cameraMovement.Y = -1;
            }
            // Move down.
            else if (state.IsKeyDown(Keys.S))
            {
                cameraMovement.Y = 1;
            }
            // Zoom in.
            if (state.IsKeyDown(Keys.Q))
            {
                _camera.AdjustZoom(0.1f);
            }
            // Zoom out.
            else if (state.IsKeyDown(Keys.E))
            {
                _camera.AdjustZoom(-0.1f);
            }
            // Center on the robot.
            else if (state.IsKeyDown(Keys.F))
            {
                _camera.CenterOn(_robot.GridPosition);
            }
            // Start/Stop the robot.
            else if (state.IsKeyDown(Keys.R))
            {
                _simulation.Start();
            }

            // When using a controller, to match the thumbstick behavior,
            // we need to normalize non-zero vectors in case the user
            // is pressing a diagonal direction.
            if (cameraMovement != Vector2.Zero)
            {
                cameraMovement.Normalize();
            }

            // Scale movement to move 16 pixels per second
            cameraMovement *= 16;

            _camera.MoveCamera(cameraMovement, true);
        }
    }
}
