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

        public Figure(string path, int id, Color figColor, int angleStep = 1)
        {
            Console.WriteLine("\nLoad Figure " + id);
            this.path = path;
            name = Path.GetFileName(path);
            name = name.Remove(name.IndexOf('.'));
            this.id = id;
            
            bitmap = new Bitmap(path);

            // Если нужна предварительная обработка в черно-белое:
            // bitmap = HandleImages.MakeBlackAndWhite(new Bitmap(path), figColor);

            var originalDeltas = new DeltaRepresentation(bitmap, figColor);
            rotated.Add(originalDeltas);
            Console.WriteLine("Loaded original delta. Delta len " + originalDeltas.deltas.Count);

            for (int angle = angleStep; angle < 360; angle += angleStep)
            {
                Console.Write(" " + angle);
                rotated.Add(originalDeltas.GetTurnedDelta(angle, 0, 0));

                // более медленный вариант:
                //var tmpBmp = RotateImage(bitmap, angle);                
                //rotated.Add(new DeltaRepresentation(tmpBmp, figColor));
            }
        }
        
        
        public static List<Figure> LoadFigures(string path, Color figColor, int angleStep = 1)
        {
            string[] files = Directory.GetFiles(path);
            List<Figure> data = new List<Figure>();

            int id = 1;
            foreach (string f in files)
            {
                data.Add(new Figure(f, id, figColor, angleStep));
                id++;
            }
            return data;
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
    }
}
