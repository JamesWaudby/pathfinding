using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Implementation.UserInput
{
    class Input
    {
        public void HandleInput(Simulation simulation)
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
                simulation.Camera.AdjustZoom(0.1f);
            }
            // Zoom out.
            else if (state.IsKeyDown(Keys.E))
            {
                simulation.Camera.AdjustZoom(-0.1f);
            }
            // Center on the robot.
            else if (state.IsKeyDown(Keys.F))
            {
                simulation.Camera.CenterOn(simulation.Robot.GridPosition);
            }
            // Start/Stop the simulation.
            else if (state.IsKeyDown(Keys.R))
            {
                simulation.Start();
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

            simulation.Camera.MoveCamera(cameraMovement, true);
        }
    }
}
