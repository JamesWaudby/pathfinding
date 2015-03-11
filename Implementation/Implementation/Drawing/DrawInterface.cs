using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Implementation.Drawing
{
    class DrawInterface : IDrawing
    {
        private SpriteFont _font;

        public void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>("font");
        }

        public void Draw(SpriteBatch spriteBatch, Simulation simulation)
        {
            Robot robot = simulation.Robot;

            // Draw some debug information
            spriteBatch.DrawString(_font, "Pathfinding Robot Simulation!", new Vector2(10, 10), Color.Black);
            spriteBatch.DrawString(_font, "Status: " + robot.Status, new Vector2(10, 25), Color.Black);
            spriteBatch.DrawString(_font, "Search Type: " + robot.Search, new Vector2(10, 40), Color.Black);
            spriteBatch.DrawString(_font, "Current Position: " + robot.GridPosition, new Vector2(10, 55), Color.Black);
            spriteBatch.DrawString(_font, "Timer: " + simulation.Stopwatch.Elapsed, new Vector2(10, 70), Color.Black);

            spriteBatch.DrawString(_font, "Grid Size: " + simulation.Graph.Width + " x " + simulation.Graph.Height, new Vector2(10, 85), Color.Black);


            int t = 55;
            spriteBatch.DrawString(_font, "Current Parent:\n" + robot.Search.CurrentParent, new Vector2(simulation.Camera.ViewportWidth - 150, 10), Color.Black);

            if (robot.GetMoves() != null)
            {
                spriteBatch.DrawString(_font, "Parent Queue:", new Vector2(simulation.Camera.ViewportWidth - 150, 40), Color.Black);

                foreach (Vector2 v in robot.Search.ParentQueue)
                {
                    spriteBatch.DrawString(_font, v.ToString(), new Vector2(simulation.Camera.ViewportWidth - 150, t), Color.Black);
                    t += 15;
                }
            }

            // Draw the user instructions.
            spriteBatch.DrawString(_font, "WASD - Move Camera.\n" +
                                              "Q/E - Zoom Camera In/Out.\n" +
                                              "F - Move Camera to Robot.\n" +
                                              "R - Start the Robot.", new Vector2(10, simulation.Camera.ViewportHeight - 65), Color.Black);
        }
    }
}
