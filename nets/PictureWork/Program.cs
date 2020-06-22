using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SbsSW.SwiPlCs.Exceptions;
using SbsSW.SwiPlCs;
using System.Drawing;
using System.Linq;
using System.Diagnostics;


namespace PictureWork
{
    class Program
    {
        static void Main(string[] args)
        {
            //Environment.SetEnvironmentVariable("SWI_HOME_DIR", @"D:\\Program Files (x86)\\swipl");
            Environment.SetEnvironmentVariable("Path", @"D:\\Program Files (x86)\\swipl\\bin");

            //List<Figure> data = Figure.LoadFigures("../../../../../src0_simple_test", Color.FromArgb(0, 0, 0));
            //List<Figure> data = Figure.LoadFigures("../../../../../src3", Color.FromArgb(0, 0, 0));

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<Figure> data = Figure.LoadFigures("../../../../../src2_blackandwhite", Color.FromArgb(0, 0, 0));
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed.Seconds);
            //var s = QueryCreator.GetPrologDeltaRepresentation(data[0][0]);
            //var s = QueryCreator.GetPrologOriginalFigureArrayRepresentation(data);

            //var result = SolutionChecker.GetSolutions(data);
            //PrintResult(result);

            Bitmap test = new Bitmap(2000, 2000);
            OutputHandling.PlaceDeltasOnABitmap(test, data[0][3], 1000, 1000, Color.Pink);
            test.Save("../../../../../resultcheck/mmm.png");


            Console.ReadLine();
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
