using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PictureWork
{
    class DeltaRepresentation
    {
        Bitmap bmp;
        int xCenter, yCenter;
        List<Point> deltas = new List<Point>();

        public DeltaRepresentation(Bitmap bmp)
        {
            this.bmp = bmp;

            xCenter = bmp.Width / 2;
            yCenter = bmp.Height / 2;

            for (int xCur = 0; xCur < bmp.Width; xCur++)
            {
                for (int yCur = 0; yCur < bmp.Height; yCur++)
                {
                    Color curColor = bmp.GetPixel(xCur, yCur);
                    if (curColor.R == 0 && curColor.G == 0 && curColor.B == 0)
                    {
                        int xDelta = xCenter - xCur;
                        int yDelta = yCenter - yCur;

                        deltas.Add(new Point(xDelta, yDelta));
                    }
                }
            }
        }

    }
}
