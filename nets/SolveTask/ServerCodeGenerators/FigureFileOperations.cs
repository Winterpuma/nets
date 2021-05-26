using System.Collections.Generic;
using DataClassLibrary;
using System.IO;

namespace SolveTask.ServerCodeGenerators
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
        public static void AddFigAllSizes(Figure figure, List<double> scaleCoefs)
        {
            foreach (double coef in scaleCoefs)
            {
                var f = figure.GetScaledImage(coef);
                //f.DeleteWrongAngles(100, 100);
                AddNewFig(f);
            }
        }
        
        /// <summary>
        /// Добавляет несколько фигур нескольких размеров в файл
        /// </summary>
        public static void AddManyFigs(List<Figure> data, List<double> scaleCoefs)
        {
            foreach (Figure fig in data)
            {
                AddFigAllSizes(fig, scaleCoefs);
            }
        }
        
    }
}
