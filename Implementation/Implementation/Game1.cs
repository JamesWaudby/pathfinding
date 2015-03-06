#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.GridRepresentation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace Implementation
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteBatch _interfaceBatch;
        private SpriteFont _font;

        private Texture2D _walkableTexture2D;
        private Texture2D _unwalkableTexture2D;
        private Texture2D _robotTexture2D;

        private Texture2D _upArrowTexture2D;
        private Texture2D _leftArrowTexture2D;
        private Texture2D _downArrowTexture2D;
        private Texture2D _rightArrowTexture2D;

        private Input _input;

        public Camera Camera { get; set; }
        public Graph Graph { get; set; }
        public Robot Robot { get; set; }

        private readonly Vector2 _startPosition = new Vector2(2, 2);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Create the world graph.
            Graph = new Graph(4, 4);

            // Temporary for generating rooms.
            Random rand = new Random();
            for (int x = 0; x < Graph.Width; x++)
            {
                for (int y = 0; y < Graph.Height; y++)
                {
                    if (rand.Next(0, 10) >= 10) Graph.Cells[x,y]= new Cell(false);
                    else Graph.Cells[x, y] = new Cell(true);
                }
            }

            // Create the robot and set it's starting position.
            Robot = new Robot(_startPosition, Graph);

            // Create the camera.
            Camera = new Camera
            {
                ViewportWidth = _graphics.GraphicsDevice.Viewport.Width,
                ViewportHeight = _graphics.GraphicsDevice.Viewport.Height
            };

            // Focus the camera on the starting position.
            Camera.CenterOn(_startPosition);

            // Create new user input.
            _input = new Input(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _interfaceBatch = new SpriteBatch(GraphicsDevice);

           
            _font = Content.Load<SpriteFont>("font");

            _walkableTexture2D = Content.Load<Texture2D>("walkable");
            _unwalkableTexture2D = Content.Load<Texture2D>("nonwalkable");
            _robotTexture2D = Content.Load<Texture2D>("robot");

            _upArrowTexture2D = Content.Load<Texture2D>("up");
            _leftArrowTexture2D = Content.Load<Texture2D>("left");
            _downArrowTexture2D = Content.Load<Texture2D>("down");
            _rightArrowTexture2D = Content.Load<Texture2D>("right");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update the user input.
            _input.HandleInput();
            
            // Update the robot.
            Robot.Update(gameTime);
           
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.TranslationMatrix);

            // Cull the unneeded tiles from being drawn.
            int left = (int)(Camera.Position.X - (Camera.ViewportWidth / Camera.Zoom)) / Robot.LocalGraph.Resolution;
            int right = (int)(Camera.Position.X + (Camera.ViewportWidth / Camera.Zoom)) / Robot.LocalGraph.Resolution + 1;
            int top = (int)(Camera.Position.Y - (Camera.ViewportHeight / Camera.Zoom)) / Robot.LocalGraph.Resolution;
            int bottom = (int)(Camera.Position.Y + (Camera.ViewportHeight / Camera.Zoom)) / Robot.LocalGraph.Resolution + 1;

            // Make sure the coordinates are in bounds.
            if (left < 0) left = 0;
            if (top < 0) top = 0;
            if (right > Robot.LocalGraph.Width) right = Robot.LocalGraph.Width;
            if (bottom > Robot.LocalGraph.Height) bottom = Robot.LocalGraph.Height;

            // Loop through only the area that needs drawing.
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the positioning for the grid.
                    int xPos = (x * Graph.Resolution);
                    int yPos = (y * Graph.Resolution);

                    // Draw the existing grid.
                    _spriteBatch.Draw(Graph.Cells[x, y].Walkable ? _walkableTexture2D : _unwalkableTexture2D,
                        new Rectangle(xPos, yPos, Graph.Resolution, Graph.Resolution), Color.White * 0.4f);

                    // Draw the Robot's grid.
                    if (Robot.LocalGraph.Cells[x, y] != null)
                    {
                        // Draw the walkable area and the frontier.
                        if (Robot.LocalGraph.Cells[x, y].Walkable)
                        {
                            // Draw the ground.
                            _spriteBatch.Draw(_walkableTexture2D,
                                new Rectangle(xPos, yPos, Graph.Resolution, Graph.Resolution),
                                Robot.LocalGraph.Cells[x, y].Visited > 0 ? Color.White : Color.Aqua);

                            // Draw how many times each cell has been visited.
                            _spriteBatch.DrawString(_font, Robot.LocalGraph.Cells[x, y].Visited.ToString(),
                                new Vector2(xPos + 2, yPos), Color.Blue);
                        }
                        // Draw the non walkable areas.
                        else
                        {
                            _spriteBatch.Draw(_unwalkableTexture2D, new Vector2(xPos, yPos), Color.White);
                        }

                        // Create a new vector to check against.
                        Vector2 current = new Vector2(x, y);

                        // Draw arrows to the cell parents.
                        if (current - Robot.LocalGraph.Cells[x, y].Parent == Graph.Dirs[0])
                        {
                            _spriteBatch.Draw(_leftArrowTexture2D, new Vector2(xPos, yPos), Color.White);
                        }
                        else if (current - Robot.LocalGraph.Cells[x, y].Parent == Graph.Dirs[1])
                        {
                            _spriteBatch.Draw(_downArrowTexture2D, new Vector2(xPos, yPos), Color.White);
                        }
                        else if (current - Robot.LocalGraph.Cells[x, y].Parent == Graph.Dirs[2])
                        {
                            _spriteBatch.Draw(_rightArrowTexture2D, new Vector2(xPos, yPos), Color.White);
                        }
                        else if (current - Robot.LocalGraph.Cells[x, y].Parent == Graph.Dirs[3])
                        {
                            _spriteBatch.Draw(_upArrowTexture2D, new Vector2(xPos, yPos), Color.White);
                        }
                    }
                }
            }

            // Draw the Robot sprite.
            _spriteBatch.Draw(_robotTexture2D, Robot.WorldPosition, Color.White);
            _spriteBatch.End();

            // Draw the user interface overlay.
            _interfaceBatch.Begin();
            _interfaceBatch.DrawString(_font, "Pathfinding Robot Simulation!", new Vector2(10, 10), Color.Black);
            _interfaceBatch.DrawString(_font, Robot.GridPosition.ToString(), new Vector2(10, 25), Color.Black);
            _interfaceBatch.DrawString(_font, "Active: " + Robot.Active, new Vector2(10, 45), Color.Black);

            int t = 65;

            foreach (Vector2 v in Robot.GetMoves())
            {
                _interfaceBatch.DrawString(_font, v.ToString(), new Vector2(10, t), Color.Black);
                t += 12;
            }

            _interfaceBatch.DrawString(_font, "WASD - Move Camera.\n" +
                                              "Q/E - Zoom Camera In/Out.\n" +
                                              "F - Move Camera to Robot.\n" +
                                              "R - Start the Robot.", new Vector2(10, Camera.ViewportHeight - 65), Color.Black);
            _interfaceBatch.End();

            base.Draw(gameTime);
        }
    }
}
