﻿using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MindstormR.Core;
using MonoBrickFirmware;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.Movement;
using MonoBrickFirmware.UserInput;
using MonoBrickFirmware.Sensors;

namespace MonoBrickHelloWorld
{
    class MainClass
    {
        private const string baseUrl = "http://test.henkmollema.nl/robot/";
        private static int _id;

        public static void Main(string[] args)
        {
            try
            {
                var terminateProgram = new ManualResetEvent(false);
                bool running = true;
                var buttons = new ButtonEvents();

                buttons.EscapePressed += () =>
                {
                    // Quit when escape is pressed.
                    running = false;
                    terminateProgram.Set();
                };

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
                var sensor = new EV3ColorSensor(SensorPort.In3) { Mode = ColorMode.Color };

                while (running)
                {
                    sw.Restart();
                    string data = client.DownloadString(baseUrl + _id + "/command");
                    sw.Stop();

                    string color = sensor.ReadAsString();
                    Info("'{0}'/'{1}' ({2:n2}ms)", false, "Robot " + _id, data, color, sw.Elapsed.TotalMilliseconds);

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

                    // todo: consider using signalr for this -> #18
                    Thread.Sleep(250);

                    client.DownloadData(string.Format("{0}/{1}/sensor/push/{2}/{3}", baseUrl, _id, "color", sensor.ReadAsString()));
                    Thread.Sleep(250);
                }

                vehicle.Off();
                Info("Logging out...", false, "Robot " + _id);
                client.DownloadString(baseUrl + _id + "/logout");
                Info("Logged out", false, "Robot " + _id);
                terminateProgram.WaitOne();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                new InfoDialog(ex.Message, true).Show();
            }
        }

        private static void Info(string format, params object[] arg)
        {
            new InfoDialog(string.Format(format, arg), true).Show();
        }

        private static void Info(string format, bool waitForOk, string title, params object[] arg)
        {
            new InfoDialog(string.Format(format, arg), waitForOk, title).Show();
        }
    }
}