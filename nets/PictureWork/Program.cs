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
            string pathSrc = @"D:\GitHub\pics\realData\"; // Путь к директории с фигурами
            string pathProlog = @"D:\GitHub\nets\nets\PictureWork\"; // Путь к директории с кодом пролога

            Color srcFigColor = Color.FromArgb(155, 155, 155); // Цвет фигур(0, 0, 0) - черный 
            Size lstSize = new Size(300, 100);//14612, 5055);//(1000, 800);//(3980, 820); // Размер листа
            double scale = 0.1; // Коэф-т масштабирования
            int angleStep = 360; // Шаг поворотов фигур
            
            string pathTmp = "../../../../../tmp/";
            string pathRes = "../../../../../result/";
            
            CleanDir(pathTmp);
            CleanDir(pathRes);


            // Загрузка из PDF и масштабирование
            //InputHandling.ConvertPDFDirToScaledImg(pathSrc, pathTmp, scale);

            // Масштабирование
            Size scaledLstSize = new Size((int)(lstSize.Width * scale), (int)(lstSize.Height * scale));
            InputHandling.ScaleWholeDirectory(pathSrc, pathTmp, scale);

            // Загрузка фигур
            Console.WriteLine("Starting process. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            List<Figure> data = Figure.LoadFigures(pathTmp, srcFigColor, angleStep, scale);
            data.Sort(Figure.CompareFiguresBySize);
            Console.WriteLine("Figure loading finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);

            // Группировка фигур по листам
            /*List<List<Figure>> preDefArr = new List<List<Figure>>();
            preDefArr.Add(FormOneListArrangement(data, 0, 1, 2, 3, 7));
            preDefArr.Add(FormOneListArrangement(data, 4, 5, 6, 8, 9, 13, 14));
            SortFigures(preDefArr);*/

            // Загрузка фигур в файл пролога
            FigureFileOperations.CreateNewFigFile(pathProlog + "figInfo.pl");
            FigureFileOperations.AddManyFigs(data, 1);

            int[] figInd = new int[data.Count];
            for (int i = 0; i < figInd.Length; i++)
                figInd[i] = i;



            // Поиск решения
            Console.WriteLine("Starting result finding. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            //var result = PrologSolutionFinder.GetAnyResult(scaledLstSize.Width, scaledLstSize.Height, scale, figInd);
            var preDefArr = SolutionChecker.GetWorkingArrangementPreDefFigs(data, scaledLstSize.Width, scaledLstSize.Height, scale);
            //List<ResultData> result = new List<ResultData>();
            //result.Add(res);
            var result = SolutionChecker.PlacePreDefinedArrangement(preDefArr, scaledLstSize.Width, scaledLstSize.Height, scale);


            // Отображение решения
            Console.WriteLine("Starting visualization. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            //OutputImage.SaveOneSingleListResult(data, result, scaledLstSize.Width, scaledLstSize.Height, pathRes);
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
