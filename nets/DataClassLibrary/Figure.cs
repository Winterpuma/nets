﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace DataClassLibrary
{
    public class Figure
    {
        public string name = "noname";  // имя фигуры
        public int id = -1;             // ee id. должен быть уникален в группе фигур
        public double scaleCoef = 1;    // коэффициент масштабирования
        public int amount = 1;          // количество фигур такого типа для размещения

        private string path;            // расположение исходного файла фигуры (img/pdf)
        Bitmap bitmap;                  // неизмененное изображение фигуры
        public int angleStep;                  // шаг прироста углов при генерации поворотов
        public int borderDistance;      // увеличение площади фигуры на это кол-во пикселей
        Color figColor;                 // цвет фигуры

        public DeltaRepresentation noScaling;           // Дельта представление загруженной картинки
        public DeltaRepresentation withBorderDistance;  // Дельта представление загруженной картинки с отступами по краям


        /// <summary>
        /// Получение списка точек нужного поворота
        /// </summary>
        /// <param name="i">Угол поворота</param>
        /// <returns>Массив точек</returns>
        public List<Point> this[int i]
        {
            get { return withBorderDistance.GetTurnedDelta(i).deltas; }
        }


        /// <summary>
        /// Создание фигуры из картинки на диске
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="id">id фигуры</param>
        /// <param name="figColor">Цвет фигуры</param>
        /// <param name="angleStep">Шаг прироста углов при генерации поворотов</param>
        /// <param name="borderDistance">Количество пикселей прироста в ширине фигуры</param>
        /// <param name="figAmount">Количество фигур данного типажа</param>
        public Figure(string path, int id, Color figColor, int angleStep = 1, int borderDistance = 0, int figAmount = 1)
        {
            this.path = path;
            name = Path.GetFileName(path);
            name = name.Remove(name.IndexOf('.'));
            this.id = id;
            this.angleStep = angleStep;
            this.figColor = figColor;
            this.borderDistance = borderDistance;
            amount = figAmount;

            bitmap = new Bitmap(path);
            LoadFigureFromItsBitmap(borderDistance);
        }


        /// <summary>
        /// Создание отмасштабированной версии фигуры
        /// </summary>
        /// <param name="parentFig">Исходная фигура</param>
        /// <param name="editedFig">Отмасштабированное изображение фигуры</param>
        /// <param name="scaleCoef">Коэффициент масштабирования</param>
        private Figure(Figure parentFig, Bitmap editedFig, double scaleCoef = 1)
        {
            path = parentFig.path;
            name = parentFig.name;
            id = parentFig.id;
            this.scaleCoef = scaleCoef;//parentFig.scaleCoef * scaleCoef;
            angleStep = parentFig.angleStep;
            figColor = parentFig.figColor;
            borderDistance = (int)Math.Floor(parentFig.borderDistance * scaleCoef); //? ok?

            bitmap = editedFig;
            LoadFigureFromItsBitmap(borderDistance);
        }


        /// <summary>
        /// Загрузка дельт из картинки
        /// </summary>
        /// <param name="borderDistance">Количество пикселей прироста в ширине фигуры</param>
        private void LoadFigureFromItsBitmap(int borderDistance)
        {
            noScaling = new DeltaRepresentation(bitmap, figColor);
            if (noScaling.deltas.Count == 0)
                throw new Exception("Empty figure, maybe different color?"); ;
            
            if (borderDistance == 0)
                withBorderDistance = noScaling;
            else
                withBorderDistance = new DeltaRepresentation(bitmap, figColor, borderDistance);
        }
        

        /// <summary>
        /// Загружает все фигуры из одной директории
        /// </summary>
        /// <param name="path">Путь к директории</param>
        /// <param name="figColor">Цвет фигур</param>
        /// <param name="angleStep">Шаг прироста углов</param>
        /// <param name="borderDistance">Дополнительный прирост пикселей фигуры в ширину</param>
        /// <param name="figAmount">Количество повторов каждой из фигур</param>
        /// <returns>Список загруженных фигур</returns>
        public static List<Figure> LoadFigures(string path, Color figColor, int angleStep = 1, int borderDistance = 0, int figAmount = 1)
        {
            string[] files = Directory.GetFiles(path);
            List<Figure> data = new List<Figure>();

            int id = 0;
            foreach (string f in files)
            {
                Figure fig = new Figure(f, id, figColor, angleStep, borderDistance, figAmount);
                data.Add(fig);
                id++;
            }
            return data;
        }


        /// <summary>
        /// Перезагружает withBorderDistance для нового количества дополнительных пикселей
        /// </summary>
        /// <param name="borderDistance">Количество доп пикселей с каждой из сторон</param>
        public void ChangeBorderDistance(int borderDistance)
        {
            withBorderDistance = new DeltaRepresentation(bitmap, figColor, borderDistance);
        }


        /// <summary>
        /// Возвращает фигуру нужного размера
        /// </summary>
        /// <param name="scaleCoef">Коэффициент масштабирования</param>
        /// <returns>Возвращает отмасшатбированную фигуру или саму себя</returns>
        public Figure GetScaledImage(double scaleCoef)
        {
            if (scaleCoef == 1)
                return this;
            Size scaledSize = new Size((int)(bitmap.Width * scaleCoef), (int)(bitmap.Height * scaleCoef));
            Bitmap scaledBitmap = new Bitmap(bitmap, scaledSize);
            return new Figure(this, scaledBitmap, scaleCoef);
        }


        /// <summary>
        /// Функция сравнения двух фигур по количеству пикселей в
        /// неповернутом представлении
        /// </summary>
        /// <returns>Положительное, если figB больше</returns>
        public static int CompareFiguresBySize(Figure figA, Figure figB)
        {
            return figB[0].Count - figA[0].Count;
        }


        /// <summary>
        /// Обновляет индексы фигур в порядке следования в списке
        /// </summary>
        /// <param name="data">Список фигур</param>
        public static void UpdIndexes(List<Figure> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                data[i].id = i;
            }
        }
    }
}
