using System;
using System.Collections.Generic;

namespace AggregationExample2;

// ------------------------
// Child class
// Player can exist without Team
// ------------------------
class Player
{
    public string Name { get; set; }

    public Player(string name)
    {
        Name = name;
    }
}

// ------------------------
// Parent class
// Team HAS Players (Aggregation)
// ------------------------
class Team
{
    public string TeamName { get; set; }

    // Aggregation:
    // Players are NOT created here
    // They are injected from outside
    public List<Player> Players { get; set; }

    public Team(string teamName, List<Player> players)
    {
        TeamName = teamName;
        Players = players;
    }

    public void ShowPlayers()
    {
        Console.WriteLine($"Team: {TeamName}");
        foreach (var player in Players)
        {
            Console.WriteLine($"- {player.Name}");
        }
    }
}

// ------------------------
// Program Entry
// ------------------------
// class Program
// {
//     static void Main(string[] args)
//     {
//         // Players exist independently
//         Player p1 = new Player("Shakib");
//         Player p2 = new Player("Tamim");

//         List<Player> playerList = new List<Player> { p1, p2 };

//         // Team aggregates players
//         Team team = new Team("Bangladesh Team", playerList);
//         team.ShowPlayers();

//         Console.WriteLine();

//         // Team is destroyed
//         team = null;

//         // Players are still alive
//         Console.WriteLine("Team removed, but players still exist:");
//         Console.WriteLine(p1.Name);
//         Console.WriteLine(p2.Name);
//     }
// }

