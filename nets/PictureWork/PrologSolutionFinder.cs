using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using System.IO;

namespace PictureWork
{
    public static class PrologSolutionFinder
    {
        public static List<ResultData> CreateAndRunTest(List<Figure> data, int width, int height, string prologCodePath = "..\\..\\..\\main.pl")
        {
            string predName = "testFigsNoTurn";
            string createdPredicate = QueryCreator.CreateFigPredNoTurn(width, height, data, predName);

            string tmpCodePath = "..\\..\\..\\tmp_main.pl";

            if (File.Exists(tmpCodePath))
                File.Delete(tmpCodePath);
            File.Copy(prologCodePath, tmpCodePath);
            File.AppendAllText(tmpCodePath, "\n");
            File.AppendAllText(tmpCodePath, createdPredicate);

            List<ResultData> res = new List<ResultData>();
            try
            {
                String[] param = { "-q", "-f", tmpCodePath };
                PlEngine.Initialize(param);
                var vars = QueryCreator.CreateListOfResulVars(data.Count);
                string queryStr = predName + "(" + String.Join(",", vars) + ").";

                //Console.WriteLine("\n\nGenerated query:\n" + queryStr);
                Console.WriteLine("Starting solution finder.");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        List<string> resultStrings = new List<string>();
                        foreach (string variable in vars)
                        {
                            string result = GetResultStringString(v[variable]);
                            Console.WriteLine(variable + "," + result);
                            resultStrings.Add(variable + "," + result);
                        }

                        res.Add(new ResultData(resultStrings, false));
                    }
                }
            }
            catch (PlException e)
            {
                Console.WriteLine(e.MessagePl);
                Console.WriteLine(e.Message);
            }
            finally
            {
                PlEngine.PlCleanup();
            }
            return res;
        }

        public static List<ResultData> CreateAndRunTestTurning(List<Figure> data, int width, int height, string prologCodePath = "..\\..\\..\\main.pl")
        {
            string predName = "testFigsTurn";
            string createdPredicate = QueryCreator.CreatePredTurn(width, height, data, predName);

            string tmpCodePath = "..\\..\\..\\tmp_main.pl";

            AppendStrToFile(tmpCodePath, prologCodePath, createdPredicate);

            List<ResultData> res = new List<ResultData>();
            try
            {
                String[] param = { "-q", "-f", tmpCodePath };
                PlEngine.Initialize(param);
                string queryStr = "findall(X, " + predName + "(X), Lx).";

                //Console.WriteLine("\n\nGenerated query:\n" + queryStr);
                Console.WriteLine("Starting solution finder.");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        res = GetFindallRes(v["Lx"]);
                    }
                }
            }
            catch (PlException e)
            {
                Console.WriteLine(e.MessagePl);
                Console.WriteLine(e.Message);
            }
            finally
            {
                PlEngine.PlCleanup();
            }
            return res;
        }

        public static List<ResultData> CreateAndRunTestTurningOptimizedFindall(List<Figure> data, int width, int height, string prologCodePath = "..\\..\\..\\main.pl")
        {
            string predName = "testFigsTurnOpt";
            string createdPredicate = QueryCreator.CreatePredTurnOptimized(width, height, data, predName);

            string tmpCodePath = "..\\..\\..\\tmp_main.pl";

            AppendStrToFile(tmpCodePath, prologCodePath, createdPredicate);

            List<ResultData> res = new List<ResultData>();
            try
            {
                String[] param = { "-q", "-f", tmpCodePath };
                PlEngine.Initialize(param);
                string queryStr = "findall(X, " + predName + "(X), Lx).";

                //Console.WriteLine("\n\nGenerated query:\n" + queryStr);
                Console.WriteLine("Starting solution finder.");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        res = GetFindallRes(v["Lx"]);
                    }
                }
            }
            catch (PlException e)
            {
                Console.WriteLine(e.MessagePl);
                Console.WriteLine(e.Message);
            }
            finally
            {
                PlEngine.PlCleanup();
            }
            return res;
        }

        private static void DbgCurLst(List<Figure> curLst)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("-----------");
            curLst.ForEach((fig) =>
            {
                Console.Write(fig.name + " ");
            });
            Console.WriteLine();

            Console.ResetColor();
        }

        public static bool DoesFiguresFit(List<Figure> data, int width, int height, string prologCodePath = "..\\..\\..\\main.pl")
        {
            DbgCurLst(data);
            string predName = "testFit";
            data.Sort(Figure.CompareFiguresBySize);
            string createdPredicate = QueryCreator.CreatePredTurnOptimized(width, height, data, predName);

            string tmpCodePath = "..\\..\\..\\tmp_main.pl";

            AppendStrToFile(tmpCodePath, prologCodePath, createdPredicate);

            bool res = false;
            try
            {
                String[] param = { "-q", "-f", tmpCodePath };
                PlEngine.Initialize(param);
                string queryStr = predName + "(Ans).";

                Console.WriteLine("Starting solution finder.");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        var tmp = v["Ans"];
                        return true;
                    }
                    return false;
                }
            }
            catch (PlException e)
            {
                Console.WriteLine(e.MessagePl);
                Console.WriteLine(e.Message);
            }
            finally
            {
                PlEngine.PlCleanup();
            }
            return res;
        }

        public static ResultData GetAnyResult(List<Figure> data, int width, int height, string prologCodePath = "..\\..\\..\\main.pl")
        {
            DbgCurLst(data);
            string predName = "testFit";
            string createdPredicate = QueryCreator.CreatePredTurnOptimized(width, height, data, predName);

            string tmpCodePath = "..\\..\\..\\tmp_main.pl";

            AppendStrToFile(tmpCodePath, prologCodePath, createdPredicate);

            bool res = false;
            try
            {
                String[] param = { "-q", "-f", tmpCodePath };
                PlEngine.Initialize(param);
                string queryStr = predName + "(Ans).";

                Console.WriteLine("Starting solution finder.");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        ResultData result = ResultData.GetRes(GetResultStringList(v["Ans"]));
                        return result;
                    }
                    return null;
                }
            }
            catch (PlException e)
            {
                Console.WriteLine(e.MessagePl);
                Console.WriteLine(e.Message);
            }
            finally
            {
                PlEngine.PlCleanup();
            }
            return null;
        }


        private static void AppendStrToFile(string tmpCodePath, string prologCodePath, string strToAppend)
        {
            if (File.Exists(tmpCodePath))
                File.Delete(tmpCodePath);
            File.Copy(prologCodePath, tmpCodePath);
            File.AppendAllText(tmpCodePath, "\n");
            File.AppendAllText(tmpCodePath, strToAppend);
        }

        private static List<ResultData> GetFindallRes(PlTerm res)
        {
            List<ResultData> convertedRes = new List<ResultData>();
            var allResults = res.ToList();
            foreach (PlTerm curAns in allResults)
            {
                convertedRes.Add(new ResultData(curAns.ToListString()));

            }
            return convertedRes;
        }

        private static ResultData GetResult(string resultString, bool nameWithAngle)
        {
            return new ResultData(resultString, nameWithAngle);
        }


        private static string GetResultStringList(PlTerm res)
        {
            return String.Join(" ", res.ToList());
        }

        private static string GetResultStringString(PlTerm res)
        {
            return String.Join(" ", res.ToString());
        }
    }
}
