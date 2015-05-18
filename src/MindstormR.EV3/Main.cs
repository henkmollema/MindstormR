using System;
using System.Diagnostics;
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

                while (true)
                {
                    sw.Restart();
                    string data = client.DownloadString(baseUrl + _id + "/command");
                    sw.Stop();

                    Info("Command {2}: '{0}'. ({1:n2})", false, "Robot " + _id, data, sw.Elapsed.TotalSeconds, _id);

                    switch (data.ToLower())
                    {
                        case "fire":
                            var motor = new Motor(MotorPort.OutB);
                            motor.SetSpeed(100);
                            Thread.Sleep(1000);
                            motor.Brake();
                            break;
                    }

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