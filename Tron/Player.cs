using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
class Player
{
    static void Main(string[] args)
    {
        string[] inputs;
        string[] directions = { "UP", "DOWN", "LEFT", "RIGHT" };

        var grid = new bool[30, 20]; // true means a player has been at this coordinate

        // game loop
        while (true)
        {
            inputs = Console.ReadLine().Split(' ');
            int totalNumberOfPlayers = int.Parse(inputs[0]); // total number of players (2 to 4).
            int yourPlayerNumber = int.Parse(inputs[1]); // your player number (0 to 3).
            for (int currentPlayer = 0; currentPlayer < totalNumberOfPlayers; currentPlayer++)
            {
                inputs = Console.ReadLine().Split(' ');
                int currentPlayerStartingX = int.Parse(inputs[0]); // starting X coordinate of lightcycle (or -1)
                int currentPlayerStartingY = int.Parse(inputs[1]); // starting Y coordinate of lightcycle (or -1)
                int currentPlayerCurrentX = int.Parse(inputs[2]); // current X coordinate of lightcycle (can be the same as X0 if you play before this player)
                int currentPlayerCurrentY = int.Parse(inputs[3]); // current Y coordinate of lightcycle (can be the same as Y0 if you play before this player)

                Console.Error.WriteLine("Player " + currentPlayer + " starting position: " + currentPlayerStartingX + ", " + currentPlayerStartingY);
                Console.Error.WriteLine("Player " + currentPlayer + " current position: " + currentPlayerCurrentX + ", " + currentPlayerCurrentY);

                grid[currentPlayerCurrentX, currentPlayerCurrentY] = true;
            }

            var nextMove = RandomMouse(directions);
            Console.WriteLine(nextMove); // A single line with UP, DOWN, LEFT or RIGHT
        }
    }

    private static object RandomMouse(string[] directions)
    {
        var randomiser = new Random(DateTime.Now.Millisecond);
        return directions[randomiser.Next(directions.Length)];
    }
}