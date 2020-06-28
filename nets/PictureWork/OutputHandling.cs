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
        public static void SaveResult(List<Figure> data, List<ResultData> res, string path, int width, int height)
        {
            int i = 1;
            foreach (ResultData resultData in res)
            {
                Bitmap b = GetResultBitmap(data, resultData, width, height);
                b.Save(path + i + ".png");
                i++;
            }
        }

        // а если в ответе в другом порядке?
        public static Bitmap GetResultBitmap(List<Figure> data, ResultData res, int width, int height)
        {
            Bitmap b = new Bitmap(width, height);
            Random random = new Random();

            for (int i = 0; i < res.allFigures.Count; i++)
            {
                Color color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                ResultFigPos figPos = res.allFigures[i];
                Figure figData = data[i];
                PlaceDeltasOnABitmap(b, figData[0], figPos.xCenter, figPos.yCenter, color);
                // а повороты????
            }

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
