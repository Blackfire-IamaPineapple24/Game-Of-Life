using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife
{
	internal class Cell
	{
		public Point _cellLocation;							// The x,y coords of this cell on the cell grid.
		public bool _cellState;								// True means is cell is alive. False means it's dead.
		private bool _nextState;							// What will the next state be? Used so that all cells can be updated at once.

		private Rectangle _cellDraw_SourceRect;				// For sampling one of the 4 cell sprites on the sprite sheet, for the randomised animation.
		private Rectangle _cellDraw_DestRect;				// For drawing to the correct cell on the grid.
		private float _cellDraw_Angle;						// Orientation of the sprite in radians, for the randomised animation.
		public float _cellDraw_Scale = 0f;					// Size of the sprite, for animating appearance/disappearance.
		private float _cellDraw_ScaleSpeedRate = 0.1f;		// How quickly the sprite size animates, in units per frame.

		// Constructor for the Cell class
		// Assigns location and starting state and initial random appearance.
		public Cell(Point cellLocation, bool cellState)
		{
			_cellLocation = cellLocation;
			_cellState = cellState;

			RandomiseAppearance();
		}

		// A function to choose a random sprite and orientation.
		public void RandomiseAppearance()
		{
			Random random = new Random();

			// The sprite sheet is a 2x2 grid of 32x32 sprites.
			// Picks one of them at random and assign it to _cellDraw_SourceRect.
			switch (random.Next(4))
			{
				case 0:
					_cellDraw_SourceRect = new Rectangle(0, 0, 32, 32);
					break;
				case 1:
					_cellDraw_SourceRect = new Rectangle(32, 0, 32, 32);
					break;
				case 2:
					_cellDraw_SourceRect = new Rectangle(0, 32, 32, 32);
					break;
				case 3:
					_cellDraw_SourceRect = new Rectangle(32, 32, 32, 32);
					break;
			}

			// Pick a random orientation, in radians.
			_cellDraw_Angle = (float)(0); // TODO - replace the 0 with a random number between 0 and 2*PI.
		}

		// The first stage of the simulation.
		// This decides whether each cell should be alive or dead.
		// The actual state of the cell doesn't change until the second stage, so we must not change _currentState until stage 2.
		public void EvolveStage1(List<List<Cell>> cells, Point gridSize)
		{
			int neighbourCount = 0; // Used to keep track of how many live neighbours this cell has.

			// Check ⬆
			// If this isn't the top-most edge, and the cell above is alive, count it.
			if (_cellLocation.Y > 0
				&& cells[_cellLocation.Y - 1][_cellLocation.X]._cellState)
			{
				neighbourCount++;
			}

			// Check ⬇
			// If this isn't the bottom-most edge, and the cell below is alive, count it.
			if (_cellLocation.Y < gridSize.Y - 1
				&& cells[_cellLocation.Y + 1][_cellLocation.X]._cellState)
			{
				neighbourCount++;
			}

			// Check ⬅
			// If this isn't the left-most edge, and the cell to the left is alive, count it.
			if (_cellLocation.X > 0
				&& cells[_cellLocation.Y][_cellLocation.X - 1]._cellState)
			{
				neighbourCount++;
			}

			// Check ↖
			// If this isn't the left-most or topmost edge, and the cell to the upper-left is alive, count it.
			if (_cellLocation.X > 0
				&& _cellLocation.Y > 0
				&& cells[_cellLocation.Y - 1][_cellLocation.X - 1]._cellState)
			{
				neighbourCount++;
			}

			// Check ↙
			// If this isn't the left-most or bottommost edge, and the cell to the lower-left is alive, count it.
			if (_cellLocation.X > 0
				&& _cellLocation.Y < gridSize.Y - 1
				&& cells[_cellLocation.Y + 1][_cellLocation.X - 1]._cellState)
			{
				neighbourCount++;
			}

			// Check ➡
			// If this isn't the right-most edge, and the cell to the right is alive, count it.
			if (_cellLocation.X < gridSize.X - 1
				&& cells[_cellLocation.Y][_cellLocation.X + 1]._cellState)
			{
				neighbourCount++;
			}

			// Check ↗
			// If this isn't the rightmost or top-most edge, and the cell to the upper-right is alive, count it.
			if (_cellLocation.X < gridSize.X - 1
				&& _cellLocation.Y > 0
				&& cells[_cellLocation.Y - 1][_cellLocation.X + 1]._cellState)
			{
				neighbourCount++;
			}

			// Check ↘
			// If this isn't the rightmost or bottom-most edge, and the cell to the lower-right is alive, count it.
			if (false) // TODO - based on the previous checks, replace the false with a condition that checks whether there is a lower-left neighbour.
			{
				neighbourCount++;
			}

			// Based on the number of living neighbours found, set the next state of this cell.

			// TODO - write some code here that sets whether the cell should be alive or dead.
			// Read over all the code and comments to understand how this works.
			// You will need to check the current cell state and the number of neighbours found, and use them set the new value of _nextState.
			// You can base this on the standard rules for Conway's Game of Life, which you will find online.
			// If this one feels hard, it's probably easier than you think. Your solution should be around 2 to 8 lines.
		}

		// The second stage of the simulation.
		// This is where the cell state actually changes. So far we have only updated _nextState, not _currentState.
		// This is so that all cells can decide their new state without any of them changing
		// and then they all change at once.
		public void EvolveStage2()
		{
			_cellState = _nextState;
		}

		// A function to toggle the alive/dead state of a cell when clicked by the player.
		public void ClickToggle()
		{
			// The boolean state should become it's opposite.
			_cellState = !_cellState;
		}

		// A function to draw the cell.
		// Uses the current state to update the scale, so that the can grow/shrink when it appears/disappears.
		public void Draw(SpriteBatch spriteBatch, Texture2D cellTxr, Point screenSize, Point gridSize)
		{
			// If the cell is alive is not full size...
			if (_cellState && _cellDraw_Scale < 1f)
			{
				// ...then increase the size.
				_cellDraw_Scale += _cellDraw_ScaleSpeedRate;
			}
			// If the cell is dead and has not shrunk away...
			else if (!_cellState && _cellDraw_Scale > 0f)
			{
				// ...then decrease the size.
				_cellDraw_Scale -= _cellDraw_ScaleSpeedRate;
			}

			if (_cellDraw_Scale > 0f)
			{
				// If the cell ought to be visible (not shrunk away)
				// first recalculate the rectangle to draw it to, based on its grid position and its size.
				// The 0.5f is needed because the origin of the cell sprite will be at it's centre,
				// so we aim for the centre of the grid cell.
				_cellDraw_DestRect = new Rectangle(
					(int)((_cellLocation.X + 0.5f) * screenSize.X / gridSize.X),
					(int)((_cellLocation.Y + 0.5f) * screenSize.Y / gridSize.Y),
					(int)(screenSize.X / gridSize.X * _cellDraw_Scale),
					(int)(screenSize.Y / gridSize.Y * _cellDraw_Scale));

				// Then make the draw call, using the spriteBatch reference passed into the function.
				// The source rectangle and the angle have already been decided by RandomiseAppearance().
				// The origin is the centre of the 32x32 pixels in the source rectangle.
				spriteBatch.Draw(cellTxr,
					_cellDraw_DestRect,
					_cellDraw_SourceRect,
					Color.White,
					_cellDraw_Angle,
					new Vector2(16, 16),
					SpriteEffects.None,
					0f);
			}
		}
	}
}
