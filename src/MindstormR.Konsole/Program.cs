using System;
using MindstormR.Core;
using MonoBrick.EV3;

namespace MindstormR.Konsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                // Change 'usb' to 'WiFi' when you want to use WiFi. 
                var brick = new Brick<Sensor, Sensor, Sensor, Sensor>("WiFi");
                var robot = new Robot(brick.Vehicle);

                brick.Connection.Open();
                ConsoleKeyInfo cki;
                Console.WriteLine("The EV3 brick has started. Use the arrow keys to navigate. Press Q to quit.");
                do
                {
                    cki = Console.ReadKey(true);
                    switch (cki.Key)
                    {
                        case ConsoleKey.UpArrow:
                            robot.Move(Movement.Forward);
                            Console.WriteLine("Vehicle speed set to " + robot.Speed);
                            break;

                        case ConsoleKey.DownArrow:
                            robot.Move(Movement.Backward);
                            Console.WriteLine("Vehicle speed set to " + robot.Speed);
                            break;

                        case ConsoleKey.LeftArrow:
                            robot.Move(Movement.Left);
                            Console.WriteLine("Vehicle steering set to " + robot.Steering);
                            break;

                        case ConsoleKey.RightArrow:
                            robot.Move(Movement.Right);
                            Console.WriteLine("Vehicle steering set to " + robot.Steering);
                            break;

                        case ConsoleKey.O:
                            Console.WriteLine("Vehicle off");
                            brick.Vehicle.Off();
                            break;

                        case ConsoleKey.B:
                            Console.WriteLine("Vehicle break");
                            brick.Vehicle.Brake();
                            break;
                    }
                } while (cki.Key != ConsoleKey.Q);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                Console.WriteLine("Press any key to quit...");
                Console.ReadKey();
            }
        }
    }
}
