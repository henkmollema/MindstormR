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
            Get["/login"] = Login;
            Get["{id:int}/logout"] = Logout;
            Get["/flush"] = Flush;

            Get["all"] = GetRobots;

            Get["{id:int}/{command}"] = PushCommand;

            Get["{id:int}/command"] = GetCommand;

            Get["{id:int}/sensors/get"] = GetSensorValues;
            Get["{id:int}/sensors/push"] = PushSensorValues;
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

        private dynamic Flush(dynamic parameters)
        {
            _id = 1000;
            _commands.Clear();
            _clients.Clear();
            return true.ToString();
        }

        private dynamic GetRobots(dynamic parameters)
        {
            return Response.AsJson(_clients.ToArray());
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
            // Dequeue the last command from the queue of the robot with the specified id.
            Queue<string> commands;
            if (_commands.TryGetValue(parameters.id, out commands))
            {
                if (commands.Count > 0)
                {
                    return commands.Dequeue().ToString();
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

        private dynamic PushSensorValues(dynamic parameters)
        {
            int id = parameters.id;

            // Parse sensor values from query string params.
            dynamic q = Request.Query;
            _sensors[id]["touch"] = q["touch"];
            _sensors[id]["gyro"] = q["gyro"];
            _sensors[id]["color"] = q["color"];
            _sensors[id]["ir"] = q["ir"];
            return true.ToString();
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

