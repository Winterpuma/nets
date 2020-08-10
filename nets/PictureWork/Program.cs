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
        // Параметры
        static string pathSrc = @"D:\GitHub\pics\realData0\"; // Путь к директории с фигурами
        static string pathProlog = @"D:\GitHub\nets\nets\PictureWork\"; // Путь к директории с кодом пролога


        static Color srcFigColor = Color.FromArgb(155, 155, 155); // Цвет фигур(0, 0, 0) - черный 
        static Size lstSize = new Size(300, 100);//14612, 5055);//(1000, 800);//(3980, 820); // Размер листа
        static double scale = 1; // Коэф-т масштабирования

        static int angleStep = 30; // Шаг поворотов фигур
        static int borderDistance = 0;
        static double[] scaleCoefs = { 1 };

        static string pathTmp = "tmp/";
        static string pathRes = "result/";

        static void Main(string[] args)
        {
            ;// test3;
        }
        
        static void oldMain(string[] args)
        {   
            //Environment.SetEnvironmentVariable("SWI_HOME_DIR", @"D:\\Program Files (x86)\\swipl");
            Environment.SetEnvironmentVariable("Path", @"D:\\Program Files (x86)\\swipl\\bin");
            
            CleanDir(pathTmp);
            CleanDir(pathRes);


            // Загрузка из PDF и масштабирование
            //InputHandling.ConvertPDFDirToScaledImg(pathSrc, pathTmp, scale);

            Log("Started. Scale: " + scale + " angleStep:" + angleStep + " lstSize: " + lstSize.Width + "x" + lstSize.Height);
            // Масштабирование
            Size scaledLstSize = new Size((int)(lstSize.Width * scale), (int)(lstSize.Height * scale));
            InputHandling.ScaleWholeDirectory(pathSrc, pathTmp, scale);

            // Загрузка фигур
            Console.WriteLine("Starting process. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            List<Figure> data = Figure.LoadFigures(pathTmp, srcFigColor, angleStep, scale, borderDistance);
            data.Sort(Figure.CompareFiguresBySize);
            Figure.UpdIndexes(data);
            //Figure.DeleteWrongAngles(scaledLstSize.Width, scaledLstSize.Height, data);
            Console.WriteLine("Figure loading finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            Log("Loaded Figs.");

            // Группировка фигур по листам
            //List<List<Figure>> preDefArr = new List<List<Figure>>();
            //preDefArr.Add(FormOneListArrangement(data, 0, 1, 2));
            //preDefArr.Add(FormOneListArrangement(data, 4, 5, 6, 8, 9, 13, 14));
            //SortFigures(preDefArr);


            // Поиск решения
            Console.WriteLine("Starting result finding. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            //var result = PrologSolutionFinder.GetAnyResult(scaledLstSize.Width, scaledLstSize.Height, scale, figInd);
            var preDefArr = SolutionChecker.FindAnAnswer(data, scaledLstSize.Width, scaledLstSize.Height, pathProlog, scaleCoefs);
            //List<ResultData> result = new List<ResultData>();
            //result.Add(res);
            var result = SolutionChecker.PlacePreDefinedArrangement(preDefArr, scaledLstSize.Width, scaledLstSize.Height, scale);
            if (result == null)
                Log("Prolog finished. No answer.");
            else
            {
                Log("Prolog finished. Answer was found.");
                // Отображение решения
                Console.WriteLine("Starting visualization. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                //OutputImage.SaveOneSingleListResult(data, result, scaledLstSize.Width, scaledLstSize.Height, pathRes);
                OutputImage.SaveResult(data, preDefArr, result, pathRes, scaledLstSize.Width, scaledLstSize.Height);
                //OutputText.SaveResult(preDefArr, result, pathRes + "result.txt");
            }


            Console.WriteLine("Process finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            Log("Finished.");
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
        
        static void Log(string msg)
        {
            string time = "[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "] ";
            File.AppendAllText(pathRes + "log.txt", time + msg + "\n");
        }

    }
}
