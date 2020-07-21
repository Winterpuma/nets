using System;
using System.Collections.Generic;
using DataClassLibrary;
using System.IO;

namespace IO
{
    public static class OutputText
    {

        /*
        [Лист%номер%
            [%имя фигуры%  %х% %у% %угол поворота%]
        ]
        */

        private static string GetOneFigureResult(List<Figure> data, ResultFigPos res)
        {
            Figure curFig = data[Convert.ToInt32(res.name)]; // или -1?
            return '[' + curFig.name + ' ' + res.xCenter + ' ' + res.yCenter + ' ' + res.angle + ']';
        }

        private static string GetOneSIngleListResult(List<Figure> data, ResultData res, int listN = 0)
        {
            List<string> allFigs = new List<string>();
            foreach (ResultFigPos rfp in res.allFigures)
                allFigs.Add(GetOneFigureResult(data, rfp));

            return "[Лист" + listN + "\n\t" + String.Join("\n\t", allFigs) + "\n]";
        }

        private static string GetMultipleListsRes(List<List<Figure>> data, List<ResultData> res)
        {
            List<string> allLists = new List<string>();
            for (int i = 0; i < res.Count; i++)
                allLists.Add(GetOneSIngleListResult(data[i], res[i], i));

            return String.Join("\n", allLists);
        }


        /// <summary>
        /// Сохраняет один результат одного листа
        /// </summary>
        /// <param name="data"></param>
        /// <param name="res"></param>
        /// <param name="path"></param>
        public static void SaveOneSingleListResult(List<Figure> data, ResultData res, string path)
        {
            string formedText = GetOneSIngleListResult(data, res);
            File.WriteAllText(path, formedText);
        }
        
        /// <summary>
        /// Сохраняет результат единственного расположения фигур на
        /// нескольких листах
        /// </summary>
        /// <param name="arrangement"></param>
        /// <param name="resultData"></param>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SaveResult(List<List<Figure>> arrangement, List<ResultData> resultData, string path)
        {
            string formedText = GetMultipleListsRes(arrangement, resultData);
            File.WriteAllText(path, formedText);
        }

    }
}
