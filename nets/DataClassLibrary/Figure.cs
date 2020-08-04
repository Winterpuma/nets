using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace DataClassLibrary
{
    public class Figure
    {
        private string path;
        public string name = "noname";
        public int id = -1;
        public double scaleCoef = 1;
        Bitmap bitmap;
        int angleStep;
        int borderDistance;
        Color figColor;

        public DeltaRepresentation noScaling;
        public List<DeltaRepresentation> rotated = new List<DeltaRepresentation>();

        public List<Point> this[int i]
        {
            get { return rotated[i].deltas; }
        }

        public Figure(string path, int id, Color figColor, int angleStep = 1, int borderDistance = 0)
        {
            this.path = path;
            name = Path.GetFileName(path);
            name = name.Remove(name.IndexOf('.'));
            this.id = id;
            this.angleStep = angleStep;
            this.figColor = figColor;
            this.borderDistance = borderDistance;
            
            bitmap = new Bitmap(path);
            LoadFigureFromItsBitmap(borderDistance);
        }

        private Figure(Figure parentFig, Bitmap editedFig, double scaleCoef = 1)
        {
            path = parentFig.path;
            name = parentFig.name;
            id = parentFig.id;
            this.scaleCoef = parentFig.scaleCoef * scaleCoef;
            angleStep = parentFig.angleStep;
            figColor = parentFig.figColor;
            borderDistance = parentFig.borderDistance;

            bitmap = editedFig;
            LoadFigureFromItsBitmap(borderDistance);
        }

        private void LoadFigureFromItsBitmap(int borderDistance)
        {

            noScaling = new DeltaRepresentation(bitmap, figColor);
            if (noScaling.deltas.Count == 0)
                throw new Exception("Empty figure, maybe different color?"); ;

            DeltaRepresentation originalDeltas;
            if (borderDistance == 0)
                originalDeltas = noScaling;
            else
                originalDeltas = new DeltaRepresentation(bitmap, figColor, borderDistance);

            rotated.Add(originalDeltas);

            for (int angle = angleStep; angle < 360; angle += angleStep)
            {
                rotated.Add(originalDeltas.GetTurnedDelta(angle, 0, 0));
            }
        }
        
        
        public static List<Figure> LoadFigures(string path, Color figColor, int angleStep = 1, double scale = 1)
        {
            string[] files = Directory.GetFiles(path);
            List<Figure> data = new List<Figure>();

            int id = 0;
            foreach (string f in files)
            {
                Figure fig = new Figure(f, id, figColor, angleStep);
                fig.scaleCoef = scale;
                data.Add(fig);
                id++;
            }
            return data;
        }

        public void ChangeBorderDistance(int borderDistance)
        {
            rotated.Clear();

            DeltaRepresentation originalDeltas = new DeltaRepresentation(bitmap, figColor, borderDistance);

            rotated.Add(originalDeltas);
            Console.WriteLine("Loaded original delta. Delta len " + originalDeltas.deltas.Count);

            for (int angle = angleStep; angle < 360; angle += angleStep)
            {
                Console.Write(" " + angle);
                rotated.Add(originalDeltas.GetTurnedDelta(angle, 0, 0));
            }
        }

        public Figure GetScaledImage(double scaleCoef)
        {
            if (scaleCoef == 1)
                return this;
            Size scaledSize = new Size((int)(bitmap.Width / scaleCoef), (int)(bitmap.Height / scaleCoef));
            Bitmap scaledBitmap = new Bitmap(bitmap, scaledSize);
            return new Figure(this, scaledBitmap, scaleCoef);
        }

        public static int CompareFiguresBySize(Figure x, Figure y)
        {
            return y[0].Count - x[0].Count;
        }
    }
}
