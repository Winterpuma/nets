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
            File.Create(fileName);
            figInfoPath = fileName;
            _figInd = 0;
        }

        /// <summary>
        /// List<Figure> - список разных размеров фигуры
        /// </summary>
        public static void AddNewFig(Func<List<Figure>, int, string> predicate, List<Figure> figSizes)
        {
            string strToAppend = QueryCreator.CreateFigDifferentSizes(figSizes, _figInd);
            File.AppendAllText(figInfoPath, "\n");
            File.AppendAllText(figInfoPath, strToAppend);
            _figInd++;
        }
    }
}
