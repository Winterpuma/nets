using System;
using System.Collections.Generic;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;


namespace PictureWork
{
    class SolutionChecker
    {
        public static List<string> Tryer360(List<Figure> data)
        {
            List<string> res = new List<string>();
            try
            {
                string filename = "..\\..\\..\\main.pl";
                String[] param = { "-q", "-f", filename };
                PlEngine.Initialize(param);

                string queryStr = "tryer360(" +
                    QueryCreator.GetPrologAllRotatedFigureArrayRepresentationWithAngle(data) +
                    ",Res).";

                Console.WriteLine("Starting tryer360");
                //Console.WriteLine("\n\nGenerated query:\n" + queryStr);

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        Console.WriteLine("-");
                        res.Add(GetResStr(v["Res"]));
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

        public static List<string> F1(List<Figure> data)
        {
            List<string> res = new List<string>();
            try
            {
                string filename = "..\\..\\..\\main.pl";
                String[] param = { "-q", "-f", filename };
                PlEngine.Initialize(param);

                string queryStr = "f1(" + 
                    QueryCreator.GetPrologOriginalFigureArrayRepresentation(data) +
                    ",[],Res).";

                //Console.WriteLine("\n\nGenerated query:\n" + queryStr);
                Console.WriteLine("Starting prolog f1");

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                        res.Add(GetResStr(v["Res"]));
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
        

        public static string GetResStr(PlTerm res)
        {
            string resStr = String.Join(" ", res.ToList());
                /*
            foreach (PlTerm line in res.ToList())
            {
                resStr += line.ToString() + " ";
            }*/

            return resStr;
        }
        

        /*
        public static void GetOverlay()
        {
            try
            {
                string filename = "..\\..\\..\\program.pl";
                String[] param = { "-q", "-f", filename };
                PlEngine.Initialize(param);

                using (PlQuery q = new PlQuery(
                    "plane_img(Matr)," +
                    "figure_one(Deltas)," +
                    "overlay(Matr, Deltas, 3, 3, Res)."))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                        PrintMatr(v["Res"]);
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
        }

        public static void PrintMatr(PlTerm matr)
        {
            foreach (string line in matr.ToListString())
            {
                Console.WriteLine(line);
            }
        }
        */
    }
}
