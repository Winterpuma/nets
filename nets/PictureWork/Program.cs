using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using DataClassLibrary;
using IO;


namespace PictureWork
{
    class Program
    {
        static void Main(string[] args)
        {
            //Environment.SetEnvironmentVariable("SWI_HOME_DIR", @"D:\\Program Files (x86)\\swipl");
            Environment.SetEnvironmentVariable("Path", @"D:\\Program Files (x86)\\swipl\\bin");

            // Параметры
            string pathSrc = "../../../../../bmpKips2/"; // Путь к директории с фигурами
            Color srcFigColor = Color.FromArgb(155, 155, 155); // Цвет фигур(0, 0, 0) - черный 
            Size lstSize = new Size(14612, 5055);//(1000, 800);//(3980, 820); // Размер листа
            int scale = 50; // Коэф-т масштабирования
            int angleStep = 120; // Шаг поворотов фигур
            
            string pathTmp = "../../../../../tmp/";
            string pathRes = "../../../../../result/";
            
            //CleanDir(pathTmp);
            CleanDir(pathRes);


            // Загрузка из PDF и масштабирование
            //InputHandling.ConvertPDFDirToScaledImg(pathSrc, pathTmp, scale);

            // Масштабирование
            //InputHandling.ScaleWholeDirectory(pathSrc, pathTmp, scale);
            Size scaledLstSize = new Size(lstSize.Width / scale, lstSize.Height / scale);

            // Загрузка фигур
            Console.WriteLine("Starting process. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            List<Figure> data = Figure.LoadFigures(pathTmp, srcFigColor, angleStep);
            data.Sort(Figure.CompareFiguresBySize);
            Console.WriteLine("Figure loading finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);

            // Группировка фигур по листам
            /*List<List<Figure>> preDefArr = new List<List<Figure>>();
            preDefArr.Add(FormOneListArrangement(data, 0, 1, 2, 3, 7));
            preDefArr.Add(FormOneListArrangement(data, 4, 5, 6, 8, 9, 13, 14));
            SortFigures(preDefArr);*/

            
            // Поиск решения
            Console.WriteLine("Starting result finding. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            //var res = SolutionChecker.FindMinArrangementDec(data, scaledLstSize.Width, scaledLstSize.Height);
            //var res = SolutionChecker.FindMinArrangementHalfDiv(data, scaledLstSize.Width, scaledLstSize.Height);
            var preDefArr = SolutionChecker.GetWorkingArrangement(data, scaledLstSize.Width, scaledLstSize.Height);
            //List<ResultData> result = new List<ResultData>();
            //result.Add(res);
            var result = SolutionChecker.PlacePreDefinedArrangement(preDefArr, scaledLstSize.Width, scaledLstSize.Height);
            

            // Отображение решения
            Console.WriteLine("Starting visualization. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            //OutputHandling.SaveResult(data, result, pathRes, scaledLstSize.Width, scaledLstSize.Height);
            OutputImage.SaveResult(preDefArr, result, pathRes, scaledLstSize.Width, scaledLstSize.Height);

            Console.WriteLine("Process finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            Console.ReadLine();
        }       
        
        public static void CleanDir(string path)
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

        static void SortFigures(List<List<Figure>> arr)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                arr[i].Sort(Figure.CompareFiguresBySize);
            }
        }

        static List<Figure> FormOneListArrangement(List<Figure> data, params int[] ind)
        {
            List<Figure> res = new List<Figure>();
            foreach(int i in ind)
            {
                res.Add(data[i]);
            }
            return res;
        }
        
    }
}
