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
        public static void PlaceDeltasOnABitmap(Bitmap bmp, List<Point> deltas, int centerX, int centerY, Color color)
        {
            foreach (Point p in deltas)
            {
                bmp.SetPixel(centerX + p.X, centerY + p.Y, color);
            }
        }

    }
}
