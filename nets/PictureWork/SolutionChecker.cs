using System;
using System.Collections.Generic;
using DataClassLibrary;

namespace PictureWork
{
    public class SolutionChecker
    {
        #region Проверки 
        /// <summary>
        /// Считает сумму дельт
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int CountDeltas(List<Figure> data)
        {
            int sum = 0;
            foreach(Figure f in data)
            {
                sum += f[0].Count;
            }
            return sum;
        }

        /// <summary>
        /// Проверяет помещается ли текущее размещение деталей на одном листе
        /// </summary>
        private static bool DoesCurrentListFit(List<Figure> list, int w, int h)
        {
            int area = w * h;
            if (CountDeltas(list) > area || !PrologSolutionFinder.DoesFiguresFit(list, w, h))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Проверяет помещается ли текущее размещение деталей по площади дельт и 
        /// перебор решений пролога
        /// </summary>
        private static bool DoesCurrentArrangementFit(List<List<Figure>> sequence, int w, int h)
        {
            int area = w * h;
            foreach (List<Figure> curLstArrangement in sequence)
            {
                if (CountDeltas(curLstArrangement) > area || !PrologSolutionFinder.DoesFiguresFit(curLstArrangement, w, h))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяет нет ли пустых листов внутри расстановки
        /// </summary>
        private static bool ArrangementIsBad(List<List<Figure>> arrangement)
        {
            foreach (List<Figure> i in arrangement)
            {
                if (i.Count == 0)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Проверяет помещаются ли фигуры на M листах
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="MLists">Количество листов</param>
        /// <returns></returns>
        private static bool DoesFiguresFitOnMLists(List<Figure> data, int width, int height, int MLists)
        {
            if (GetWorkingArrangement(data, width, height, MLists) != null)
                return true;
            return false;
        }
        #endregion

        private static void Dbg1(List<List<Figure>> curSequence)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-----------");
            curSequence.ForEach((item) =>
            {
                item.ForEach((fig) =>
                {
                    Console.Write(fig.name + " ");
                });
                Console.WriteLine();
            });
            Console.WriteLine();

            Console.ResetColor();
        }

        private static int GetHalf(int min, int max)
        {
            return min + (int)Math.Floor((double)((max - min) / 2));
        }

        #region Поиск решения на конкретном кол-ве листов
        /// <summary>
        /// Получает единственное подходящее решение или null
        /// </summary>
        public static List<List<Figure>> GetWorkingArrangement(List<Figure> data, int w, int h, int nEmpty,
            List<List<Figure>> result = null)
        {
            if (result == null)
            {
                result = new List<List<Figure>>();
                result.Add(new List<Figure>());
                result[0].Add(data[0]); // первую фигуру всегда в новый лист
                data.RemoveAt(0);
                nEmpty--;
            }
            if (data.Count == 0)
            {
                Dbg1(result);
                if (nEmpty == 0)
                    return result;
                else
                    return null;
            }

            Figure currentFig = data[0];
            var nextData = new List<Figure>(data);
            nextData.RemoveAt(0);

            // Если остались свободные листы, то пытаемся в новый
            if (nEmpty > 0)
            {
                var tmp = new List<Figure>();
                tmp.Add(currentFig);
                var newResult = new List<List<Figure>>(result);
                newResult.Add(tmp);
                var curRes = GetWorkingArrangement(nextData, w, h, nEmpty - 1, newResult);
                if (curRes != null)
                    return curRes;
            }

            // Если не получилось добавление в новый, то
            // пытаемся последовательно добавлять в уже существующие листы
            for (int i = 0; i < result.Count; i++)
            {
                var newCurLst = new List<Figure>(result[i]);
                newCurLst.Add(currentFig);
                if (DoesCurrentListFit(newCurLst, w, h))
                {
                    result[i].Add(currentFig);
                    var curRes = GetWorkingArrangement(nextData, w, h, nEmpty, result);
                    if (curRes != null)
                        return result;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Поиск расположения на минимальном кол-ве листов половинным делением
        /// </summary>
        public static List<List<Figure>> FindMinArrangementHalfDiv(List<Figure> data, int width, int height)
        {
            int maxLstNumber = data.Count;
            int minLstNumber = 1;
            List<List<Figure>> res = new List<List<Figure>>();

            do
            {
                int curLstNumber = GetHalf(minLstNumber, maxLstNumber);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("------ min: " + minLstNumber + " max: " + maxLstNumber + " cur: " + curLstNumber + "  " +
                    DateTime.Now.Minute + ":" + DateTime.Now.Second);
                Console.ResetColor();
                
                var tmp = GetWorkingArrangement(new List<Figure>(data), width, height, curLstNumber);
                if (tmp != null) // было найдено решение для текущего кол-ва листов
                {
                    maxLstNumber = curLstNumber;
                    res = tmp;
                    Console.WriteLine("Yes");
                }
                else
                {
                    minLstNumber = curLstNumber + 1;
                    Console.WriteLine(curLstNumber + ": No");
                }
            }
            while (maxLstNumber != minLstNumber);

            return res;
        }

        /// <summary>
        /// Поиск расположения на минимальном кол-ве листов
        /// Попытки найти решение на убывающем кол-ве листов.
        /// </summary>
        public static List<List<Figure>> FindMinArrangementDec(List<Figure> data, int width, int height)
        {
            int curLstNumber = data.Count;
            List<List<Figure>> res = new List<List<Figure>>();

            while (curLstNumber > 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("------ cur: " + curLstNumber + "  " +
                    DateTime.Now.Minute + ":" + DateTime.Now.Second);
                Console.ResetColor();

                var tmp = GetWorkingArrangement(new List<Figure>(data), width, height, curLstNumber);
                if (tmp != null) // было найдено решение для текущего кол-ва листов
                {
                    res = tmp;
                    curLstNumber--;
                }
                else
                    return res;
            }

            return res;
        }
        #endregion

        /// <summary>
        /// Получает единственное подходящее решение или null 
        /// без привязки к количеству листов
        /// </summary>
        public static List<List<Figure>> GetWorkingArrangement(List<Figure> data, int w, int h,
            List<List<Figure>> result = null)
        {
            if (result == null)
            {
                result = new List<List<Figure>>();
                result.Add(new List<Figure>());
                result[0].Add(data[0]); // первую фигуру всегда в новый лист
                data.RemoveAt(0);
            }
            if (data.Count == 0)
            {
                Dbg1(result);
                return result;
            }

            Figure currentFig = data[0];
            var nextData = new List<Figure>(data);
            nextData.RemoveAt(0);

            
            // Пытаемся последовательно добавлять в уже существующие листы
            for (int i = 0; i < result.Count; i++)
            {
                var newCurLst = new List<Figure>(result[i]);
                newCurLst.Add(currentFig);
                if (DoesCurrentListFit(newCurLst, w, h))
                {
                    result[i].Add(currentFig);
                    var curResult = GetWorkingArrangement(nextData, w, h, result);
                    if (curResult != null)
                        return result;
                }
            }

            // Если не получилось добавить к существующим, кладем в новый
            var tmp = new List<Figure>();
            tmp.Add(currentFig);
            var newResult = new List<List<Figure>>(result);
            newResult.Add(tmp);
            var curRes = GetWorkingArrangement(nextData, w, h, newResult);
            if (curRes != null)
                return curRes;

            return null;
        }

        /// <summary>
        /// Известно разделение фигур по листам, ищется расположение
        /// </summary>
        public static List<ResultData> PlacePreDefinedArrangement(List<List<Figure>> arrangement, int w, int h)
        {
            List<ResultData> results = new List<ResultData>();
            foreach (List<Figure> curLst in arrangement)
            {
                var res = PrologSolutionFinder.GetAnyResult(curLst, w, h);
                if (res == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: can't fit figures with given arrangement");
                    Console.ResetColor();
                    return null;
                }
                else
                {
                    results.Add(res);
                }
            }
            return results;
        }
    }
}
