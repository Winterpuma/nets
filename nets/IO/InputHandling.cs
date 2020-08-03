using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Apitron.PDF.Rasterizer;
using Apitron.PDF.Rasterizer.Configuration;
using DataClassLibrary;

namespace IO
{
    public static class InputHandling
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

        public static void ScaleWholeDirectory(string dirSrcPath, string dirDstPath, Size newSize)
        {
            string[] files = Directory.GetFiles(dirSrcPath);
            List<Figure> data = new List<Figure>();

            foreach (string f in files)
            {
                Image img = new Bitmap(f);
                Image yourImage = ResizeImage(img, newSize);
                yourImage.Save(dirDstPath + Path.GetFileName(f));
            }
        }

        public static void ConvertPDFDirToScaledImg(string dirSrcPath, string dirDstPath, int scale)
        {
            string[] files = Directory.GetFiles(dirSrcPath);

            foreach (string f in files)
            {
                string filename = Path.GetFileNameWithoutExtension(f).ToLower();
                Document document = new Document(File.Open(f, FileMode.Open));
                RenderingSettings settings = new RenderingSettings();
                
                for (int i = 0; i < document.Pages.Count; i++)
                {
                    Page currentPage = document.Pages[i];
                    
                    using (Bitmap bitmap = currentPage.Render((int)currentPage.Width, (int)currentPage.Height, settings))
                    {
                        string imgName = dirDstPath + filename + i + ".png";
                        Image yourImage = ResizeImage(bitmap, scale);
                        yourImage.Save(imgName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }                
            }
        }

        public static void ConvertPDFDirToImg(string dirSrcPath, string dirDstPath)
        {
            string[] files = Directory.GetFiles(dirSrcPath);

            foreach (string f in files)
            {
                string filename = Path.GetFileNameWithoutExtension(f).ToLower();
                Document document = new Document(File.Open(f, FileMode.Open));
                RenderingSettings settings = new RenderingSettings();

                for (int i = 0; i < document.Pages.Count; i++)
                {
                    Page currentPage = document.Pages[i];

                    using (Bitmap bitmap = currentPage.Render((int)currentPage.Width, (int)currentPage.Height, settings))
                    {
                        string imgName = dirDstPath + filename + i + ".png";
                        bitmap.Save(imgName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
        }
    }
}
