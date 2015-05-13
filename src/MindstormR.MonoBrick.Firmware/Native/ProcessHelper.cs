using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
namespace MonoBrickFirmware.Native
{
	public class ProcessHelper
	{
		public static void StartProcess (string fileName, string arguments = "")
		{
			Process proc = new System.Diagnostics.Process ();
			proc.EnableRaisingEvents = false; 
			if (arguments == "") {
				Console.WriteLine ("Starting process: {0}", fileName);
			} 
			else 
			{
				Console.WriteLine ("Starting process: {0} with arguments: {1}", fileName, arguments);
			}
			proc.StartInfo.FileName = fileName;
			proc.StartInfo.Arguments = arguments;
			proc.Start();
		}
		
		public static int RunAndWaitForProcess(string fileName, string arguments = "" , int timeout = 0){
			Process proc = new System.Diagnostics.Process ();
			if(timeout != 0){
				System.Timers.Timer timer = new System.Timers.Timer(timeout);
				timer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e) {
					timer.Stop();
					proc.Kill();
				};
				timer.Start();	
			}
			proc.EnableRaisingEvents = false; 
			if (arguments == "") {
				Console.WriteLine ("Starting process: {0}", fileName);
			} 
			else 
			{
				Console.WriteLine ("Starting process: {0} with arguments: {1}", fileName, arguments);
			}
			proc.StartInfo.FileName = fileName;
			proc.StartInfo.Arguments = arguments;
			proc.Start ();
			proc.WaitForExit ();
			return proc.ExitCode;	
		}
		
		public static string RunAndWaitForProcessWithOutput(string fileName, string arguments = "")
		{
			string result;
			ProcessStartInfo start = new ProcessStartInfo();
			start.FileName = fileName;
			start.Arguments = arguments; 
			start.UseShellExecute = false;
			start.RedirectStandardOutput = true;
			if (arguments == "") {
				Console.WriteLine ("Starting process: {0}", fileName);
			} 
			else 
			{
				Console.WriteLine ("Starting process: {0} with arguments: {1}", fileName, arguments);
			}using (Process process = Process.Start(start))
			{
			    using (StreamReader reader = process.StandardOutput)
			    {
					result = reader.ReadToEnd();
				}
				process.WaitForExit();
			}
			return result;
		}
		
		/// <summary>
		/// Gets the process identifier.
		/// </summary>
		/// <returns>The process id. Returns -1 if a process with the sechText is not running or isn't found</returns>
		/// <param name="seachText">Process search text.</param>
		public static int GetProcessId (string seachText)
		{
			string output = RunAndWaitForProcessWithOutput ("ps", "");
			string[] result = output.Split (new [] { '\r', '\n' });//split lines
			List<string> psStrings = new List<string> ();
			foreach (var s in result) {
				if (s.Contains (seachText)) {
					psStrings.Add (s);
				}
			}
			if (psStrings.Count >= 1) {
				//The process id is in the first line second word. Regex is used to remove whitespaces between words
				string processId = System.Text.RegularExpressions.Regex.Replace(psStrings[0],@"\s+"," ").Split(' ')[1];
				try{
					int id = int.Parse(processId);
					return id;
				}
				catch{
				
				}
			}
			return -1;
		}
		
		/// <summary>
		/// Determines if a process is running.
		/// </summary>
		/// <returns><c>true</c> if is process is running; otherwise false.</returns>
		/// <param name="seachText">Process seach text.</param>
		public static bool IsProcessRunning (string seachText)
		{
			bool running = (GetProcessId (seachText) != -1);
			Console.WriteLine("Is prcocess " + seachText + " running: " + running);
			return running;
		}
		
		/// <summary>
		/// Kills a process.
		/// </summary>
		/// <param name="seachText">Process seach text.</param>
		public static void KillProcess (string seachText)
		{
			KillProcess(GetProcessId(seachText));
		}
		
		/// <summary>
		/// Kills a process.
		/// </summary>
		/// <param name="id">Process id</param>
		public static void KillProcess (int id)
		{
			Console.WriteLine("Killing process with id: " + id);
			RunAndWaitForProcess("kill", id.ToString());
		}
		
		  
	}
}

