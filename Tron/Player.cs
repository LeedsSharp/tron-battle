using System;
using System.Collections.Generic;
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

			Cell currentCell = null;

			// Determine cells prior to making move
			for (int i = 0; i < numberOfPlayers; i++)
			{
				inputs = Console.ReadLine().Split(' ');

				int x = int.Parse(inputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
				int y = int.Parse(inputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)

				// Regardless of the player, add the current cell to a list of closed cells
				TheGrid.AddClosedCell(x, y);

				// Retain my player's current cell
				if (i == myPlayerNumber)
				{
					currentCell = TheGrid.Cell(x, y);
				}
			}


			for (int i = 0; i < numberOfPlayers; i++)
			{
				if (i != myPlayerNumber)
					continue;

				var floodFill = new FloodFillAlgorithm(TheGrid);
				var immediateNeighbours = floodFill.NeighboursOf(currentCell, 30);
				var nextCell = immediateNeighbours.OrderByDescending(n => n.Value).ThenBy(n => Guid.NewGuid()).First().Key;

				string newDirection = null;

				if (nextCell.X > currentCell.X)
				{
					newDirection = Right;
				}
				else if (nextCell.X < currentCell.X)
				{
					newDirection = Left;
				}
				else if (nextCell.Y > currentCell.Y)
				{
					newDirection = Down;
				}
				else
				{
					newDirection = Up;
				}

				// Set direction
				Console.WriteLine(newDirection);
			}

		}

	}

}

public class FloodFillAlgorithm
{
	private readonly Grid _grid;

	private bool IncludeClosedCells { get; set; }

	public FloodFillAlgorithm(Grid grid)
	{
		_grid = grid;
	}

	/// <summary>
	/// Returns the immediate neighbouring cells
	/// </summary>
	public IEnumerable<Cell> NeighboursOf(Cell cell)
	{
		return NeighboursOf(cell, 1).Keys;
	}

	/// <summary>
	/// Returns a dictionary of the immediate neighbouring cells and their associated floodfills
	/// </summary>
	public IDictionary<Cell, int> NeighboursOf(Cell cell, int depth)
	{
		// Find immediate neighbors
		var immediateNeighbours = new HashSet<Cell>();
		FindNeighbouringCellsOf(cell, 1, immediateNeighbours);

		var ret = new Dictionary<Cell, int>();

		if (depth == 1)
		{
			ret.Add(cell, immediateNeighbours.Count);
		}
		else
		{
			foreach (var neighbour in immediateNeighbours)
			{
				var neighbourFloodFill = new HashSet<Cell>();

				FindNeighbouringCellsOf(neighbour, depth, neighbourFloodFill);

				ret.Add(neighbour, neighbourFloodFill.Count);
			}
		}

		return ret;
	}

	private void FindNeighbouringCellsOf(Cell cell, int depth, HashSet<Cell> set)
	{
		depth = depth - 1;

		// Move right
		var newCell = _grid.Cell(cell.X + 1, cell.Y);
		if (newCell.X <= _grid.MaxX && !set.Contains(newCell) && (!newCell.IsClosed() || (IncludeClosedCells && newCell.IsClosed())))
		{
			set.Add(newCell);

			if (depth > 0)
			{
				FindNeighbouringCellsOf(newCell, depth, set);
			}
		}

		// Move down
		newCell = _grid.Cell(cell.X, cell.Y + 1);
		if (newCell.Y <= _grid.MaxY && !set.Contains(newCell) && (!newCell.IsClosed() || (IncludeClosedCells && newCell.IsClosed())))
		{
			set.Add(newCell);

			if (depth > 0)
			{
				FindNeighbouringCellsOf(newCell, depth, set);
			}
		}

		// Move left
		newCell = _grid.Cell(cell.X - 1, cell.Y);
		if (newCell.X >= 0 && !set.Contains(newCell) && (!newCell.IsClosed() || (IncludeClosedCells && newCell.IsClosed())))
		{
			set.Add(newCell);

			if (depth > 0)
			{
				FindNeighbouringCellsOf(newCell, depth, set);
			}
		}

		// Move up
		newCell = _grid.Cell(cell.X, cell.Y - 1);
		if (newCell.Y >= 0 && !set.Contains(newCell) && (!newCell.IsClosed() || (IncludeClosedCells && newCell.IsClosed())))
		{
			set.Add(newCell);

			if (depth > 0)
			{
				FindNeighbouringCellsOf(newCell, depth, set);
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

	public Cell Cell(int x, int y)
	{
		return new Cell(x, y, this);
	}

	public void AddClosedCell(int x, int y)
	{
		ClosedCells.Add(Cell(x, y));
	}

	public bool HasClosedCell(int x, int y)
	{
		return ClosedCells.Contains(Cell(x, y));
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
