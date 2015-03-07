using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Implementation.Drawing
{
    class DrawInterface : IDraw
    {
        private SpriteFont _font;
        private readonly Robot _robot;
        private readonly Simulation _simulation;

        private readonly int _screenHeight;
        private int _screenWidth;

        public DrawInterface(Game1 game)
        {
            _robot = game.Robot;
            _simulation = game.Simulation;

            _screenHeight = game.Camera.ViewportHeight;
            _screenWidth = game.Camera.ViewportWidth;
        }

        public void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>("font");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw some debug information
            spriteBatch.DrawString(_font, "Pathfinding Robot Simulation!", new Vector2(10, 10), Color.Black);
            spriteBatch.DrawString(_font, _robot.GridPosition.ToString(), new Vector2(10, 25), Color.Black);
            spriteBatch.DrawString(_font, "Status: " + _robot.Status.ToString(), new Vector2(10, 45), Color.Black);
            spriteBatch.DrawString(_font, "Timer: " + _simulation.Stopwatch.Elapsed.ToString(), new Vector2(10, 65), Color.Black);

            int t = 85;

            if (_robot.GetMoves() != null)
            {
                foreach (Vector2 v in _robot.GetMoves())
                {
                    spriteBatch.DrawString(_font, v.ToString(), new Vector2(10, t), Color.Black);
                    t += 15;
                }
            }

            // Draw the user instructions.
            spriteBatch.DrawString(_font, "WASD - Move Camera.\n" +
                                              "Q/E - Zoom Camera In/Out.\n" +
                                              "F - Move Camera to Robot.\n" +
                                              "R - Start the Robot.", new Vector2(10, _screenHeight - 65), Color.Black);
        }
    }
}
