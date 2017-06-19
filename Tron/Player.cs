using System;

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

	            if (i == myPlayerNumber)
	            {
		            string newDirection = null;

		            if (y > 0 && currentDirection != Down)
		            {
						// Can move up
			            newDirection = Up;
		            }

					else if (y < GridMaxY && currentDirection != Up)
		            {
						// Can move down
			            newDirection = Down;
		            }

					else if (x > 0 && currentDirection != Right)
		            {
						// Can move leftLeft;
			            newDirection = Left;
		            }

					else if (x < GridMaxX && currentDirection != Left)
		            {
						// Can move right
			            newDirection = Right;
		            }

		            else
		            {
			            // Ah bugger ...
		            }

					// Retain direction
		            currentDirection = newDirection;

					// Set direction
		            Console.WriteLine(newDirection);
				}
			}

        }

    }


}