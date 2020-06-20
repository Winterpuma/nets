using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace PictureWork
{
    class Figure
    {
        private string path;
        public string name = "noname";
        Bitmap bitmap;
        public int id = -1;
        public List<DeltaRepresentation> rotated = new List<DeltaRepresentation>();

        public List<Point> this[int i]
        {
            get { return rotated[i].deltas; }
        }

        public Figure(string path, int id)
        {
            this.path = path;
            name = Path.GetFileName(path);
            this.id = id;
            bitmap = new Bitmap(path);

            rotated.Add(new DeltaRepresentation(bitmap));
            for (int angle = 1; angle < 360; angle++)
            {
                rotated.Add(new DeltaRepresentation(RotateImage(bitmap, angle)));
            }
        }

        public static Bitmap RotateImage(Image img, float rotationAngle)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2); // rotation point - center
            gfx.RotateTransform(rotationAngle);
            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
            
            gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
            
            gfx.DrawImage(img, new Point(0, 0));
            gfx.Dispose();
            
            return bmp;
        }

        public static List<Figure> LoadFigures(string path)
        {
            string[] files = Directory.GetFiles(path);
            List<Figure> data = new List<Figure>();

            int id = 1;
            foreach (string f in files)
            {
                data.Add(new Figure(f, id));
                id++;
            }
            return data;
        }
    }
}
