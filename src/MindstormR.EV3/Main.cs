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
                for (int i = 0; i < 5; i++)
                {
                    sw.Restart();
                    string data = client.DownloadString(baseUrl + 1020 + "/command");
                    sw.Stop();
                    Info("Command from robot 1020: '{0}'. ({1:n2})", data, sw.Elapsed.TotalSeconds);
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