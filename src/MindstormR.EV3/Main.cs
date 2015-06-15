using System;
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
using MonoBrickFirmware.Management;

namespace MonoBrickHelloWorld
{
    class MainClass
    {
        private const string BaseUrl = "http://test.henkmollema.nl/robot/";
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
                string s = client.DownloadString(BaseUrl + "login");
                sw.Stop();

                if (!int.TryParse(s, out _id))
                {
                    Info("Invalid ID from the webserver: '{0}'. ({1:n2}s)", true, "Error", s, sw.Elapsed.TotalSeconds);
                    return;
                }

                Info("Robot logged in. ID: {0}. ({1:n2}s)", _id, sw.Elapsed.TotalSeconds);

                var vehicle = new Vehicle(MotorPort.OutA, MotorPort.OutC);
                var robot = new Robot(vehicle);

                var touchSensor = new EV3TouchSensor(SensorPort.In1);
                var gyroSensor = new EV3GyroSensor(SensorPort.In2, GyroMode.Angle);
                var colorSensor = new EV3ColorSensor(SensorPort.In3) { Mode = ColorMode.Color };
                var irSensor = new EV3IRSensor(SensorPort.In4, IRMode.Proximity);

                float b = Battery.Current;
                while (running)
                {
                    sw.Restart();

                    float current = Battery.Current;
                    if (current < b)
                    {
                        b = current;
                    }

                    // Get the new command and push the sensor data in one go.
                    string command = client.DownloadString(
                                         string.Format("{0}/{1}/command?touch={2}&gyro={3}&color={4}&ir={5}&battery={6}", 
                                             BaseUrl, 
                                             _id,
                                             touchSensor.ReadAsString(),
                                             gyroSensor.ReadAsString(),
                                             colorSensor.ReadAsString(),
                                             irSensor.ReadAsString(),
                                             (int)(b * 1000)));
                    sw.Stop();
                    Info("Command: '{0}' ({1:n2}ms)", false, "Robot " + _id, command, sw.Elapsed.TotalMilliseconds);

                    switch (command.ToLower())
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
                            break;

                        case "backward":
                            robot.Move(Movement.Backward);
                            break;

                        case "left":
                            robot.Move(Movement.Left);
                            break;

                        case "right":
                            robot.Move(Movement.Right);
                            break;
                    }

                    // todo: consider using signalr for this -> #18
                    Thread.Sleep(100);
                }

                vehicle.Off();
                Info("Logging out...", false, "Robot " + _id);
                client.DownloadString(BaseUrl + _id + "/logout");
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