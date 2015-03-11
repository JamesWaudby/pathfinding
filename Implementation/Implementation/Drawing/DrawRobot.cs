using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Implementation.Drawing
{
    class DrawRobot : IDrawing
    {
        private Texture2D _robotTexture2D;

        public void LoadContent(ContentManager content)
        {
            _robotTexture2D = content.Load<Texture2D>("robot");
        }

        public void Draw(SpriteBatch spriteBatch, Simulation simulation)
        {
            // Draw the Robot sprite.
            spriteBatch.Draw(_robotTexture2D, simulation.Robot.WorldPosition, Color.Orchid);
        }
    }
}
