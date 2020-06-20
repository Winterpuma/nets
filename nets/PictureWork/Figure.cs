﻿using System;
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

        public Figure(string path, int id, Color figColor)
        {
            this.path = path;
            name = Path.GetFileName(path);
            name = name.Remove(name.IndexOf('.'));
            this.id = id;
            
            bitmap = new Bitmap(path);
            // или предварительно обработать в черно-белое
            //bitmap = HandleImages.MakeBlackAndWhite(new Bitmap(path), figColor);

            rotated.Add(new DeltaRepresentation(bitmap, figColor));
            int angleStep = 30;
            for (int angle = angleStep; angle < 360; angle += angleStep)
            {
                var tmpBmp = RotateImage(bitmap, angle);
                // сохранить повороты модели:
                //tmpBmp.Save("../../../../../src2_blackandwhite/" + name + "_" + angle.ToString() + ".png");
                rotated.Add(new DeltaRepresentation(tmpBmp, figColor));
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

        public static List<Figure> LoadFigures(string path, Color figColor)
        {
            string[] files = Directory.GetFiles(path);
            List<Figure> data = new List<Figure>();

            int id = 1;
            foreach (string f in files)
            {
                data.Add(new Figure(f, id, figColor));
                id++;
            }
            return data;
        }
    }
}
