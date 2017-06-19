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

	private const int GridMaxX = 29;
	private const int GridMaxY = 19;


	static void Main(string[] args)
    {
        string[] inputs;

	    string currentDirection = null;
		var closedCells = new HashSet<Point>();

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int numberOfPlayers = int.Parse(inputs[0]); // total number of players (2 to 4).
            int myPlayerNumber = int.Parse(inputs[1]); // your player number (0 to 3).
            for (int i = 0; i < numberOfPlayers; i++)
            {
                inputs = Console.ReadLine().Split(' ');

				int x = int.Parse(inputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
                int y = int.Parse(inputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)

				
				// Regardless of the player, add the current cell to a list of closed cells
	            closedCells.Add(new Point(x, y));


				if (i == myPlayerNumber)
	            {
		            var newDirectionOptions = new Collection<string>();

		            if (y > 0 && currentDirection != Down && !closedCells.Contains(new Point(x, y - 1)))
		            {
						// Can move up
			            newDirectionOptions.Add(Up);
		            }

					if (y < GridMaxY && currentDirection != Up && !closedCells.Contains(new Point(x, y + 1)))
		            {
						// Can move down
			            newDirectionOptions.Add(Down);
		            }

					if (x > 0 && currentDirection != Right && !closedCells.Contains(new Point(x - 1, y)))
		            {
						// Can move leftLeft;
			            newDirectionOptions.Add(Left);
		            }

					if (x < GridMaxX && currentDirection != Left && !closedCells.Contains(new Point(x + 1, y)))
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