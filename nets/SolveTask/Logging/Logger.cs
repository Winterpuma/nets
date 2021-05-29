using System;

namespace SolveTask.Logging
{
	public class Logger
	{
		public string TimeStamp { get => $"[{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}]"; }

		protected string GetMsgWithTimeStamp(string msg) =>
			$"{TimeStamp} {msg}\n";
	}
}
