using System;
using System.Diagnostics;
using MonoBrickFirmware;
using MonoBrickFirmware.Display.Dialogs;
using MonoBrickFirmware.Display;
using MonoBrickFirmware.Movement;
using System.Threading;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace MonoBrickHelloWorld
{
    class MainClass
    {
        private static int _id;

        public static void Main(string[] args)
        {
            try
            {
                new InfoDialog("Creating hub..", false).Show();
                var connection = new HubConnection("http://mindstormr.novucura.com/");

                new InfoDialog("Creating hub proxy.", false).Show();
                var hub = connection.CreateHubProxy("MindstormrHub");

                hub.On<int>("LoggedIn", id =>
                    {
                        _id = id;
                        new InfoDialog("Logged in: " + _id, true).Show();
                    });


                new InfoDialog("Connecting...", false).Show();

                // Set the connection timeout to 5 minutes.
                connection.TransportConnectTimeout = TimeSpan.FromSeconds(300);
                var sw = Stopwatch.StartNew();
                connection.Start().Wait(TimeSpan.FromSeconds(300));
                sw.Stop();

                new InfoDialog(string.Format("Connected in {0:n2}s", sw.Elapsed.TotalSeconds), true).Show();

                for (int i = 0; i < 5; i++)
                {
                    sw.Restart();
                    hub.Invoke("PushCommand", _id, "forward");
                    sw.Stop();
                    new InfoDialog(string.Format("Pushed command in {0:n2}ms", sw.Elapsed.TotalMilliseconds), true).Show();
                }

                for (int i = 0; i < 5; i++)
                {
                    sw.Restart();
                    hub.Invoke("GetCommand", _id);
                    sw.Stop();
                    new InfoDialog(string.Format("Retrieved command in {0:n2}ms", sw.Elapsed.TotalMilliseconds), true).Show();
                }
            }
            catch (Exception ex)
            {
                LcdConsole.Clear();
                LcdConsole.WriteLine(ex.Message);
                LcdConsole.WriteLine("");
            }
        }
    }
}