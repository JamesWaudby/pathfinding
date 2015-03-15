#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Implementation.Drawing;
using Implementation.GridRepresentation;
using Implementation.UserInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        private Input _input;

        // The drawing objects.
        private DrawGrid _drawGrid;
        private DrawRobot _drawRobot;
        private DrawInterface _drawInterface;
        private DrawWorld _drawWorld;

        public Simulation Simulation { get; set; }
        private List<Simulation> _simulations;
        private int _simulationNumber = 0;


        // Used for the Poster Presentation.
        private const bool DemoMode = true;

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
            _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            // Create the simulations.
            _simulations = new List<Simulation>();

            for (int i = 0; i < 6; i++)
            {
                int size = 5 + (i * 5);
                _simulations.Add(new Simulation(_graphics, size, size, 2048, 100, new Vector2(size / 2, size / 2)));
                _simulations.Add(new Simulation(_graphics, size, size, 2048, 60, new Vector2(size / 2, size / 2)));
            }

            // Create new user input.
            _input = new Input();

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _interfaceBatch = new SpriteBatch(GraphicsDevice);

            // Creating the drawing objects.
            _drawGrid = new DrawGrid();
            _drawRobot = new DrawRobot();
            _drawInterface = new DrawInterface();
            _drawWorld = new DrawWorld();

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
            _drawWorld.LoadContent(Content);
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
            _input.HandleInput(_simulations[_simulationNumber]);
            
            // Update the Simulation
            if (!_simulations[_simulationNumber].Complete)
            {
                _simulations[_simulationNumber].Update(gameTime);
            }
            else
            {
                // Reset the current simulation for next time.
                _simulations[_simulationNumber].Reset();

                if (_simulationNumber < _simulations.Count() - 1)
                {
                    _simulationNumber++;
                }
                else
                {
                    //  Keep the simulations looping indefinitely.
                    if (DemoMode)
                    {
                        _simulationNumber = 0;
                    }
                }
                _simulations[_simulationNumber].Start();
            }

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

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _simulations[_simulationNumber].Camera.TranslationMatrix);

            // Draw both the world grid and the robot local grid.
            _drawGrid.Draw(_spriteBatch, _simulations[_simulationNumber]);
            //_drawWorld.Draw(_spriteBatch, _simulations[_simulationNumber]);

            // Draw the robot.
            _drawRobot.Draw(_spriteBatch, _simulations[_simulationNumber]);

            _spriteBatch.End();

            #endregion

            #region User Interface Drawing.

            // Draw the user interface overlay.
            _interfaceBatch.Begin();
            _drawInterface.Draw(_interfaceBatch, _simulations[_simulationNumber]);
            _interfaceBatch.End();

            #endregion

            base.Draw(gameTime);
        }

    }
}
