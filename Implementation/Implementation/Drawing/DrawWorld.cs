using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Implementation.GridRepresentation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Implementation.Drawing
{
    class DrawWorld : IDrawing
    {

        private Texture2D _walkableTexture2D;
        private Texture2D _unwalkableTexture2D;

        public void LoadContent(ContentManager content)
        {
            _walkableTexture2D = content.Load<Texture2D>("walkable");
            _unwalkableTexture2D = content.Load<Texture2D>("nonwalkable");
        }

        public void Draw(SpriteBatch spriteBatch, Simulation simulation)
        {
            Camera.Camera camera = simulation.Camera;
            Robot robot = simulation.Robot;
            Graph graph = simulation.Graph;

            // Cull the unneeded tiles from being drawn.
            int left = (int)(camera.Position.X - (camera.ViewportWidth / camera.Zoom)) / robot.LocalGraph.Resolution;
            int right = (int)(camera.Position.X + (camera.ViewportWidth / camera.Zoom)) / robot.LocalGraph.Resolution + 1;
            int top = (int)(camera.Position.Y - (camera.ViewportHeight / camera.Zoom)) / robot.LocalGraph.Resolution;
            int bottom = (int)(camera.Position.Y + (camera.ViewportHeight / camera.Zoom)) / robot.LocalGraph.Resolution + 1;

            // Make sure the coordinates are in bounds.
            if (left < 0) left = 0;
            if (top < 0) top = 0;
            if (right > robot.LocalGraph.Width) right = robot.LocalGraph.Width;
            if (bottom > robot.LocalGraph.Height) bottom = robot.LocalGraph.Height;

            // Loop through only the area that needs drawing.
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the positioning for the grid.
                    int xPos = (x * graph.Resolution);
                    int yPos = (y * graph.Resolution);

                    // Create a new vector to check against.
                    Vector2 current = new Vector2(x, y);

                    // Draw the existing grid.
                    spriteBatch.Draw(graph.Cells[x, y].Walkable ? _walkableTexture2D : _unwalkableTexture2D,
                        new Rectangle(xPos, yPos, graph.Resolution, graph.Resolution), Color.White);

                }
            }
        }
    }
}
