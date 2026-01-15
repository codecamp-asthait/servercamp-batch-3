using System;
using System.Collections.Generic;

namespace CompositionExample
{
    // ------------------------
    // Child class
    // Cannot exist without House
    // ------------------------
    class Room
    {
        public string Name { get; }

        public Room(string name)
        {
            Name = name;
        }
    }

    // ------------------------
    // Parent class
    // House OWNS Rooms
    // ------------------------
    class House
    {
        private List<Room> rooms;

        public House()
        {
            // Composition:
            // Rooms are created INSIDE the House
            rooms = new List<Room>
            {
                new Room("Bedroom"),
                new Room("Living Room"),
                new Room("Kitchen")
            };
        }

        public void ShowRooms()
        {
            Console.WriteLine("House Rooms:");
            foreach (var room in rooms)
            {
                Console.WriteLine($"- {room.Name}");
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
    //         House house = new House();
    //         house.ShowRooms();

    //         Console.WriteLine();

    //         // When house is destroyed,
    //         // rooms are destroyed automatically
    //         house = null;

    //         Console.WriteLine("House destroyed → Rooms destroyed");
    //     }
    // }
}
