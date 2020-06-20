using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PictureWork
{
    class HandleImages
    {
        public static Bitmap MakeBlackAndWhite(Bitmap bmp, Color figColor)
        {
            for (int xCur = 0; xCur < bmp.Width; xCur++)
            {
                for (int yCur = 0; yCur < bmp.Height; yCur++)
                {
                    Color curColor = bmp.GetPixel(xCur, yCur);
                    if (curColor.R == figColor.R && curColor.G == figColor.G && curColor.B == figColor.B)
                    {
                        bmp.SetPixel(xCur, yCur, Color.Black);
                    }
                    else
                    {
                        bmp.SetPixel(xCur, yCur, Color.White);
                    }
                }
            }
            return bmp;
        }

    }
}
