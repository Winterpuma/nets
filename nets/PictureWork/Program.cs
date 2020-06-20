using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SbsSW.SwiPlCs.Exceptions;
using SbsSW.SwiPlCs;
using System.Drawing;


namespace PictureWork
{
    class Program
    {
        static void Main(string[] args)
        {
            //Environment.SetEnvironmentVariable("SWI_HOME_DIR", @"D:\\Program Files (x86)\\swipl");
            Environment.SetEnvironmentVariable("Path", @"D:\\Program Files (x86)\\swipl\\bin");

            List<Figure> data = Figure.LoadFigures("../../../../../src2_blackandwhite", Color.FromArgb(0, 0, 0));
            //var s = QueryCreator.GetPrologDeltaRepresentation(data[0][0]);
            //var s = QueryCreator.GetPrologOriginalFigureArrayRepresentation(data);

            //SolutionChecker.GetSolutions(data);
            
            Console.ReadLine();
        }
        
    }
}
