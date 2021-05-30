using System;
using DataClassLibrary;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SolveTask.Server
{
    class PrologServer : IServer
    {
		private readonly int _timeoutMin;

        private static readonly string _qFName = "queryFile.pl";

		public string Adress { get; }
        public bool Busy { get => currentTask?.Status == TaskStatus.Running; }
		public TimeSpan BusyTime { get => Busy ? DateTime.Now - startTime : TimeSpan.Zero; }

		public Task<HttpResponseMessage> currentTask;
        private DateTime startTime;

		public PrologServer(string serverAdress)
        {
            Adress = serverAdress;
            _timeoutMin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("serverAnswerMinTimeout"));
        }

        /// <summary>
        /// Получение ответа соответствующей query
        /// </summary>
        /// <returns>Набор расположений фигур</returns>
        public ResultData GetQueryResult(string query)
        {
            CreaterQueryFileOnServer(query);

            try
            {
                string ans = GetAnswer();
                return HandleServerResponce(ans);
            }
            catch (AggregateException)
            {
                Console.WriteLine("Exit by timer");
                return null;
            }
        }

        /// <summary>
        /// Создание файла запроса на сервера
        /// </summary>
        /// <param name="query">Текст запроса</param>
        private void CreaterQueryFileOnServer(string query)
        {
            string tmpFile = $"tmp{Guid.NewGuid()}_{_qFName}";
            using (StreamWriter file =
                new StreamWriter(tmpFile))
            {
                file.WriteLine(query);//_queryName + "(Ans) :- " + strToAppend);
            }

            UploadFile(tmpFile, _qFName);
        }

        /// <summary>
        /// Загрузка файла на сервер
        /// </summary>
        /// <param name="srcFilename">Загружаемый файл</param>
        /// <param name="dstFilename">Название файла на сервере</param>
        public string UploadFile(string srcFilename, string dstFilename)
        {
            var uriString = $"{Adress}upload/{dstFilename}";
            var myWebClient = new WebClient();

            byte[] responseArray;

            try
			{
                responseArray = myWebClient.UploadFile(uriString, srcFilename);
			}
            catch (WebException)
			{
                Thread.Sleep(TimeSpan.FromSeconds(5));
                responseArray = myWebClient.UploadFile(uriString, srcFilename);
            }

            return Encoding.ASCII.GetString(responseArray);
        }

        /// <summary>
        /// Получение ответа myQuery предиката
        /// </summary>
        /// <returns>Ответ сервера</returns>
        public string GetAnswer()
        {
			HttpClient client = new HttpClient
			{
				Timeout = TimeSpan.FromMinutes(_timeoutMin)
			};

			var response = client.GetAsync(Adress).Result;

            return response.Content.ReadAsStringAsync().Result;
        }


        public Task<HttpResponseMessage> StartTask(string query)
        {
            CreaterQueryFileOnServer(query);

            HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(_timeoutMin)
            };

            var task = client.GetAsync(Adress);
            startTime = DateTime.Now;
            currentTask = task;

            return task;
        }

        public static ResultData HandleServerResponce(string responce)
		{
            if (responce == "NoAnswer")
                return null;
            if (responce.Contains("Time limit exceeded"))
                return null;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            options.Converters.Add(new ResultFigPosConverter(options));

            ResultData res;
            try
			{
                res = JsonSerializer.Deserialize<ResultData>(responce, options);
            }
            catch (Exception)
			{
                return null;
			}

            return res;
        }
	}
}
