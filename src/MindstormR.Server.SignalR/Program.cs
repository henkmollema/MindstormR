using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;

namespace MindstormR.Server.SignalR
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://*:8888";
            var app = WebApp.Start(url);

            Console.WriteLine("Server running on {0}", url);

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

            app.Dispose();
            app = null;
            Console.WriteLine("Hosting SignalR stopped");
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage(Microsoft.Owin.Diagnostics.ErrorPageOptions.ShowAll);

            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR(new HubConfiguration { EnableDetailedErrors = true });
        }
    }

    public class MindstormrHub : Hub
    {
        private static int _id = 1000;
        private static Dictionary<string, int> _clients = new Dictionary<string, int>();

        public override Task OnConnected()
        {
            string conId = Context.ConnectionId;
            int id = _id++;
            _clients.Add(conId, id);

            Clients.Caller.LoggedIn(id);

            return base.OnConnected();
        }
    }
}
