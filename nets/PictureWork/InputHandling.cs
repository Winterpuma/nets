using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace PictureWork
{
    static class InputHandling
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

        public static Image ResizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        public static Image ResizeImage(Image img, int scale)
        {
            return (Image)(new Bitmap(img, new Size(img.Width / scale, img.Height / scale)));
        }

        public static void ScaleWholeDirectory(string dirSrcPath, string dirDstPath, int scale)
        {
            string[] files = Directory.GetFiles(dirSrcPath);
            List<Figure> data = new List<Figure>();
            
            foreach (string f in files)
            {
                Image img = new Bitmap(f);
                Image yourImage = ResizeImage(img, scale);
                yourImage.Save(dirDstPath + Path.GetFileName(f));
            }
        }
    }
}
