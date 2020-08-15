using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataClassLibrary;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PictureWork
{
    static class PrologServer
    {
        // C:\Program Files\swipl\bin
        static string serverAdress = "http://localhost:8080/";
        static string prologPath = @"C:\Program Files\swipl\bin";
        static string codePath = "";
        private static readonly string _mainName = "mainServer.pl";
        private static readonly string _qFName = "queryFile.pl";
        private static readonly string _queryName = "myQuery";


        static public bool IsInitialized = false;

        static public void Initialize()
        {
            Environment.SetEnvironmentVariable("Path", prologPath);
            string strCmdText = "/C swipl " + codePath + _mainName;
            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
            IsInitialized = true;
        }

        private static ResultData GetAnyResult(string query)
        {
            if (!IsInitialized)
                Initialize();
            CreaterQueryFile(_qFName, query);
            try
            {
                string ans = getAns().Result;
                if (ans == "NoAnswer")
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
        /// Получение ответа для одного размера с нуля
        /// </summary>
        public static ResultData GetAnyResult(int width, int height, double scale, List<int> figInd)
        {
            Console.WriteLine("~~~~~~ SCALE: " + scale);
            // Предполагается, что файл с фигурами уже загружен
            string queryStr = QueryCreator.GetAnsQuery(width, height, scale, figInd);
            return GetAnyResult(queryStr);
        }

        /// <summary>
        /// Получение ответа для одного размера на основе предыдущего размещения в другом масштабе
        /// </summary>
        public static ResultData GetAnyResult(int width, int height, double scale, ResultData prevScaleRes, List<int> figInd)
        {
            Console.WriteLine("~~~~~~ SCALE: " + scale);
            // Предполагается, что файл с фигурами уже загружен
            string queryStr = QueryCreator.GetAnsQuery(width, height, scale, prevScaleRes, figInd);
            Console.WriteLine(queryStr);
            return GetAnyResult(queryStr);
        }

        /// <summary>
        /// Получение ответа, основанного на последовательной итерации размеров фигур внутри пролога
        /// </summary>
        public static ResultData GetAnyResult(List<int> width, List<int> height, List<double> scales, List<int> figInd)
        {
            // Предполагается, что файл с фигурами уже загружен
            string queryStr = QueryCreator.GetAnsQuery(width, height, scales, figInd);
            return GetAnyResult(queryStr);
        }


        private static void CreaterQueryFile(string fileName, string strToAppend)
        {
            using (StreamWriter file =
                new StreamWriter(fileName))
            {
                file.WriteLine(_queryName + "(Ans) :- " + strToAppend);
            }

        }

        
            
        public static async Task<string> getAns()
        {
            HttpClient client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                { "val", "3" }
            };

            var content = new FormUrlEncodedContent(values);
            
            var response = await client.PostAsync(serverAdress, content);

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

    }
}
