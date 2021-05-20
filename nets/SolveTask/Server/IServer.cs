using DataClassLibrary;

namespace SolveTask.Server
{
	interface IServer
	{
		string Adress{ get; }
		ResultData GetQueryResult(string query);
		string UploadFile(string srcFilename, string dstFilename);
	}
}
