using System;
using System.Collections.Generic;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using System.IO;

namespace PictureWork
{
    class SolutionChecker
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

        public static List<List<Figure>> GetOnePosibleArrangement(List<Figure> data, int w, int h, int MLists, List<List<Figure>> curSequence = null)
        {
            if (curSequence == null)
            {
                curSequence = new List<List<Figure>>();
                for (int i = 0; i < MLists; i++)
                    curSequence.Add(new List<Figure>());
            }
            if (data.Count == 0)
            {
                Dbg1(curSequence);
                
                if (DoesCurrentArrangementFit(curSequence, w, h)) // Проверка подходит ли текущий результат
                    return curSequence;
                else
                    return null;
            }

            for (int i = 0; i < MLists; i++)
            {
                List<List<Figure>> newSequence = new List<List<Figure>>();

                curSequence.ForEach((item) =>
                {
                    newSequence.Add(new List<Figure>(item));
                });

                var newData = new List<Figure>(data);
                newData.RemoveAt(0);
                newSequence[i].Add(data[0]);

                List<List<Figure>> res = GetOnePosibleArrangement(newData, w, h, MLists, newSequence);
                if (res != null)
                    return res;
            }
            return null;
        }


        public static List<List<List<Figure>>> AllPosibleArrangements(List<Figure> data, int w, int h, int MLists, 
            List<List<Figure>> curSequence = null, List<List<List<Figure>>> res = null)
        {
            if (res == null)
                res = new List<List<List<Figure>>>();
            if (curSequence == null)
            {
                curSequence = new List<List<Figure>>();
                for (int i = 0; i < MLists; i++)
                    curSequence.Add(new List<Figure>());
            }
            if (data.Count == 0)
            {
                //Dbg1(curSequence);
                if (true)//!ArrangementIsBad(curSequence))
                    res.Add(curSequence);
                return res;
            }

            for (int i = 0; i < MLists; i++)
            {
                List<List<Figure>> newSequence = new List<List<Figure>>();

                curSequence.ForEach((item) =>
                {
                    newSequence.Add(new List<Figure>(item));
                });
                

                var newData = new List<Figure>(data);
                newData.RemoveAt(0);
                newSequence[i].Add(data[0]);

                AllPosibleArrangements(newData, w, h, MLists, newSequence, res);
            }
            return res;
        }

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
            foreach (List<Figure> curLst in result)
            {
                var newCurLst = new List<Figure>(curLst);
                newCurLst.Add(currentFig);
                if (DoesCurrentListFit(newCurLst, w, h))
                {
                    curLst.Add(currentFig); // а точно ли меняется curLst(result)??
                    var curRes = GetWorkingArrangement(nextData, w, h, nEmpty, result);
                    if (curRes != null)
                        return result;
                }
            }
            
            return null;
        }



        /// <summary>
        /// Убирает перестановки листов местами и расстановки,
        /// в которых есть пустые листы.
        /// Изменяет arrangements!
        /// </summary>
        public static List<List<List<Figure>>> CleanupDuplicates(List<List<List<Figure>>> arrangements)
        {
            int mLst = arrangements[0].Count;
            int fact = 1;
            while (mLst != 1)
            {
                fact *= mLst;
                mLst--;
            }
            
            arrangements.RemoveAll(ArrangementIsBad);
            
            int newLen = arrangements.Count / fact;
            arrangements.RemoveRange(newLen, arrangements.Count - newLen);

            return arrangements;
        }

        private static bool DoesFiguresFitOnMLists(List<Figure> data, int width, int height, int MLists)
        {
            if (GetOnePosibleArrangement(data, width, height, MLists) != null)
                return true;
            return false;
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
                int curLstNumber = 10;// GetHalf(minLstNumber, maxLstNumber);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("------ min: " + minLstNumber + " max: " + maxLstNumber + " cur: " + curLstNumber + "  " +
                    DateTime.Now.Minute + ":" + DateTime.Now.Second);
                Console.ResetColor();
                
                var tmp = AllPosibleArrangements(data, width, height, curLstNumber);
                var arrForLstNum = CleanupDuplicates(tmp);

                foreach (List<List<Figure>> arrangement in arrForLstNum)
                {
                    Dbg1(arrangement);
                    if (DoesCurrentArrangementFit(arrangement, width, height))
                    {
                        maxLstNumber = curLstNumber;
                        res = arrangement;
                        Console.WriteLine("Yes");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("No");
                    }
                }
                if (maxLstNumber != curLstNumber)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(curLstNumber + ": No");
                    Console.ResetColor();
                    minLstNumber = curLstNumber + 1;
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

                var tmp = GetWorkingArrangement(data, width, height, curLstNumber);
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

        public static List<List<Figure>> FindMinArrangement(List<Figure> data, int width, int height)
        {
            data.Sort(Figure.CompareFiguresBySize);
            List<List<Figure>> res = new List<List<Figure>>();

            for (int i = 0; i < data.Count;)
            {
                ;
            }

            return res;
        }

        private static int GetHalf(int min, int max)
        {
            return min + (int)Math.Floor((double)((max - min) / 2));
        }
    }
}
