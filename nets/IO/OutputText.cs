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

        private static string GetOneFigureResult(Figure curFig, ResultFigPos res)
        {
            return '[' + curFig.name + ' ' + res.xCenter + ' ' + res.yCenter + ' ' + res.angle + ']';
        }


        private static string GetOneSIngleListResult(List<int> arrangement,  List<Figure> data, ResultData res, int listN = 0)
        {
            List<string> allFigs = new List<string>();
            for (int i = 0; i < arrangement.Count; i++)
            {
                allFigs.Add(GetOneFigureResult(data[arrangement[i]], res.answer[i]));
            }

            return "[Лист" + listN + "\n\t" + String.Join("\n\t", allFigs) + "\n]";
        }

        private static string GetMultipleListsRes(List<List<int>> arrangement, List<Figure> data, List<ResultData> res)
        {
            List<string> allLists = new List<string>();
            for (int i = 0; i < res.Count; i++)
                allLists.Add(GetOneSIngleListResult(arrangement[i], data, res[i], i));

            return String.Join("\n", allLists);
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
        public static void SaveResult(List<List<int>> arrangement, List<Figure> data, List<ResultData> resultData, string path)
        {
            string formedText = GetMultipleListsRes(arrangement, data, resultData);
            File.WriteAllText(path, formedText);
        }

    }
}
