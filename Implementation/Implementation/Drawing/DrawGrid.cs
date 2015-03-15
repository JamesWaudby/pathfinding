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
    class DrawGrid : IDrawing
    {
        private SpriteFont _font;

        private Texture2D _walkableTexture2D;
        private Texture2D _unwalkableTexture2D;

        private Texture2D _upArrowTexture2D;
        private Texture2D _leftArrowTexture2D;
        private Texture2D _downArrowTexture2D;
        private Texture2D _rightArrowTexture2D;

        public void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>("font");

            _walkableTexture2D = content.Load<Texture2D>("walkable");
            _unwalkableTexture2D = content.Load<Texture2D>("nonwalkable");

            _upArrowTexture2D = content.Load<Texture2D>("up");
            _leftArrowTexture2D = content.Load<Texture2D>("left");
            _downArrowTexture2D = content.Load<Texture2D>("down");
            _rightArrowTexture2D = content.Load<Texture2D>("right");
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
                        new Rectangle(xPos, yPos, graph.Resolution, graph.Resolution), Color.White * 0.4f);

                    // Draw the Robot's grid.
                    if (robot.LocalGraph.Cells[x, y] != null)
                    {
                        // Draw the walkable area and the frontier.
                        if (robot.LocalGraph.Cells[x, y].Walkable)
                        {
                            if (robot.Search.CurrentParent == current)
                            {
                                // Draw the ground.
                                spriteBatch.Draw(_walkableTexture2D, new Rectangle(xPos, yPos, graph.Resolution, graph.Resolution), Color.PaleGreen);
                            }
                            else
                            {
                                // Draw the ground.
                                spriteBatch.Draw(_walkableTexture2D,
                                    new Rectangle(xPos, yPos, graph.Resolution, graph.Resolution),
                                    robot.LocalGraph.Cells[x, y].Visited > 0 ? Color.White : Color.Aqua);
                              

                            }

                            // Draw how many times each cell has been visited.
                            spriteBatch.DrawString(_font, robot.LocalGraph.Cells[x, y].Visited.ToString(CultureInfo.InvariantCulture),
                                new Vector2(xPos + 2, yPos), Color.Blue);
                        }
                        // Draw the non walkable areas.
                        else
                        {
                            spriteBatch.Draw(_unwalkableTexture2D, new Vector2(xPos, yPos), Color.White);
                        }

                        // Draw arrows to the cell parents.
                        if (current - robot.LocalGraph.Cells[x, y].Parent == Graph.Dirs[0])
                        {
                            spriteBatch.Draw(_downArrowTexture2D, new Vector2(xPos, yPos), Color.White);
                        }
                        else if (current - robot.LocalGraph.Cells[x, y].Parent == Graph.Dirs[1])
                        {
                            spriteBatch.Draw(_rightArrowTexture2D, new Vector2(xPos, yPos), Color.White);
                        }
                        else if (current - robot.LocalGraph.Cells[x, y].Parent == Graph.Dirs[2])
                        {
                            spriteBatch.Draw(_upArrowTexture2D, new Vector2(xPos, yPos), Color.White);
                        }
                        else if (current - robot.LocalGraph.Cells[x, y].Parent == Graph.Dirs[3])
                        {
                            spriteBatch.Draw(_leftArrowTexture2D, new Vector2(xPos, yPos), Color.White);
                        }
                    }
                }
            }
        }
    }
}
