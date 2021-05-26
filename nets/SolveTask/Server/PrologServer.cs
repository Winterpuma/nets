using System;
using DataClassLibrary;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Configuration;
using System.Net;
using System.Text;

namespace SolveTask.Server
{
    class PrologServer : IServer
    {
		private readonly int _timeoutMin;

        private static readonly string _qFName = "queryFile.pl";

		public string Adress { get; }

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
                if (ans == "NoAnswer")
                    return null;
                if (ans.Contains("Time limit exceeded"))
                    return null;

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };
                options.Converters.Add(new ResultFigPosConverter(options));

                return JsonSerializer.Deserialize<ResultData>(ans, options);
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
            string tmpFile = $"tmp_{_qFName}";
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

            byte[] responseArray = myWebClient.UploadFile(uriString, srcFilename);

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
    }
}
