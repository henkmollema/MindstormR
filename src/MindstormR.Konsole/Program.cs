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
                        case ConsoleKey.B:
                            // Fire a single shot.
                            Console.WriteLine("Firing motor B!");

                            brick.MotorB.ResetTacho();
                            brick.MotorB.MoveTo(127, 1080, true);
                            break;

                        case ConsoleKey.X:
                            // Keep shooting.
                            brick.MotorB.On(127);
                            break;

                        case ConsoleKey.Z:
                            brick.MotorB.Brake();
                            break;

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
