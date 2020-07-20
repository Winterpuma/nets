using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PictureWork
{
    public static class OutputHandling
    {
        public static Bitmap SaveOneSingleListResult(List<Figure> data, ResultData res, int width, int height, string path)
        {
            List<Color> color = GetNRandomColors(res.allFigures.Count);
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

            List<Color> color = GetNRandomColors(res[0].allFigures.Count);

            foreach (ResultData resultData in res)
            {
                Bitmap b = GetResultBitmap(data, resultData, width, height, color);
                b.Save(path + i + ".png");
                i++;
            }
        }

        // а если в ответе в другом порядке?
        public static Bitmap GetResultBitmap(List<Figure> data, ResultData res, int width, int height, List<Color> color)
        {
            Bitmap b = new Bitmap(width, height);
            
            for (int i = 0; i < res.allFigures.Count; i++)
            {
                ResultFigPos figPos = res.allFigures[i];
                Figure figData = data[i];
                PlaceDeltasOnABitmap(b, figData[(int)figPos.angle], figPos.xCenter, figPos.yCenter, color[i]);
            }
            b.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            return b;
        }


        public static void PlaceDeltasOnABitmap(Bitmap bmp, List<Point> deltas, int centerX, int centerY, Color color)
        {
            foreach (Point p in deltas)
            {
                bmp.SetPixel(centerX + p.X, centerY + p.Y, color);
            }
        }

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
