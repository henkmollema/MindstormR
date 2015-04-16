using System;
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
                sbyte speed = 0;
                sbyte leftTurnPerfect = 0;
                sbyte rightTurnPercent = 0;

                brick.Connection.Open();
                ConsoleKeyInfo cki;
                Console.WriteLine("The EV3 brick has started. Use the arrow keys to navigate. Press Q to quit.");
                do
                {
                    cki = Console.ReadKey(true); 
                    switch (cki.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (speed < 100)
                            {
                                speed = (sbyte)(speed + 10);
                            }
                            Console.WriteLine("Vehicle speed set to " + speed);
                            brick.Vehicle.Forward(speed);
                            break;

                        case ConsoleKey.DownArrow:
                            if (speed > -100)
                            {
                                speed = (sbyte)(speed - 10);
                            }
                            Console.WriteLine("Vehicle speed set to " + speed);
                            brick.Vehicle.Forward(speed);
                            break;

                        case ConsoleKey.LeftArrow:
                            if (rightTurnPercent > 0)
                            {
                                rightTurnPercent -= 10;
                                Console.WriteLine("Set vehicle right turn percent to " + rightTurnPercent);
                                brick.Vehicle.TurnRightForward(speed, rightTurnPercent);
                            }
                            else
                            {
                                if (leftTurnPerfect < 100)
                                {
                                    leftTurnPerfect += 10;
                                }
                                Console.WriteLine("Set vehicle left turn perfect to " + leftTurnPerfect);
                                brick.Vehicle.TurnLeftForward(speed, leftTurnPerfect);
                            }
                            break;

                        case ConsoleKey.RightArrow:
                            if (leftTurnPerfect > 0)
                            {
                                leftTurnPerfect -= 10;
                                Console.WriteLine("Set vehicle left turn percent to " + leftTurnPerfect);
                                brick.Vehicle.TurnLeftForward(speed, leftTurnPerfect);
                            }
                            else
                            {
                                if (rightTurnPercent < 100)
                                {
                                    rightTurnPercent += 10;
                                }
                                Console.WriteLine("Set vehicle right speed to " + rightTurnPercent);
                                brick.Vehicle.TurnRightForward(speed, rightTurnPercent);
                            }
                            break;

                        case ConsoleKey.O:
                            Console.WriteLine("Vehicle off");
                            speed = 0;
                            brick.Vehicle.Off();
                            break;

                        case ConsoleKey.B:
                            Console.WriteLine("Vehicle break");
                            speed = 0;
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