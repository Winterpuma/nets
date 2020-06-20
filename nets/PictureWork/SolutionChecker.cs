using System;
using System.Collections.Generic;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;


namespace PictureWork
{
    class SolutionChecker
    {
        public static void GetSolutions(List<Figure> data)
        {
            try
            {
                string filename = "..\\..\\..\\main.pl";
                String[] param = { "-q", "-f", filename };
                PlEngine.Initialize(param);

                string queryStr = "f1(" + 
                    QueryCreator.GetPrologOriginalFigureArrayRepresentation(data) +
                    ",[],Res).";

                Console.WriteLine("\n\nGenerated query:\n" + queryStr);

                using (PlQuery q = new PlQuery(queryStr))
                {
                    foreach (PlQueryVariables v in q.SolutionVariables)
                        ShowRes(v["Res"]);
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

        public static void ShowRes(PlTerm res)
        {
            Console.Write("Res = ");

            foreach (PlTerm line in res.ToList())
            {
                Console.Write(line.ToString() + " ");
            }
            Console.WriteLine();
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
