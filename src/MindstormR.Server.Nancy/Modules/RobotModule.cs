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
            Get["/login"] = Login;

            Get["logout/{id:int}"] = Logout;
        }

        private dynamic Login(dynamic parameters)
        {
            int id = _id++;
            _clients.Add(id);
            return id.ToString();
        }

        private dynamic Logout(dynamic parameters)
        {
            _clients.Remove(parameters.id);
            return true.ToString();
        }
    }

    public class RobotBase
    {
        public int Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is RobotBase)
            {
                return Id == ((RobotBase)obj).Id;
            }

            return base.Equals(obj);
        }
    }
}

