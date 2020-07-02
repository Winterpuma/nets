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

            Console.WriteLine("Starting process.");

            //List<Figure> data = Figure.LoadFigures("../../../../../src0_bigger", Color.FromArgb(0, 0, 0));
            List<Figure> data = Figure.LoadFigures("../../../../../src2_normal", Color.FromArgb(127, 127, 127));
            //List<Figure> data = Figure.LoadFigures("../../../../../src2_blackandwhite", Color.FromArgb(0, 0, 0));



            //SaveString(QueryCreator.CreateSimpleTest(401, 85, data[0], data[1]), "GenerateTest.txt");

            
            Console.WriteLine("Figure loading finished");
            //var s = QueryCreator.GetPrologDeltaRepresentation(data[0][0]);
            //var s = QueryCreator.GetPrologOriginalFigureArrayRepresentation(data);
            

            Console.WriteLine("Starting result finder.");

            //var result = SolutionChecker.F1(data);
            //var result = SolutionChecker.Tryer360(data);
            //PrintResult(result);

            
            List<ResultData> result = new List<ResultData>();
            result.Add(new ResultData("x1,199,39 x2,164,32"));
            OutputHandling.SaveResult(data, result, "../../../../../result/", 420, 100);

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
            var file = File.CreateText(path);
            file.Write(s);
            Console.WriteLine(s);
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
