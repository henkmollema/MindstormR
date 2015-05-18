using System;
using System.Collections.Generic;
using Nancy;

namespace MindstormR.Client.Nancy
{
    public class RobotModule : NancyModule
    {
        private static List<int> _clients = new List<int>();
        private static Dictionary<int, Queue<Command>> _commands = new Dictionary<int, Queue<Command>>();
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

            Get["{id:int}/command"] = PopCommand;
        }

        private dynamic Login(dynamic parameters)
        {
            int id = _id++;
            _clients.Add(id);
            _commands.Add(id, new Queue<Command>());
            return id.ToString();
        }

        private dynamic Logout(dynamic parameters)
        {
            _commands.Remove(parameters.id);
            _clients.Remove(parameters.id);
            return true.ToString();
        }

        private dynamic Flush(dynamic parameters)
        {
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
            // Push a command on the stack of the robot with the specified id.
            Queue<Command> commands;
            if (_commands.TryGetValue(id, out commands))
            {
                commands.Enqueue(command);
                return true.ToString();
            }

            return false.ToString();
        }

        private dynamic PopCommand(dynamic parameters)
        {
            // Pop the last command from the stack of the robot with the specified id.
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

