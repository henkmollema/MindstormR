using System;
using MonoBrick;

namespace MindstormR.Core
{
    /// <summary>
    /// A Lego Mindstorms robot.
    /// </summary>
    public class Robot
    {
        private readonly IVehicle _vehicle;
        private sbyte _speed;
        private sbyte _steering;

        /// <summary>
        /// Initializes a new instance of the <see cref="Robot"/> class using the specified vehicle.
        /// </summary>
        /// <param name="vehile">The lego vehicle instance.</param>
        public Robot(IVehicle vehile)
        {
            _vehicle = vehile;
        }

        /// <summary>
        /// Moves the robot in the specified direction.
        /// </summary>
        /// <param name="movement">The direction to move the robot to.</param>
        public void Move(Movement movement)
        {
            switch (movement)
            {
                case Movement.Forward:
                case Movement.Backward:
                    if (movement == Movement.Forward)
                    {
                        // Moving forward.
                        if (_speed < 100)
                        {
                            // Increment the speed by 10 until 100.
                            _speed += 10;
                        }
                    }
                    else
                    {
                        // Moving backward.
                        if (_speed > -100)
                        {
                            // Decrement the speed by 10 until -100.
                            _speed -= 10;
                        }
                    }

                    if (NotSteering)
                    {
                        // No steering, just straight forward.
                        _vehicle.Forward(_speed);
                    }
                    else
                    {
                        if (SteeringLeft)
                        {
                            if (movement == Movement.Forward)
                            {
                                _vehicle.TurnLeftForward(_speed, Math.Abs(_steering));
                            }
                            else
                            {
                                // todo: test if we need to reverse right or left (don't forget Math.Abs)
                                _vehicle.TurnRightReverse(_speed, _steering);
                            }
                        }
                        else
                        {
                            if (movement == Movement.Forward)
                            {
                                // Steering to the right.
                                _vehicle.TurnRightForward(_speed, _steering);
                            }
                            else
                            {
                                // todo: test if we need to reverse right or left (don't forget Math.Abs)
                                _vehicle.TurnLeftReverse(_speed, Math.Abs(_steering));
                            }
                        }
                    }
                    break;

                case Movement.Left:
                case Movement.Right:
                    if (movement == Movement.Right)
                    {
                        // Moving to the right.
                        if (_steering < 100)
                        {
                            // Increment the steering until 100.
                            _steering += 10;
                        }
                    }
                    else
                    {
                        // Moving to the left.
                        if (_steering > -100)
                        {
                            // Decrement. the steering until -100.
                            _steering -= 10;
                        }
                    }

                    if (NotMoving)
                    {
                        // Vehicle is not moving forward/backward, spin the vehicle.
                        if (SteeringRight)
                        {
                            // Spin to the right.
                            _vehicle.SpinRight(_steering);
                        }
                        else
                        {
                            // Spin to the left.
                            _vehicle.SpinLeft(Math.Abs(_steering));
                        }
                    }
                    else
                    {
                        // Vehile is moving forward/backward, turn the vehicle.
                        if (SteeringRight)
                        {
                            // Turn to the right.
                            if (MovingForward)
                            {
                                _vehicle.TurnRightForward(_speed, _steering);
                            }
                            else
                            {
                                // Vehicle is moving backward.
                                _vehicle.TurnLeftReverse(Math.Abs(_speed), _steering);
                            }
                        }
                        else
                        {
                            // Turn to the left.
                            if (MovingForward)
                            {
                                _vehicle.TurnLeftForward(_speed, Math.Abs(_steering));
                            }
                            else
                            {
                                // Vehicle is moving backward.
                                // todo: test backwrd steering.
                                _vehicle.TurnRightReverse(Math.Abs(_speed), Math.Abs(_steering));
                            }
                        }
                    }
                    break;
            }
        }

        public sbyte Speed
        {
            get
            {
                return _speed;
            }
        }

        public sbyte Steering
        {
            get
            {
                return _steering;
            }
        }

        public bool NotMoving
        {
            get
            {
                return _speed == 0;
            }
        }

        public bool MovingForward
        {
            get
            {
                return _speed > 0;
            }
        }

        public bool MovingBackward
        {
            get
            {
                return _speed < 100;
            }
        }

        public bool NotSteering
        {
            get
            {
                return _steering == 0;
            }
        }

        public bool SteeringLeft
        {
            get
            {
                return _steering < 0;
            }
        }

        public bool SteeringRight
        {
            get
            {
                return _steering > 0;
            }
        }
    }

    /// <summary>
    /// Represents a move a robot can perform.
    /// </summary>
    public enum Movement
    {
        /// <summary>
        /// Moves the robot forward.
        /// </summary>
        Forward,

        /// <summary>
        /// Moves the robot backward.
        /// </summary>
        Backward,

        /// <summary>
        /// Moves the robot to the left.
        /// </summary>
        Left,

        /// <summary>
        /// Moves the robot to the right.
        /// </summary>
        Right
    }
}
