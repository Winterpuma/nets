using DataClassLibrary;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SolveTask.Server
{
	public interface IServer
	{
		string Adress{ get; }
		bool Busy { get; }
		TimeSpan BusyTime { get; }
		ResultData GetQueryResult(string query);
		Task<HttpResponseMessage> StartTask(string query);
		string UploadFile(string srcFilename, string dstFilename);
	}
}
