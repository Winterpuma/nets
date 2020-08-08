using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataClassLibrary;
using System.IO;

namespace PictureWork
{
    public static class FigureFileOperations
    {
        private static int _figInd = 0;
        static string figInfoPath = "figInfo.pl";


        public static void CreateNewFigFile(string fileName = "figInfo.pl")
        {
            using (StreamWriter file = new StreamWriter(fileName))
            {
                file.WriteLine();
            }
            figInfoPath = fileName;
            _figInd = 0;
        }

        /// <summary>
        /// Добавляет фигуру одного размера в файл
        /// </summary>
        private static void AddNewFig(Figure figure)
        {
            string strToAppend = QueryCreator.CreateFigOneSize(figure);
            using (StreamWriter file =
                new StreamWriter(figInfoPath, true))
            {
                file.WriteLine("%" + figure.name);
                file.WriteLine(strToAppend);
                file.WriteLine();
            }
            _figInd++;
        }

        /// <summary>
        /// Добавляет фигуру нескольких размеров в файл
        /// </summary>
        public static void AddFigAllSizes(Figure figure, params double[] scaleCoefs)
        {
            foreach (double coef in scaleCoefs)
            {
                AddNewFig(figure.GetScaledImage(coef));
            }
        }
        
        /// <summary>
        /// Добавляет несколько фигур нескольких размеров в файл
        /// </summary>
        public static void AddManyFigs(List<Figure> data, params double[] scaleCoefs)
        {
            foreach (Figure fig in data)
            {
                AddFigAllSizes(fig, scaleCoefs);
            }
        }

        /// <summary>
        /// Добавляет несколько фигур нужных углов в файл
        /// </summary>
        public static void AddManyFigs(List<Figure> data, List<int> angles)
        {
            for (int i = 0; i < data.Count; i++)
            {
                List<DeltaRepresentation> newAnglLst = new List<DeltaRepresentation>();
                newAnglLst.Add(data[i].rotated[angles[i]]);
                data[i].rotated = newAnglLst;
                AddNewFig(data[i]);
            }
        }
    }
}
