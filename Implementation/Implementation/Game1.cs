#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Implementation.Drawing;
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
        private Input _input;

        // The drawing objects.
        private DrawGrid _drawGrid;
        private DrawRobot _drawRobot;
        private DrawInterface _drawInterface;

        public Camera Camera { get; set; }
        public Graph Graph { get; set; }
        public Robot Robot { get; set; }
        public Simulation Simulation { get; set; }

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
            Graph = new Graph(5, 5);

            // Temporary for generating rooms.
            Random rand = new Random();
            for (int x = 0; x < Graph.Width; x++)
            {
                for (int y = 0; y < Graph.Height; y++)
                {
                    if (rand.Next(0, 10) >= 8) Graph.Cells[x,y]= new Cell(false);
                    else Graph.Cells[x, y] = new Cell(true);
                }
            }

            // Create the starting position.
            Vector2 startPosition = new Vector2(2, 2);

            // Create the robot and set it's starting position.
            Robot = new Robot(startPosition, Graph);

            // Create the camera.
            Camera = new Camera(_graphics.GraphicsDevice.Viewport.Width,
                                _graphics.GraphicsDevice.Viewport.Height);

            // Focus the camera on the starting position.
            Camera.CenterOn(startPosition);

            // Create the simulation.
            Simulation = new Simulation(this);

            // Create new user input.
            _input = new Input(this);

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _interfaceBatch = new SpriteBatch(GraphicsDevice);

            // Creating the drawing objects.
            _drawGrid = new DrawGrid(this);
            _drawRobot = new DrawRobot(this);
            _drawInterface = new DrawInterface(this);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _drawGrid.LoadContent(Content);
            _drawRobot.LoadContent(Content);
            _drawInterface.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
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

            // Update the Simulation
            Simulation.Update();
           
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            #region Simulation Drawing.

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.TranslationMatrix);

            // Draw both the world grid and the robot local grid.
            _drawGrid.Draw(_spriteBatch);

            // Draw the robot.
            _drawRobot.Draw(_spriteBatch);

            _spriteBatch.End();

            #endregion

            #region User Interface Drawing.

            // Draw the user interface overlay.
            _interfaceBatch.Begin();
            _drawInterface.Draw(_interfaceBatch);
            _interfaceBatch.End();

            #endregion

            base.Draw(gameTime);
        }
    }
}
