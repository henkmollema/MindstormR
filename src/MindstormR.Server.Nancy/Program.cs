using System;
using System.Linq;
using System.Threading;
using Nancy.Hosting.Self;

namespace MindstormR.Server.Nancy
{
    internal class MainClass
    {
        public static void Main(string[] args)
        {
            const string uri = "http://localhost:8888";
            W("Starting MindstormR Nancy webserver at {0}", uri);

            var host = new NancyHost(new Uri(uri));
            host.Start();
            W("Nancy hosting started");

            // Under mono if you daemonize a process a Console.ReadLine will cause an EOF 
            // so we need to block another way.
            if (args.Any(s => s.Equals("-d", StringComparison.CurrentCultureIgnoreCase)))
            {
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                Console.ReadKey();
            }

            host.Stop();
            W("Hosting stopped");
        }

        private static void W(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}
