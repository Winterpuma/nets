using System;
using System.Collections.Generic;
using System.Drawing;
using DataClassLibrary;

namespace IO
{
    /// <summary>
    /// Класс функций представления результата в визуальной форме
    /// </summary>
    public static class OutputImage
    {
        /// <summary>
        /// Сохраняет найденное однолистное решение
        /// </summary>
        /// <param name="data">Список фигур</param>
        /// <param name="res">Решение</param>
        /// <param name="width">Ширина холста</param>
        /// <param name="height">Высота холста</param>
        /// <param name="path">Путь для сохранения файла</param>
        /// <returns></returns>
        public static Bitmap SaveOneSingleListResult(List<Figure> data, ResultData res, int width, int height, string path)
        {
            List<Color> color = GetNRandomColors(res.answer.Count);
            Bitmap b = GetResultBitmap(data, res, width, height, color);
            b.Save(path + "0.png");
            return b;
        }


        /// <summary>
        /// Сохраняет результат единственного расположения фигур на
        /// нескольких листах
        /// </summary>
        /// <param name="arrangement"></param>
        /// <param name="resultData"></param>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SaveResult(List<List<Figure>> arrangement, List<ResultData> resultData, string path, int width, int height)
        {
            for (int i = 0; i < resultData.Count; i++)
            {
                List<Color> color = GetNRandomColors(arrangement[i].Count);
                Bitmap b = GetResultBitmap(arrangement[i], resultData[i], width, height, color);
                b.Save(path + i + ".png");
            }
        }


        /// <summary>
        /// Сохраняет одну общую расстановку на множестве листов 
        /// </summary>
        /// <param name="data">Список фигур</param>
        /// <param name="arrangement">Расстановка фигур по листам</param>
        /// <param name="resultData">Список результатов для листов</param>
        /// <param name="path">Путь для сохранения результатов</param>
        /// <param name="width">Ширина листа</param>
        /// <param name="height">Высота листа</param>
        public static void SaveResult(List<Figure> data, List<List<int>> arrangement, List<ResultData> resultData, string path, int width, int height)
        {
            for (int i = 0; i < resultData.Count; i++)
            {
                List<Figure> curLst = new List<Figure>();
                foreach (int ind in arrangement[i])
                    curLst.Add(data[ind]);
                List<Color> color = GetNRandomColors(arrangement[i].Count);
                Bitmap b = GetResultBitmap(curLst, resultData[i], width, height, color);
                b.Save(path + i + ".png");
            }
        }


        /// <summary>
        /// Сохраняет все возможные результаты одного листа в графическом виде в указанную папку
        /// </summary>
        /// <param name="data">Данные о фигурах</param>
        /// <param name="res">Данные о решении (расположения фигур на листе)</param>
        /// <param name="path">Путь к папке для сохранения</param>
        /// <param name="width">Ширина выходной картинки (x)</param>
        /// <param name="height">Высота выходной картинки (y)</param>
        public static void SaveResult(List<Figure> data, List<ResultData> res, string path, int width, int height)
        {
            int i = 1;
            Random random = new Random();

            List<Color> color = GetNRandomColors(res[0].answer.Count);

            foreach (ResultData resultData in res)
            {
                Bitmap b = GetResultBitmap(data, resultData, width, height, color);
                b.Save(path + i + ".png");
                i++;
            }
        }
        

        /// <summary>
        /// Располагает фигуры в соответствии с найденным решением
        /// </summary>
        /// <param name="data">Список фигур</param>
        /// <param name="res">Найденное решение</param>
        /// <param name="width">Ширина холста</param>
        /// <param name="height">Высота холста</param>
        /// <param name="color">Цвета фигур</param>
        /// <returns>Изображение расположенных фигур</returns>
        private static Bitmap GetResultBitmap(List<Figure> data, ResultData res, int width, int height, List<Color> color)
        {
            Bitmap b = new Bitmap(width, height);
            
            for (int i = 0; i < res.answer.Count; i++)
            {
                ResultFigPos figPos = res.answer[i];
                Figure figData = data[i];
                DeltaRepresentation solutionFigWithoutScaling;

                if (figPos.angle == 0)
                    solutionFigWithoutScaling = figData.noScaling;
                else
                    solutionFigWithoutScaling = figData.noScaling.GetTurnedDelta((int)figPos.angle);

                
                PlaceDeltasOnABitmap(b, solutionFigWithoutScaling.deltas, figPos.xCenter, figPos.yCenter, color[i]);
            }
            return b;
        }


        /// <summary>
        /// Располагает массив точек на холсте
        /// </summary>
        /// <param name="bmp">Холст</param>
        /// <param name="deltas">Массив дельт</param>
        /// <param name="centerX">X-координата центра фигуры на холсте</param>
        /// <param name="centerY">Y-координата центра фигуры на холсте</param>
        /// <param name="color">Цвет фигуры на холсте</param>
        public static void PlaceDeltasOnABitmap(Bitmap bmp, List<Point> deltas, int centerX, int centerY, Color color)
        {
            foreach (Point p in deltas)
            {
                if (centerY + p.Y >= 0 && centerX + p.X >= 0)
                    bmp.SetPixel(centerX + p.X, centerY + p.Y, color);
            }
        }


        /// <summary>
        /// Генерирует список цветов размера n
        /// </summary>
        /// <param name="n">Количество цветов</param>
        /// <returns>Список цветов</returns>
        private static List<Color> GetNRandomColors(int n)
        {
            Random random = new Random();
            List<Color> color = new List<Color>();
            for (int j = 0; j < n; j++)
                color.Add(Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)));
            return color;
        }

    }
}
