using System;
using System.Collections.Generic;
using Nancy;

namespace MindstormR.Client.Nancy
{
    public class RobotModule : NancyModule
    {
        private static List<int> _clients = new List<int>();
        private static Dictionary<int, Queue<Command>> _commands = new Dictionary<int, Queue<Command>>();
        private static Dictionary<int, Dictionary<string, object>> _sensors = new Dictionary<int, Dictionary<string, object>>();
        private static int _id = 1000;

        public RobotModule()
            : base("robot")
        {
            Get["/login"] = Login;
            Get["{id:int}/logout"] = Logout;
            Get["/flush"] = Flush;

            Get["all"] = GetRobots;

            Get["{id:int}/forward"] = _ => PushCommand(_.id, Command.Forward);
            Get["{id:int}/backward"] = _ => PushCommand(_.id, Command.Backward);
            Get["{id:int}/left"] = _ => PushCommand(_.id, Command.Left);
            Get["{id:int}/right"] = _ => PushCommand(_.id, Command.Right);
            Get["{id:int}/fire"] = _ => PushCommand(_.id, Command.Fire);

            Get["{id:int}/command"] = GetCommand;

            Get["{id:int}/sensor/get/{sensor}"] = GetSensorValue;
            Get["{id:int}/sensor/push/{sensor}/{value}"] = PushSensorValue;
        }

        private dynamic Login(dynamic parameters)
        {
            int id = _id++;
            _clients.Add(id);
            _commands.Add(id, new Queue<Command>());
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

        private dynamic PushCommand(int id, Command command)
        {
            // Add a command to the queue of the robot with the specified id.
            Queue<Command> commands;
            if (_commands.TryGetValue(id, out commands))
            {
                commands.Enqueue(command);
                return true.ToString();
            }

            return false.ToString();
        }

        private dynamic GetCommand(dynamic parameters)
        {
            // Dequeue the last command from the queue of the robot with the specified id.
            Queue<Command> commands;
            if (_commands.TryGetValue(parameters.id, out commands))
            {
                if (commands.Count > 0)
                {
                    string s = commands.Dequeue().ToString();
                    return s;
                }
            }
            return false.ToString();
        }

        private dynamic GetSensorValue(dynamic parameters)
        {
            var data = _sensors[parameters.id];
            object value;
            if (data.TryGetValue(parameters.sensor, out value))
            {
                return value;
            }

            return null;
        }

        private dynamic PushSensorValue(dynamic parameters)
        {
            _sensors[parameters.id][parameters.sensor] = parameters.value;
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

