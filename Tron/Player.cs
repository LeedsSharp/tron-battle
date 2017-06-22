using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

class Player
{
	private const string Left = "LEFT";
	private const string Right = "RIGHT";
	private const string Up = "UP";
	private const string Down = "DOWN";

	private static readonly Grid TheGrid = new Grid(30, 20);

	static void Main(string[] args)
	{
		string currentDirection = null;

		// game loop
		while (true)
		{
			var inputs = Console.ReadLine().Split(' ');
			int numberOfPlayers = int.Parse(inputs[0]); // total number of players (2 to 4).
			int myPlayerNumber = int.Parse(inputs[1]); // your player number (0 to 3).

			for (int i = 0; i < numberOfPlayers; i++)
			{
				inputs = Console.ReadLine().Split(' ');

				int x = int.Parse(inputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
				int y = int.Parse(inputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)


				// Regardless of the player, add the current cell to a list of closed cells
				TheGrid.AddClosedCell(x, y);


				if (i == myPlayerNumber)
				{
					var newDirectionOptions = new Collection<string>();

					if (y > 0 && currentDirection != Down && !TheGrid.HasClosedCell(x, y - 1))
					{
						// Can move up
						newDirectionOptions.Add(Up);
					}

					if (y < TheGrid.MaxY && currentDirection != Up && !TheGrid.HasClosedCell(x, y + 1))
					{
						// Can move down
						newDirectionOptions.Add(Down);
					}

					if (x > 0 && currentDirection != Right && !TheGrid.HasClosedCell(x - 1, y))
					{
						// Can move leftLeft;
						newDirectionOptions.Add(Left);
					}

					if (x < TheGrid.MaxX && currentDirection != Left && !TheGrid.HasClosedCell(x + 1, y))
					{
						// Can move right
						newDirectionOptions.Add(Right);
					}

					// Retain direction
					string newDirection = newDirectionOptions.OrderBy(o => Guid.NewGuid()).First();

					currentDirection = newDirection;

					// Set direction
					Console.WriteLine(newDirection);
				}
			}

		}

	}

}

public class Grid
{
	public readonly int MaxX;
	public readonly int MaxY;
	public readonly int Width;
	public readonly int Height;
	public readonly HashSet<Cell> ClosedCells = new HashSet<Cell>();

	public Grid(int width, int height)
	{
		Width = width;
		Height = height;

		MaxX = width - 1;
		MaxY = height - 1;
	}

	public void AddClosedCell(int x, int y)
	{
		ClosedCells.Add(new Cell(x, y, this));
	}

	public bool HasClosedCell(int x, int y)
	{
		return ClosedCells.Contains(new Cell(x, y, this));
	}
}

public class Cell
{
	public readonly int X;
	public readonly int Y;

	private readonly Grid _grid;

	public Cell(int x, int y, Grid grid)
	{
		X = x;
		Y = y;

		_grid = grid;
	}

	public bool IsClosed()
	{
		return _grid.ClosedCells.Contains(this);
	}

	#region Overrides

	public override string ToString()
	{
		return String.Format("{0},{1}", X, Y);
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
			return false;

		var coordinates = (Cell)obj;
		return X == coordinates.X && Y == coordinates.Y;
	}

	public override int GetHashCode()
	{
		unchecked // Overflow is fine, just wrap
		{
			int hash = 17;
			// Suitable nullity checks etc, of course :)
			hash = hash * 23 + X.GetHashCode();
			hash = hash * 23 + Y.GetHashCode();
			return hash;
		}
	}

	#endregion

}
