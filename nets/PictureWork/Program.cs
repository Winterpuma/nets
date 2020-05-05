using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SbsSW.SwiPlCs.Exceptions;
using SbsSW.SwiPlCs;


namespace PictureWork
{
    class Program
    {
        static void Main(string[] args)
        {
            //Environment.SetEnvironmentVariable("SWI_HOME_DIR", @"D:\\Program Files (x86)\\swipl");
            Environment.SetEnvironmentVariable("Path", @"D:\\Program Files (x86)\\swipl\\bin");
            
            //List<Figure> data = LoadFigures("../../../../src");
            PlEngineExamples.Ex3();
            
            Console.ReadLine();
        }
        
    }
}
