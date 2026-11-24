using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;


namespace GameOfLife
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		public Point _screenSize = new Point(1024, 1024);	// The size of the window in pixels.
		public Point _gridSize = new Point(32, 32);			// The number of cells the window is divided up into.
		public Texture2D _cellTxr, _backTxr;				// Images loaded from disk.
		private Point _mouseGrid;							// The current location of the mouse pointer on the grid.
		private Point _mouseGridPrev;						// The gird location of the mouse pointer on the previous tick, so that know when it has changed.
		private bool _isMouseClicked;						// Keeps track of when the mouse has been clicked, so we can act when it changes.
		private double _evolveCounter = 1f;					// A counter to know when to evolve the simulation one step.
		private double _frameCounter = 0f;					// A counter to know when to advance the animation.
		private double _evolveRate = 2f;					// How often to evolve the simulation, in steps per second.
		private double _frameRate = 6f;						// How often to update the animation, in frames per second.

		List<List<Cell>> _cells = new List<List<Cell>>();	// A list containing lists of Cell instances, 

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// Set up the window size and apply it.
			_graphics.PreferredBackBufferWidth = _screenSize.X; // Use the screen size variable to set the width
			_graphics.PreferredBackBufferHeight = _screenSize.Y; // Use the screen size variable to set the height
			_graphics.ApplyChanges();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// Load the images.
			_cellTxr = Content.Load<Texture2D>("cells"); // Get the right filename for the cells sprite-sheet
			_backTxr = Content.Load<Texture2D>("background");

			// Set up the initial layout of cells.
			InitialiseCellGrid();
		}

		// A function to set up the cell grid.
		// Populates the 2d Lists, and also add some random living cells to start with.
		private void InitialiseCellGrid()
		{
			Random random = new Random();
			// In the List<List<Cells>> (a list of lists of cells)
			// the outer list lists the rows (the y axis)
			// and each inner list is list of cells (the x axis).
			// The cell at x=11,y=12 can be found at _cells[12][11].
			// We need to set up those lists, and add instances of the cell class.
			// First we loop through all the possible rows on the y axis...
			for (int y = 0; y < _gridSize.Y; y++)
			{
				// ...and for each possible row, we a list to contain it...
				_cells.Add(new List<Cell>());

				// ...and then we loop through all the possible cells in that row, on the x axis...
				for (int x = 0; x < _gridSize.X; x++)
				{
					// ...and for each possible cell, we initialise a new one and add it.
					// The cell instance is initialised with x and y co-ords from the for loops
					// and is also given a 1/12 chance of starting out alive.
					// (If the random number <12 turns out to be 0, then the parameter is true, otherwise false.)
					_cells[y].Add(new Cell(new Point(x, y), random.Next(0, 12) == 0)); // Replace 99 with code to get a random number < 12. There is already a Random instance available.
				}
			}
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

			// Handle mouse input from the player.
			// First get the current mouse state and store it.
			MouseState mouseState = Mouse.GetState();
			// Take our current record of what cell the mouse is over, and move it to the previous record.
			// This will help us to know that the user has pointed to a different cell.
			_mouseGridPrev = _mouseGrid;
			// Now update the current record of what cell the mouse is over.
			_mouseGrid = new Point(mouseState.X / _gridSize.X, mouseState.Y / _gridSize.Y);
			
			// If the mouse button is pressed and we have not reacted yet, or if the user has dragged onto a new cell...
			if (mouseState.LeftButton == ButtonState.Pressed && (!_isMouseClicked || _mouseGrid != _mouseGridPrev))
			{
				// ...then record that the mouse has clicked
				_isMouseClicked = true;
				// If the grid location the mouse is under is a valid cell location...
				if (_mouseGrid.X >= 0 && _mouseGrid.X < _gridSize.X
					&& _mouseGrid.Y >= 0 && _mouseGrid.Y < _gridSize.Y)
				{
					// ...then toggle the cell under the mouse.
					_cells[_mouseGrid.Y][_mouseGrid.X].ClickToggle(); // Replace the 0s in [0][0] with the _mouseGrid coordinates.
				}
			}
			// If the mouse button has been realised and we haven't reacted yet... 
			else if (mouseState.LeftButton != ButtonState.Pressed && _isMouseClicked)
			{
				// ...record that the mouse has been released.
				_isMouseClicked= false;
			}
			
			// Here we update the cells' animations.
			// First, count down the frame counter based on how much time has passed since the last tick.
			_frameCounter -= gameTime.ElapsedGameTime.TotalSeconds;
			// If the frame counter has counted down to zero...
			if(_frameCounter <= 0)
			{
				// ...then reset the counter.
				_frameCounter = 1 / _frameRate;
				// Since it's time to update the animation, we want to execute the RandomiseAppearance function on every cell.
				// First we loop through the rows in the y axis...
				foreach (List<Cell> everyRow in _cells)
				{
					// ...then we loop through the cells in each row, on the x axis.
					foreach (Cell everyCell in everyRow)
					{
						// Execute the RandomiseAppearance function on every cell
						everyCell.RandomiseAppearance();
					}
				}
			}

			// This is where we evolve the simulation
			// But only if the mouse button is not held down. We want to pause the simulation while the player is drawing.
			if (mouseState.LeftButton != ButtonState.Pressed)
			{
				// First, count down the evolve counter based on how much time has passed since the last tick.
				_evolveCounter -= gameTime.ElapsedGameTime.TotalSeconds;
				// If the frame counter has counted down to zero...
				if (_evolveCounter <= 0)
				{
					// ...then reset the counter.
					_evolveCounter = 1 / _evolveRate;
					// Since it's time to update the simulation, we want to execute the EvolveStage1 function on every cell.
					// First we loop through the rows in the y axis...
					foreach (List<Cell> everyRow in _cells)
					{
						// ...then we loop through the cells in each row, on the x axis.
						foreach (Cell everyCell in everyRow)
						{
							everyCell.EvolveStage1(_cells, _gridSize);
						}
					}
					// After EvolveStage1 has been executed on every cell without changing their current state
					// we now want to execute EvolveStage2, so that the current state can be updated.
					// First we loop through the rows in the y axis...
					foreach (List<Cell> everyRow in _cells)
					{
						// ...then we loop through the cells in each row, on the x axis.
						foreach (Cell everyCell in everyRow)
						{
							everyCell.EvolveStage2();
						}
					}
				}
			}
			
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			// Start the sprite batch.
			_spriteBatch.Begin();

			// Draw the background to the whole window.
			// Draw the background to the whole window using the screen size variable
			_spriteBatch.Draw(_backTxr, new Rectangle(0, 0, _screenSize.X, _screenSize.Y), Color.White);

			// We want to execute the Draw function on every cell.
			// First we loop through the rows in the y axis...
			foreach(List<Cell> everyRow in _cells)
			{
				// ...then we loop through the cells in each row, on the x axis.
				foreach(Cell everyCell in everyRow)
				{
					everyCell.Draw(_spriteBatch, _cellTxr, _screenSize, _gridSize);
				}
			}

			// End the sprite batch.
			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}