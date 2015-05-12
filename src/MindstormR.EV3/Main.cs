using System;
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
                string url = "http://test.henkmollema.nl/robot/login";
                WebClient client = new WebClient();
                string s = client.DownloadString(url);
                _id = int.Parse(s);

                new InfoDialog("Robot id: " + _id, true).Show();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
        }
    }
}