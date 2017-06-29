using System;
using System.Collections.Generic;
using System.Linq;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Game
{
    static void Main(string[] args)
    {
        var players = new List<Player>();
        Player me = null;
        var board = new Board();

        // game loop
        while (true)
        {
            var inputs = Console.ReadLine().Split(' ');
            int totalNumberOfPlayers = int.Parse(inputs[0]); // total number of players (2 to 4).
            int yourPlayerNumber = int.Parse(inputs[1]); // your player number (0 to 3).
            for (int p = 0; p < totalNumberOfPlayers; p++)
            {
                inputs = Console.ReadLine().Split(' ');
                if (players.Count < (p + 1)) players.Add(new Player(board));

                players[p].StartPosition.X = int.Parse(inputs[0]); // starting X coordinate of lightcycle (or -1)
                players[p].StartPosition.Y = int.Parse(inputs[1]); // starting Y coordinate of lightcycle (or -1)
                
                players[p].MoveTo(int.Parse(inputs[2]), int.Parse(inputs[3]));

                Console.Error.WriteLine("Player " + p + " starting position: " + players[p].StartPosition.X + ", " + players[p].StartPosition.Y);
                Console.Error.WriteLine("Player " + p + " current position: " + players[p].CurrentPosition.X + ", " + players[p].CurrentPosition.Y);


                // track your current position
                if (p == yourPlayerNumber)
                {
                    me = players[p];
                }
            }

            if (me == null) continue;


            var nextMove = me.GenerateMove();
            Console.WriteLine(nextMove.ToString().ToUpper()); // A single line with UP, DOWN, LEFT or RIGHT
        }
    }
}

public class Player
{
    private readonly Board _board;
    private Direction? _lastDirection = null;

    public Player(Board board)
    {
        _board = board;
        CurrentPosition = new Point();
        StartPosition = new Point();
    }

    public Point StartPosition { get; internal set; }
    public Point CurrentPosition { get; internal set; }

    internal void MoveTo(int x, int y)
    {
        CurrentPosition.X = x;
        CurrentPosition.Y = y;
        _board.Visit(x, y);
    }

    public string GenerateMove()
    {
        var validDirections = _board.GetAvailbleDirections(CurrentPosition).ToList();
        return RandomStickyMouse(validDirections);
    }

    private string RandomStickyMouse(List<Direction> directions)
    {
        if (!_lastDirection.HasValue || !directions.Contains(_lastDirection.Value))
        {
            var randomiser = new Random(DateTime.Now.Millisecond);
            _lastDirection = directions[randomiser.Next(directions.Count)];
        }

        return _lastDirection.ToString();
    }
}

public class Board
{
    private const int XLength = 30;
    private const int YLength = 20;

    public Board()
    {
        Grid = new bool[XLength, YLength]; // true means a player has been at this coordinate
    }

    private readonly bool[,] Grid;

    public void Visit(int x, int y)
    {
        Grid[x, y] = true;
    }

    bool SpaceAvailable(int x, int y)
    {
        return x >= 0 && x < XLength &&
            y >= 0 && y < YLength &&
            !Grid[x, y];
    }

    public IEnumerable<Direction> GetAvailbleDirections(Point p)
    {
        if (SpaceAvailable(p.X, p.Y-1)) yield return Direction.UP;
        if (SpaceAvailable(p.X + 1, p.Y)) yield return Direction.RIGHT;
        if (SpaceAvailable(p.X, p.Y + 1)) yield return Direction.DOWN;
        if (SpaceAvailable(p.X - 1, p.Y)) yield return Direction.LEFT;
    }
}

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
}

public enum Direction
{
     UP, DOWN, LEFT, RIGHT
}