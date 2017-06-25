using System;
using System.Collections;
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
				PlayerContexts[i].SetCurrentCell(TheGrid.Cell(x, y));
			}


			for (int i = 0; i < numberOfPlayers; i++)
			{
				if (i != myPlayerNumber)
					continue;

				var playerContext = PlayerContexts[myPlayerNumber];
				var opponentContext = PlayerContexts.FirstOrDefault(x => x.Key != myPlayerNumber).Value;

				var pathfinder = new PathfinderAlgorithm();
				var pathToOpponent = pathfinder.FindPath(playerContext.CurrentCell, opponentContext.CurrentCell);

				var floodFill = new FloodFillAlgorithm(TheGrid);
				var floodfillCounts = floodFill.NeighboursOf(playerContext.CurrentCell, 30);
				var nextCell = floodfillCounts.OrderByDescending(n => n.Value).ThenBy(n => Guid.NewGuid()).First().Key;

				// Record direction
				playerContext.SetNewDirection(nextCell);

				// Set direction
				Console.WriteLine(playerContext.CurrentDirection);
			}

		}

	}

}

public class Direction
{
	public const string Left = "LEFT";
	public const string Right = "RIGHT";
	public const string Up = "UP";
	public const string Down = "DOWN";
}

public class PlayerContext
{
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
			CurrentDirection = Direction.Right;
		}
		else if (nextCell.X < CurrentCell.X)
		{
			CurrentDirection = Direction.Left;
		}
		else if (nextCell.Y > CurrentCell.Y)
		{
			CurrentDirection = Direction.Down;
		}
		else
		{
			CurrentDirection = Direction.Up;
		}
	}

	public bool CanMoveUp()
	{
		return !(CurrentCell.Y == 0 || CurrentDirection == Direction.Down || _grid.Cell(CurrentCell.X, CurrentCell.Y - 1).IsClosed());
	}

	public bool CanMoveDown()
	{
		return !(CurrentCell.Y == _grid.MaxY || CurrentDirection == Direction.Up || _grid.Cell(CurrentCell.X, CurrentCell.Y + 1).IsClosed());
	}

	public bool CanMoveLeft()
	{
		return !(CurrentCell.X == 0 || CurrentDirection == Direction.Right || _grid.Cell(CurrentCell.X - 1, CurrentCell.Y).IsClosed());
	}

	public bool CanMoveRight()
	{
		return !(CurrentCell.X == _grid.MaxX || CurrentDirection == Direction.Left || _grid.Cell(CurrentCell.X + 1, CurrentCell.Y).IsClosed());
	}

}

public class FloodFillAlgorithm
{
	private readonly Grid _grid;

	public bool IncludeClosedCells { get; set; }

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

		foreach (var neighbour in immediateNeighbours)
		{
			var neighbourFloodFill = new HashSet<Cell>();

			if (depth > 1)
				FindNeighbouringCellsOf(neighbour, depth, neighbourFloodFill);

			ret.Add(neighbour, neighbourFloodFill.Count);
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

public struct Cell
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

	public Cell? Up()
	{
		if (Y - 1 >= 0)
			return _grid.Cell(X, Y - 1);

		return null;
	}

	public Cell? Left()
	{
		if (X - 1 >= 0)
			return _grid.Cell(X - 1, Y);

		return null;
	}

	public Cell? Right()
	{
		if (X + 1 <= _grid.MaxX)
			return _grid.Cell(X + 1, Y);

		return null;
	}

	public Cell? Down()
	{
		if (Y + 1 <= _grid.MaxY)
			return _grid.Cell(X, Y + 1);

		return null;
	}

	public IEnumerable<Cell> Neighbours(bool includeClosed = false)
	{
		var allNeighbours = new[]
			{
				Up(),
				Down(),
				Left(),
				Right()
			}
			.Where(x => x.HasValue)
			.Select(x => x.Value);

		return includeClosed ? allNeighbours : allNeighbours.Where(x => !x.IsClosed());
	}

	#region Overrides

	public override string ToString()
	{
		return $"{X},{Y}";
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

public class PathfinderAlgorithm
{

	public Path FindPath(Cell start, Cell destination)
	{
		// Retain list of visited cells
		var closed = new HashSet<Cell>();

		// Create 
		var queue = new PriorityQueue(0, new Path(start));

		while (!queue.IsEmpty)
		{
			// Take the next path in the queue
			var path = queue.Dequeue();

			// Ignore this path if the last step of this path was added to the closed list.
			if (closed.Contains(path.LastStep))
			{
				Console.Error.WriteLine();
				continue;
			}



			// If the last step of this path is the destination, return this one.
			if (path.LastStep.Equals(destination))
				return path;

			closed.Add(path.LastStep);

			// Loop through the immediate neighbours of the current cell and build nested
			// paths. Because the Neighbours() call will ignore closed cells by default, we
			// should override this as this would mean ignoring the destination cell (it is
			// the opponents current cell, so must be closed). Therefore we should ensure
			// that we are including all cells but filtering out those that are closed but
			// not the destination cell.
			foreach (var cell in path.LastStep.Neighbours(includeClosed: true))
			{
				if (cell.IsClosed() && !cell.Equals(destination))
					continue;

				var newPath = path.AddStep(cell);
				queue.Enqueue(newPath.TotalCost + ManhattanEstimate(cell, destination), newPath);
			}
		}

		return null;
	}

	public static double EuclideanEstimate(Cell start, Cell destination)
	{
		return Math.Sqrt(Math.Pow(start.X - destination.X, 2) + Math.Pow(start.Y - destination.Y, 2));
	}

	private static int ManhattanEstimate(Cell start, Cell destination)
	{
		return (Math.Abs(destination.X - start.X) + Math.Abs(destination.Y - start.Y)) * 10;
	}

}

public class Path : IEnumerable<Cell>
{
	public Cell LastStep { get; }
	public Path PreviousSteps { get; }
	public int TotalCost { get; }

	private Path(Cell lastStep, Path previousSteps, int totalCost)
	{
		LastStep = lastStep;
		PreviousSteps = previousSteps;
		TotalCost = totalCost;
	}

	public Path(Cell start) : this(start, null, 0) { }

	public Path AddStep(Cell step)
	{
		return new Path(step, this, TotalCost + 1);
	}

	public IEnumerator<Cell> GetEnumerator()
	{
		for (var p = this; p != null; p = p.PreviousSteps)
		{
			yield return p.LastStep;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}

public class PriorityQueue
{
	private readonly SortedDictionary<double, Queue<Path>> _list = new SortedDictionary<double, Queue<Path>>();

	public PriorityQueue(double startPriority, Path startItemToQueue)
	{
		Enqueue(startPriority, startItemToQueue);
	}

	public void Enqueue(double priority, Path value)
	{
		Queue<Path> q;
		if (!_list.TryGetValue(priority, out q))
		{
			q = new Queue<Path>();
			_list.Add(priority, q);
		}
		q.Enqueue(value);
	}

	public Path Dequeue()
	{
		// will throw if there isn’t any first element!
		var pair = _list.First();
		var v = pair.Value.Dequeue();
		if (pair.Value.Count == 0) // nothing left of the top priority.
			_list.Remove(pair.Key);

		return v;
	}

	public bool IsEmpty => !_list.Any();
}
