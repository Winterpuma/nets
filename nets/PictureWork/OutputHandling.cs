using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PictureWork
{
    static class OutputHandling
    {
        /// <summary>
        /// Сохраняет результаты в графическом виде в указанную папку
        /// </summary>
        /// <param name="data">Данные о фигурах</param>
        /// <param name="res">Данные о решении (расположение фигур)</param>
        /// <param name="path">Путь к папке для сохранения</param>
        /// <param name="width">Ширина выходной картинки (x)</param>
        /// <param name="height">Высота выходной картинки (y)</param>
        public static void SaveResult(List<Figure> data, List<ResultData> res, string path, int width, int height)
        {
            int i = 1;
            Random random = new Random();

            List<Color> color = new List<Color>();
            for (int j = 0; j < res[0].allFigures.Count; j++)
                color.Add(Color.FromArgb(random.Next(255), random.Next(255), random.Next(255)));

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

    }
}
