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
            Size lstSize = new Size(3980, 820); // Размер листа
            int scale = 20; // Коэф-т масштабирования

            string pathTmp = "../../../../../tmp/";
            string pathRes = "../../../../../result/";

            CleanDir(pathTmp);
            CleanDir(pathRes);

            // Масштабирование
            InputHandling.ScaleWholeDirectory(pathSrc, pathTmp, scale);
            Size scaledLstSize = new Size(lstSize.Width / scale, lstSize.Height / scale);

            // Загрузка фигур
            Console.WriteLine("Starting process. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            List<Figure> data = Figure.LoadFigures(pathTmp, srcFigColor); 
            Console.WriteLine("Figure loading finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            
            // Поиск решения
            Console.WriteLine("Starting result finder. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            var result = SolutionChecker.CreateAndRunTest(data, scaledLstSize.Width, scaledLstSize.Height);

            // Отображение решения
            Console.WriteLine("Starting visualization. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            OutputHandling.SaveResult(data, result, pathRes, scaledLstSize.Width, scaledLstSize.Height);

            Console.WriteLine("Process finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            Console.ReadLine();
        }       
        
        private static void CleanDir(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo curDir in dir.GetDirectories())
            {
                curDir.Delete(true);
            }
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
