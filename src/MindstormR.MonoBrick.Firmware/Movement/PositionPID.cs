﻿using System;
using MonoBrickFirmware.Tools;
using MonoBrickFirmware.Display;
using System.Threading;
using System.Timers;

namespace MonoBrickFirmware.Movement
{
	public class PositionPID: PIDAbstraction 
	{
		private Motor motor;
		private float target;
		private bool brake;
		private const float SampleTime = 50; 
		private MovingAverage movingAverage = null;

		bool setAverage = true;

		public PositionPID (Motor motor, Int32 position, bool brake, sbyte maxPower, float P, float I, float D, int settleTimeMs): 
		base(P,I,D,SampleTime, (float) maxPower, -((float) maxPower))   
		{
			movingAverage = new MovingAverage((uint)((uint)settleTimeMs/(uint)SampleTime));
			this.motor = motor;
			target = position;
			this.brake = brake;
		}


		public WaitHandle Run(Int32 position)
		{
			target = position;
			return base.Run();	
		}

		protected override void ApplyOutput (float output)
		{
			if (output == 0.0f && brake) 
			{
				motor.Brake();
			} 
			else 
			{
				motor.SetPower((sbyte)output);
			}
		}
		
		protected override float CalculateError ()
		{
			float currentPosition = ((float)motor.GetTachoCount());
			return (float)(target - currentPosition);	
		}
		
		protected override bool StopLoop ()
		{
			float average;
			if (setAverage) 
			{
				movingAverage.FillWindow(currentError);
				setAverage = false;
			} 
			else 
			{
				movingAverage.Update(currentError);
			}
			average = movingAverage.GetAverage();	
			if (average <= 1.0f && average >= -1.0f) {
				if (brake) 
				{
					motor.Brake();
				}
				else 
				{
					motor.SetPower (0);

				}
				setAverage = true;
				return true;
			}
			bool errorOk = currentError == 0;
			bool outputOk = currentOutput == 0;
			return errorOk && outputOk;
		}
	}
}

