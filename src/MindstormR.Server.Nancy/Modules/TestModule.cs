using System;
using MindstormR.Core;
using MonoBrick.EV3;
using Nancy;

namespace MindstormR.Server.Nancy
{
    public class TestModule : NancyModule
    {
        private static Robot _robot;

        public TestModule()
        {
            Get["/"] = paramaters =>
            {
                if (_robot == null)
                {
                    //var brick = new Brick<Sensor, Sensor, Sensor, Sensor>("WiFi");
                    //_robot = new Robot(brick.Vehicle);
                }

                return "Lego Mindstorms EV3 control panel should be here.";
            };
        }
    }
}

