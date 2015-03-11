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

        public Simulation Simulation { get; set; }
        private List<Simulation> _simulations;
        private int _simulationNumber = 0;


        // Used for the Poster Presentation.
        private bool _demoMode = true;

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
            // Create the simulations.
            _simulations = new List<Simulation>
            {
                new Simulation(_graphics, 5, 5, 10, 100, new Vector2(2, 2)),
                new Simulation(_graphics, 5, 5, 10, 80, new Vector2(2, 2)),
                //new Simulation(_graphics, 5, 5, 55, 80, new Vector2(2, 2)),
                new Simulation(_graphics, 10, 10, 10, 100, new Vector2(4, 4)),
                new Simulation(_graphics, 10, 10, 10, 80, new Vector2(4, 4)),
                new Simulation(_graphics, 20, 20, 10, 100, new Vector2(9, 9)),
                new Simulation(_graphics, 20, 20, 10, 80, new Vector2(9, 9)),
                new Simulation(_graphics, 40, 40, 100, 100, new Vector2(19, 19)),
                new Simulation(_graphics, 40, 40, 100, 80, new Vector2(19, 19)),
                //new Simulation(_graphics, 100, 100, 100, 100, new Vector2(49, 49)),
                //new Simulation(_graphics, 100, 100, 100, 80, new Vector2(49, 49))
            };

            // Create new user input.
            _input = new Input();

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _interfaceBatch = new SpriteBatch(GraphicsDevice);

            // Creating the drawing objects.
            _drawGrid = new DrawGrid();
            _drawRobot = new DrawRobot();
            _drawInterface = new DrawInterface();

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
                    _simulations[_simulationNumber].Start();
                }
                else
                {
                    //  Keep the simulations looping indefinitely.
                    if (_demoMode)
                    {
                        _simulationNumber = 0;
                    }
                }
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
