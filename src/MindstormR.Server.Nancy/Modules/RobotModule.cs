using System;
using System.Collections.Generic;
using Nancy;

namespace MindstormR.Client.Nancy
{
    public class RobotModule : NancyModule
    {
        private static List<int> _clients = new List<int>();
        private static Dictionary<int, Queue<string>> _commands = new Dictionary<int, Queue<string>>();
        private static Dictionary<int, Dictionary<string, object>> _sensors = new Dictionary<int, Dictionary<string, object>>();
        private static int _id = 1000;

        public RobotModule()
            : base(modulePath: "robot")
        {
            Get["login"] = Login;
            Get["{id:int}/logout"] = Logout;
            Get["all"] = GetRobots;
            Get["flush"] = Flush;

            Get["{id:int}/{command}"] = PushCommand;
            Get["{id:int}/command"] = GetCommand;
            Get["{id:int}/sensors/get"] = GetSensorValues;
        }

        private dynamic Login(dynamic parameters)
        {
            int id = _id++;
            _clients.Add(id);
            _commands.Add(id, new Queue<string>());
            _sensors.Add(id, new Dictionary<string, object>());
            return id.ToString();
        }

        private dynamic Logout(dynamic parameters)
        {
            _sensors.Remove(parameters.id);
            _commands.Remove(parameters.id);
            _clients.Remove(parameters.id);
            return true.ToString();
        }

        private dynamic GetRobots(dynamic parameters)
        {
            return Response.AsJson(_clients.ToArray());
        }

        private dynamic Flush(dynamic parameters)
        {
            _id = 1000;
            _sensors.Clear();
            _commands.Clear();
            _clients.Clear();
            return true.ToString();
        }

        private dynamic PushCommand(dynamic parameters)
        {
            int id = parameters.id;

            // Add a command to the queue of the robot with the specified id.
            Queue<string> commands;
            if (_commands.TryGetValue(id, out commands))
            {
                commands.Enqueue(parameters.command);
                return true.ToString();
            }

            return false.ToString();
        }

        private dynamic GetCommand(dynamic parameters)
        {
            int id = parameters.id;

            // Parse sensor values from query string params.
            dynamic q = Request.Query;
            _sensors[id]["touch"] = q["touch"];
            _sensors[id]["gyro"] = q["gyro"];
            _sensors[id]["color"] = q["color"];
            _sensors[id]["ir"] = q["ir"];
            _sensors[id]["battery"] = q["battery"];

            // Dequeue the last command from the queue of the robot with the specified id.
            Queue<string> commands;
            if (_commands.TryGetValue(id, out commands))
            {
                if (commands.Count > 0)
                {
                    return commands.Dequeue();
                }
            }

            return false.ToString();
        }

        private dynamic GetSensorValues(dynamic parameters)
        {
            // Serialize dictionary with commands to JSON.
            var data = _sensors[parameters.id];
            return Response.AsJson(new { data });
        }
    }

    public enum Command
    {
        Forward,
        Backward,
        Left,
        Right,
        Fire
    }
}

