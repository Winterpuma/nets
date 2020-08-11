﻿using System;
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
        private static readonly string _mainName = "mainServer.pl";
        private static readonly string _qFName = "queryFile.pl";
        private static readonly string _queryName = "myQuery";


        static public bool IsInitialized = false;

        static public void Initialize(string codePath)
        {
            Environment.SetEnvironmentVariable("Path", prologPath);
            string strCmdText = "/C swipl " + codePath + _mainName;
            System.Diagnostics.Process.Start("CMD.exe", strCmdText);
            IsInitialized = true;
        }

        static public ResultData GetAnyResult(string query)
        {
            CreaterQueryFile(_qFName, query);
            string ans = getAns().Result;
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            options.Converters.Add(new ResultFigPosConverter(options));
            ResultData result = JsonSerializer.Deserialize<ResultData>(ans, options);

            return result;
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
