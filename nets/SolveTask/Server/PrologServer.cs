using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataClassLibrary;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Configuration;
using System.Net;

namespace SolveTask.Server
{
    class PrologServer
    {
        private readonly string serverAdress;
        private readonly int _timeoutMin;

        private static readonly string _qFName = "queryFile.pl";

        public PrologServer(string serverAdress)
        {
            this.serverAdress = serverAdress;
            _timeoutMin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("serverAnswerMinTimeout"));
        }
        
        /// <summary>
        /// Получение ответа соответствующей query
        /// </summary>
        /// <returns>Набор расположений фигур</returns>
        public ResultData GetAnyResult(string query)
        {
            CreaterQueryFileOnServer(query);

            try
            {
                string ans = getAns().Result;
                if (ans == "NoAnswer")
                    return null;
                if (ans.Contains("Time limit exceeded"))
                    return null;
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                };
                options.Converters.Add(new ResultFigPosConverter(options));
                ResultData result = JsonSerializer.Deserialize<ResultData>(ans, options);

                return result;
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
        public void UploadFile(string srcFilename, string dstFilename)
		{
            var uriString = $"{serverAdress}upload/{dstFilename}";
            var myWebClient = new WebClient();

            byte[] responseArray = myWebClient.UploadFile(uriString, srcFilename);

            // Decode and display the response.
            //Console.WriteLine("\nResponse Received. The contents of the file uploaded are:\n{0}",
            //    System.Text.Encoding.ASCII.GetString(responseArray));
        }
        
        /// <summary>
        /// Получение ответа myQuery предиката
        /// </summary>
        /// <returns>Ответ сервера</returns>
        public async Task<string> getAns()
        {
            HttpClient client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "val", "3" }
            };

            var content = new FormUrlEncodedContent(values);

            client.Timeout = TimeSpan.FromMinutes(_timeoutMin);
            var response = await client.PostAsync(serverAdress, content);

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
