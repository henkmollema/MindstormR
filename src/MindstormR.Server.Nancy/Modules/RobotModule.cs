using System;
using Nancy;
using System.Collections.Generic;

namespace MindstormR.Client.Nancy
{

    public class RobotModule : NancyModule
    {
        private static List<int> _clients = new List<int>();
        private static int _id = 1000;

        public RobotModule()
            : base("robot")
        {
            Get["/login"] = _ =>
            {
                int id = _id++;
                _clients.Add(id);
                return id.ToString();
            };

            Get["logout/{id:int}"] = _ =>
            {
                _clients.Remove(_.id);
                return true.ToString();
            };
        }
    }
}

