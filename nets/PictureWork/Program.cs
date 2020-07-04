using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;


namespace PictureWork
{
    class Program
    {
        static void Main(string[] args)
        {
            //Environment.SetEnvironmentVariable("SWI_HOME_DIR", @"D:\\Program Files (x86)\\swipl");
            Environment.SetEnvironmentVariable("Path", @"D:\\Program Files (x86)\\swipl\\bin");

            // Параметры
            string pathSrc = "../../../../../src2_big/"; // Путь к директории с фигурами
            Color srcFigColor = Color.FromArgb(127, 127, 127); // Цвет фигур(0, 0, 0) - черный 
            Size lstSize = new Size(4032, 864); // Размер листа
            int scale = 1; // Коэф-т масштабирования

            string pathTmp = "../../../../../tmp/";
            string pathRes = "../../../../../result/";

            // Масштабирование
            // create or clean tmp dir!
            InputHandling.ScaleWholeDirectory(pathSrc, pathTmp, scale);
            Size scaledLstSize = new Size(lstSize.Width / scale, lstSize.Height / scale);

            // Загрузка фигур
            Console.WriteLine("Starting process. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            List<Figure> data = Figure.LoadFigures(pathTmp, srcFigColor); 
            Console.WriteLine("Figure loading finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            
            // Поиск решения
            Console.WriteLine("Starting result finder. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            //SolutionChecker.RunCreatedTest();
            //var result = SolutionChecker.F1(data);
            //var result = SolutionChecker.Tryer360(data);
            //PrintResult(result);

            // Отображение решения
            /*
            List<ResultData> result = new List<ResultData>();
            result.Add(new ResultData("x1,199,39 x2,164,32"));
            OutputHandling.SaveResult(data, result, pathRes, 420, 100);*/

            Console.ReadLine();
        }

        static void GetAndSaveQuery(List<Figure> data)
        {
            string queryStr = "tryer360(" +
                    QueryCreator.GetPrologAllRotatedFigureArrayRepresentationWithAngle(data) +
                    ",Res).";
            var file1 = File.CreateText("generated_query.txt");
            file1.Write(queryStr);

            queryStr = "f1(" +
                    QueryCreator.GetPrologOriginalFigureArrayRepresentation(data) +
                    ",[],Res).";
            var file2 = File.CreateText("generated_query_f1.txt");
            file2.Write(queryStr);

            file1.Close();
            file2.Close();
        }

        static void SaveString(string s, string path)
        {
            File.AppendAllText(path, "\n");
            File.AppendAllText(path, s);
            /*
            var file = File.CreateText(path);
            file.Write(s);
            Console.WriteLine(s);*/
        }

        static void PrintResult(List<string> res)
        {
            Console.WriteLine();
            foreach (string s in res)
                Console.WriteLine(s);
            if (res == null || !res.Any())
                Console.WriteLine("Result is empty");
        }
        
    }
}
