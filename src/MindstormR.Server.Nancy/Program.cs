using System;
using System.Diagnostics;
using Nancy.Hosting.Self;

namespace MindstormR.Server.Nancy
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            using (var nancyHost = new NancyHost(new Uri("http://localhost:8888/")))
            {
                nancyHost.Start();

                Console.WriteLine("Nancy now listening - navigating to http://localhost:8888/nancy/. Press enter to stop");
                try
                {
                    Process.Start("http://localhost:8888/");
                }
                catch (Exception)
                {
                }
                Console.ReadKey();
            }

            Console.WriteLine("Stopped. Good bye!");
        }
    }
}
