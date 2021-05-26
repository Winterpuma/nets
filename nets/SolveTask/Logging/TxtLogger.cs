using System;
using System.IO;

namespace SolveTask.Logging
{
	class TxtLogger : Logger
	{

		const string FileName = "log.txt";
		readonly string Path;

		public TxtLogger(string pathToDir)
		{
			Path = pathToDir + FileName;
		}

		public void Log(string msg)
		{
			File.AppendAllText(Path, GetMsgWithTimeStamp(msg));
		}
	}
}
