using System;
using System.Collections.Generic;
using System.Linq;

class Player
{
	private static readonly Grid TheGrid = new Grid(30, 20);
	private static readonly IDictionary<int, PlayerContext> PlayerContexts = new Dictionary<int, PlayerContext>();

	static void Main(string[] args)
	{
		while (true)
		{
			var inputs = Console.ReadLine().Split(' ').Select(Int32.Parse).ToArray();
			int numberOfPlayers = inputs[0]; // total number of players (2 to 4).
			int myPlayerNumber = inputs[1]; // your player number (0 to 3).


			// Determine cells prior to making move
			for (int i = 0; i < numberOfPlayers; i++)
			{
				inputs = Console.ReadLine().Split(' ').Select(Int32.Parse).ToArray();

				int x = inputs[2]; // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
				int y = inputs[3]; // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)

				// Regardless of the player, add the current cell to a list of closed cells
				TheGrid.AddClosedCell(x, y);

				// Ensure a player context exists for each player.
				if (!PlayerContexts.ContainsKey(i))
				{
					PlayerContexts.Add(i, new PlayerContext(TheGrid));
				}

				// Retain my player's current cell
				if (i == myPlayerNumber)
				{
					PlayerContexts[i].SetCurrentCell(TheGrid.Cell(x, y));
				}
			}


			for (int i = 0; i < numberOfPlayers; i++)
			{
				if (i != myPlayerNumber)
					continue;

				var playerContext = PlayerContexts[myPlayerNumber];

				var floodFill = new FloodFillAlgorithm(TheGrid);
				var immediateNeighbours = floodFill.NeighboursOf(playerContext.CurrentCell, 30);
				var nextCell = immediateNeighbours.OrderByDescending(n => n.Value).ThenBy(n => Guid.NewGuid()).First().Key;

				// Record direction
				playerContext.SetNewDirection(nextCell);

				// Set direction
				Console.WriteLine(playerContext.CurrentDirection);
			}

		}

	}

}

public class PlayerContext
{
	private const string Left = "LEFT";
	private const string Right = "RIGHT";
	private const string Up = "UP";
	private const string Down = "DOWN";

	private readonly Grid _grid;

	public readonly HashSet<Cell> Path;
	public Cell CurrentCell { get; private set; }
	public Cell PreviousCell { get; private set; }
	public string CurrentDirection { get; private set; }

	public PlayerContext(Grid grid)
	{
		_grid = grid;

		Path = new HashSet<Cell>();
	}

	public void SetCurrentCell(Cell cell)
	{
		// Retain previous cell
		PreviousCell = CurrentCell;

		CurrentCell = cell;
		Path.Add(cell);
	}

	public void SetNewDirection(string direction)
	{
		CurrentDirection = direction;
	}

	public void SetNewDirection(Cell nextCell)
	{
		if (nextCell.X > CurrentCell.X)
		{
			CurrentDirection = Right;
		}
		else if (nextCell.X < CurrentCell.X)
		{
			CurrentDirection = Left;
		}
		else if (nextCell.Y > CurrentCell.Y)
		{
			CurrentDirection = Down;
		}
		else
		{
			CurrentDirection = Up;
		}
	}

	public bool CanMoveUp()
	{
		return !(CurrentCell.Y == 0 || CurrentDirection == Down || _grid.Cell(CurrentCell.X, CurrentCell.Y - 1).IsClosed());
	}

	public bool CanMoveDown()
	{
		return !(CurrentCell.Y == _grid.MaxY || CurrentDirection == Up || _grid.Cell(CurrentCell.X, CurrentCell.Y + 1).IsClosed());
	}

	public bool CanMoveLeft()
	{
		return !(CurrentCell.X == 0 || CurrentDirection == Right || _grid.Cell(CurrentCell.X - 1, CurrentCell.Y).IsClosed());
	}

	public bool CanMoveRight()
	{
		return !(CurrentCell.X == _grid.MaxX || CurrentDirection == Left || _grid.Cell(CurrentCell.X + 1, CurrentCell.Y).IsClosed());
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
