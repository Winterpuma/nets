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
            Example();
            
            Console.ReadLine();
        }
        
        
        
        public static void Example()
        {            
            string filename = Path.GetTempFileName();
            StreamWriter sw = File.CreateText(filename);
            sw.WriteLine("father(martin, inka).");
            sw.WriteLine("father(uwe, gloria).");
            sw.WriteLine("father(uwe, melanie).");
            sw.WriteLine("father(daddy, ayala).");
            sw.Close();
            
            String[] param = {"-q", "-f", filename};
            try
            {
                PlEngine.Initialize(param);
                Console.WriteLine("uwe children:");
                using (PlQuery q = new PlQuery("father(uwe, Child)"))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        Console.WriteLine(v["Child"].ToString());
                    }
                }
                Console.WriteLine("daddy children:");
                using (PlQuery q = new PlQuery("father(daddy, Child)"))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        Console.WriteLine(v["Child"].ToString());
                    }
                }
            }
            catch (PlException e)
            {
                Console.WriteLine(e.MessagePl);
                Console.WriteLine(e.Message);
            }
            finally
            {
                PlEngine.PlCleanup();
            }
        }
    }
}
