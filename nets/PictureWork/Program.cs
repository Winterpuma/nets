﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Configuration;
using System.Globalization;
using DataClassLibrary;
using IO;

namespace PictureWork
{
    class Program
    {
        // Параметры
        static string pathSrc; // Путь к директории с фигурами
        static string pathPrologCode; // Путь к директории с кодом пролога

        static string pathTmp; // Путь для сохранения первично отмасштабированных фигур
        static string pathRes; // Путь для сохранения результата и лога

        static Color srcFigColor; // Цвет фигур на загружаемой картинке
        static int figAmount; // Кол-во фигур

        static Size lstSize; // Размер листа
        static double scale; // Коэф-т первоначального масштабирования

        static int angleStep; // Шаг поворотов фигур
        static int borderDistance; // Отступ от границы фигур (чтобы не слипались)
        static List<double> scaleCoefs = new List<double>();

        
        /// <summary>
        /// Инициализация параметров на основе файла конфигурации
        /// </summary>
        static void InitConfiguration()
        {
            pathSrc = ConfigurationManager.AppSettings.Get("pathSrc");
            pathPrologCode = ConfigurationManager.AppSettings.Get("pathPrologCode");

            pathTmp = ConfigurationManager.AppSettings.Get("pathTmp");
            pathRes = ConfigurationManager.AppSettings.Get("pathRes");

            int sizex = Convert.ToInt32(ConfigurationManager.AppSettings.Get("lstSizeX"));
            int sizey = Convert.ToInt32(ConfigurationManager.AppSettings.Get("lstSizeY"));
            lstSize = new Size(sizex, sizey);

            scale = Convert.ToDouble(ConfigurationManager.AppSettings.Get("scale"), CultureInfo.InvariantCulture);
            angleStep = Convert.ToInt32(ConfigurationManager.AppSettings.Get("angleStep"));
            borderDistance = Convert.ToInt32(ConfigurationManager.AppSettings.Get("borderDistance"));

            string hexCol = ConfigurationManager.AppSettings.Get("figColor");
            srcFigColor = ColorTranslator.FromHtml(hexCol);

            string scCoefs = ConfigurationManager.AppSettings.Get("scaleCoefs");
            foreach (string curCoef in scCoefs.Split(' '))
                scaleCoefs.Add(Convert.ToDouble(curCoef, CultureInfo.InvariantCulture));

            figAmount = Convert.ToInt32(ConfigurationManager.AppSettings.Get("figAmount"));
        }

        static void Main(string[] args)
        {
            InitConfiguration();

            CleanDir(pathTmp);
            CleanDir(pathRes);


            Log("Started. Scale: " + scale + " angleStep:" + angleStep + " lstSize: " + lstSize.Width + "x" + lstSize.Height);

            // Загрузка из PDF и масштабирование
            //InputHandling.ConvertPDFDirToScaledImg(pathSrc, pathTmp, scale);

            // Масштабирование
            Size scaledLstSize = new Size((int)(lstSize.Width * scale), (int)(lstSize.Height * scale));
            InputHandling.ScaleWholeDirectory(pathSrc, pathTmp, scale);


            // Загрузка фигур
            Console.WriteLine("Starting process. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            List<Figure> data = Figure.LoadFigures(pathTmp, srcFigColor, angleStep, borderDistance, figAmount);
            data.Sort(Figure.CompareFiguresBySize);
            Figure.UpdIndexes(data);
            //Figure.DeleteWrongAngles(scaledLstSize.Width, scaledLstSize.Height, data);
            SolutionChecker.LoadFigures(data, pathPrologCode, scaleCoefs);

            Console.WriteLine("Figure loading finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            Log("Loaded Figs.");


            // Поиск решения
            Console.WriteLine("Starting result finding. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            var preDefArr = SolutionChecker.FindAnAnswer(data, scaledLstSize.Width, scaledLstSize.Height, pathPrologCode, scaleCoefs);
            var result = SolutionChecker.PlacePreDefinedArrangement(preDefArr, scaledLstSize.Width, scaledLstSize.Height, scaleCoefs);
            if (result == null)
                Log("Prolog finished. No answer.");
            else
            {
                Log("Prolog finished. Answer was found.");
                // Отображение решения
                Console.WriteLine("Starting visualization. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                OutputImage.SaveResult(data, preDefArr, result, pathRes, scaledLstSize.Width, scaledLstSize.Height);
                OutputText.SaveResult(preDefArr, data, result, pathRes + "result.txt");
            }


            Console.WriteLine("Process finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            Log("Finished.");
            Console.ReadLine();
        }       
        
        /// <summary>
        /// Создание новой директории или удаление содержимого существующей
        /// </summary>
        public static void CleanDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return;
            }
                
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
        
        /// <summary>
        /// Примитивная функция логирования сообщения
        /// </summary>
        static void Log(string msg)
        {
            string time = "[" + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "] ";
            File.AppendAllText(pathRes + "log.txt", time + msg + "\n");
        }

    }
}
