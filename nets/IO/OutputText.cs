using System;
using System.Collections.Generic;
using DataClassLibrary;
using System.IO;

namespace IO
{
    /// <summary>
    /// Класс функций представления результата в текстовой форме
    /// </summary>
    public static class OutputText
    {

        /*
        [Лист%номер%
            [%имя фигуры%  %х% %у% %угол поворота%]
        ]
        */
        

        /// <summary>
        /// Генерирует текстовую строку для одной фигуры
        /// </summary>
        private static string GetOneFigureResult(Figure curFig, ResultFigPos res)
        {
            return '[' + curFig.name + ' ' + res.xCenter + ' ' + res.yCenter + ' ' + res.angle + ']';
        }


        /// <summary>
        /// Генерирует текстовую строку для одного листа
        /// </summary>
        /// <param name="arrangement">Индексы выбранных в этот лист фигур</param>
        /// <param name="data">Список всех фигур</param>
        /// <param name="res">Размещение</param>
        /// <param name="listN">Индекс текущего листа</param>
        private static string GetOneSIngleListResult(List<int> arrangement,  List<Figure> data, ResultData res, int listN = 0)
        {
            List<string> allFigs = new List<string>();
            for (int i = 0; i < arrangement.Count; i++)
            {
                allFigs.Add(GetOneFigureResult(data[arrangement[i]], res.answer[i]));
            }

            return "[Лист" + listN + "\n\t" + String.Join("\n\t", allFigs) + "\n]";
        }


        /// <summary>
        /// Генерирует текстовую строку для нескольких листов решения
        /// </summary>
        /// <param name="arrangement">Список размещений фигур на листах</param>
        /// <param name="data">Список всех фигур</param>
        /// <param name="res">Список размещений на листах</param>
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
        public static void SaveResult(List<List<int>> arrangement, List<Figure> data, List<ResultData> resultData, string path)
        {
            string formedText = GetMultipleListsRes(arrangement, data, resultData);
            File.WriteAllText(path, formedText);
        }

    }
}
