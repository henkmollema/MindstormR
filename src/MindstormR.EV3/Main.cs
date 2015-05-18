using System;
using System.Diagnostics;
using MindstormR.Core;
using MonoBrickFirmware;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Movement;
using System.Threading;
using System.Net;

namespace MonoBrickHelloWorld
{
    class MainClass
    {
        private static int _id;

        public static void Main(string[] args)
        {
            try
            {
                const string baseUrl = "http://test.henkmollema.nl/robot/";

                var sw = Stopwatch.StartNew();
                var client = new WebClient();
                string s = client.DownloadString(baseUrl + "login");
                sw.Stop();

                if (!int.TryParse(s, out _id))
                {
                    Info("Invalid ID from the webserver: '{0}'. ({1:n2}s)", true, "Error", s, sw.Elapsed.TotalSeconds);
                    return;
                }

                Info("Robot logged in. ID: {0}. ({1:n2}s)", _id, sw.Elapsed.TotalSeconds);

                var vehicle = new Vehicle(MotorPort.OutA, MotorPort.OutC);
                var robot = new Robot(vehicle);

                while (true)
                {
                    sw.Restart();
                    string data = client.DownloadString(baseUrl + _id + "/command");
                    sw.Stop();

                    Info("Command {2}: '{0}'. ({1:n2})", false, "Robot " + _id, data, sw.Elapsed.TotalSeconds, _id);

                    switch (data.ToLower())
                    {
                        case "fire":
                            // todo: fire a single shot.
                            var motor = new Motor(MotorPort.OutB);
                            motor.SetSpeed(100);
                            Thread.Sleep(1000);
                            motor.Brake();
                            break;

                        case "forward":
                            robot.Move(Movement.Forward);
                            LcdConsole.WriteLine("Speed: {0}", robot.Speed);
                            break;

                        case "backward":
                            robot.Move(Movement.Backward);
                            LcdConsole.WriteLine("Speed: {0}", robot.Speed);
                            break;

                        case "left":
                            robot.Move(Movement.Left);
                            LcdConsole.WriteLine("Steering: {0}", robot.Steering);
                            break;

                        case "right":
                            robot.Move(Movement.Right);
                            LcdConsole.WriteLine("Steering: {0}", robot.Steering);
                            break;
                    }

                    // todo: consider using signalr for this
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }

        private static void Info(string format, params object[] arg)
        {
            new InfoDialog(string.Format(format, arg), true).Show();
        }

        private static void Info(string format, bool waitForOk, params object[] arg)
        {
            new InfoDialog(string.Format(format, arg), waitForOk).Show();
        }

        private static void Info(string format, bool waitForOk, string title, params object[] arg)
        {
            new InfoDialog(string.Format(format, arg), waitForOk, title).Show();
        }
    }
}