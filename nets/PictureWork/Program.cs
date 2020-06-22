using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SbsSW.SwiPlCs.Exceptions;
using SbsSW.SwiPlCs;
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

            //List<Figure> data = Figure.LoadFigures("../../../../../src0_simple_test", Color.FromArgb(0, 0, 0));
            List<Figure> data = Figure.LoadFigures("../../../../../src3", Color.FromArgb(0, 0, 0));
            //List<Figure> data = Figure.LoadFigures("../../../../../src2_blackandwhite", Color.FromArgb(0, 0, 0));

            //var s = QueryCreator.GetPrologDeltaRepresentation(data[0][0]);
            //var s = QueryCreator.GetPrologOriginalFigureArrayRepresentation(data);

            var result = SolutionChecker.GetSolutions(data);
            PrintResult(result);
            
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
