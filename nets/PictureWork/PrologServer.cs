using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataClassLibrary;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Configuration;

namespace PictureWork
{
    static class PrologServer
    {
        // C:\Program Files\swipl\bin
        static string serverAdress;// = "http://localhost:8080/";
        static string pathPrologBin;// = @"C:\Program Files\swipl\bin";
        static string codePath = "";
        private static readonly string _mainName = "mainServer.pl";
        private static readonly string _qFName = "queryFile.pl";

        private static readonly int _timeoutMin;// = 5;


        static public bool IsInitialized = false;

        static public void Initialize()
        {
            Environment.SetEnvironmentVariable("Path", pathPrologBin);
            if (!File.Exists(codePath + _mainName))
                throw new Exception("main code path \"" + codePath + _mainName + "\" doesn't exist");
            string strCmdText = "/C swipl " + codePath + _mainName;
            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
            IsInitialized = true;
        }
        static PrologServer()
        {
            codePath = ConfigurationManager.AppSettings.Get("pathPrologCode");
            pathPrologBin = ConfigurationManager.AppSettings.Get("pathPrologBin");
            serverAdress = ConfigurationManager.AppSettings.Get("serverAdress");
            _timeoutMin = Convert.ToInt32(ConfigurationManager.AppSettings.Get("serverAnswerMinTimeout"));
        }

        private static ResultData GetAnyResult(string query)
        {
            if (!IsInitialized)
                Initialize();
            CreaterQueryFile(codePath + _qFName, query);

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
                file.WriteLine(strToAppend);//_queryName + "(Ans) :- " + strToAppend);
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

            client.Timeout = TimeSpan.FromMinutes(_timeoutMin);
            var response = await client.PostAsync(serverAdress, content);

            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

    }
}
