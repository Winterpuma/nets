using System;
using System.Collections.Generic;
using DataClassLibrary;

namespace SolveTask.ServerCodeGenerators
{
    static class QueryCreator
    {
        
        /// <summary>
        /// [(-1,[(0,1)]),
        /// (0,[(-1, 2)]),
        /// (1,[(-1, -1),(2, 2)])]
        /// </summary>
        public static string CreateFigFromDict(SortedDictionary<int, List<Segment>> figure)
        {
            List<string> distinctY = new List<string>();

            foreach (KeyValuePair<int, List<Segment>> kvp in figure)
            {
                distinctY.Add("(" + kvp.Key + ",[" + String.Join(",", kvp.Value) + "])");
            }

            return "[" + String.Join(",", distinctY) + "]";
        }
        

        public static string CreateFigOneSize(Figure oneSizeFig)
        {
            List<string> allAngles = new List<string>();

            int angleStep = oneSizeFig.angleStep;
            DeltaRepresentation deltaToTurn = oneSizeFig.withBorderDistance;

            for (int i = 0; i < 360; i += angleStep)
            {
                DeltaRepresentation curDelta = deltaToTurn.GetTurnedDelta(i); //TODO using?? // TODO WouldDeltaFit check
                
                {
                    SegmentRepresentation sr = new SegmentRepresentation(curDelta.GetDictRepresentation());
                    allAngles.Add("(" +
                        i + ", [" + String.Join(",", sr.segments[0]) + "]," + sr.GetMinMaxYLine() + ","
                        + CreateFigFromDict(sr.segments) + ")");
                }
            }

            string scaleStr = oneSizeFig.scaleCoef.ToString(System.Globalization.CultureInfo.InvariantCulture);

            return "fig" + oneSizeFig.id + "(Fig, " + scaleStr + ") :- " +
                "Fig = [" + String.Join(",", allAngles) + "].";
        }

        public static string GetAnsQuery(int width, int height, double scale, List<int> figInd)
        {
            string queryStr = "generate(" + (width - 1) + "," + (height - 1) + ", Field),";
            List<string> figNames = new List<string>();
            string scaleStr = scale.ToString(System.Globalization.CultureInfo.InvariantCulture);
            foreach (int i in figInd)
            {
                queryStr += "fig" + i + "(Fig" + i + "," + scaleStr + "),";
                figNames.Add("((0," + (width - 1) + "),(0," + (height - 1) + "), (0,359),Fig" + i + ")");
            }
            queryStr += "place_figures_in_range([" + String.Join(",", figNames) + "],Field, Ans, _).";
            return queryStr;
        }

        public static string GetAnsQuery(int width, int height, double scale, ResultData prevScaleRes, List<int> figInd)
        {
            string queryStr = "generate(" + (width - 1) + "," + (height - 1) + ", Field),";
            List<string> figLocationInfo = new List<string>();
            string scaleStr = scale.ToString(System.Globalization.CultureInfo.InvariantCulture);
            for (int j = 0; j < figInd.Count; j++)
            {
                queryStr += "fig" + figInd[j] + "(Fig" + figInd[j] + "," + scaleStr + "),";
                figLocationInfo.Add("(" + prevScaleRes.GetApproxLocationForNextFig(j, scale, width, height) + 
                    ", Fig" + figInd[j] + ")");
            }
            queryStr += "place_figures_in_range([" + String.Join(",", figLocationInfo) + "],Field, Ans, _).";
            return queryStr;
        }

        public static string GetAnsQuery(List<int> width, List<int> height, List<double> scales, List<int> figInd)
        {
            List<string> queryForEachScale = new List<string>();
            queryForEachScale.Add(GetFieldAndFigsFirstQuery(figInd, 0));
            for (int iSize = 1; iSize < scales.Count; iSize++)
                queryForEachScale.Add(GetFieldAndFigsQueryWithPrevRes(iSize, figInd));

            return "myQuery(Ans" + (width.Count - 1) + "):-\n\t" +
                GetConstants(width, "Width") + "\n\t" +
                GetConstants(height, "Height") + "\n\t" +
                GetConstants(scales, "Scale") + "\n\t" +
                String.Join(",\n\t", queryForEachScale) + ".";
        }

        private static string GetConstants(List<int> constants, string varName)
        {
            string queryStr = "";
            for (int i = 0; i < constants.Count; i++)
                queryStr += varName + i + " is " + constants[i] + ",";
            return queryStr;
        }

        private static string GetConstants(List<double> constants, string varName)
        {
            string queryStr = "";
            for (int i = 0; i < constants.Count; i++)
                queryStr += varName + i + " is " + constants[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + ",";
            return queryStr;
        }

        private static string GetFieldAndFigsFirstQuery(List<int> figInd, int i = 0)
        {
            List<string> figNames = new List<string>();
            string queryStr =
                TemplateGenerate(i) + ",";
            for (int j = 0; j < figInd.Count; j++)
            {
                queryStr += "fig" + figInd[j] + "(Fig" + figInd[j] + i + ",Scale" + i + "),";
                figNames.Add("((0,Width" + i + "),(0,Height" + i + "), (0,359),Fig" + figInd[j] + i + ")");
            }
            queryStr += "place_figures_in_range([" + String.Join(",", figNames) + "],Field" + i + ", Ans" + i + ", _)";
            return queryStr;
        }

        private static string GetFieldAndFigsQueryWithPrevRes(int i, List<int> figInd)
        {
            List<string> figNames = new List<string>();
            string queryStr =
                TemplateGenerate(i) + ",\n\t" +
                TemplateFigs(i, figInd, figNames) + "\n\t" +
                TemplateKscale(i) + ",\n\t" +
                TemplateGetNextApproc(i, figNames) + ",\n\t" +
                TemplatePlace(i);
            return queryStr;
        }

        private static string TemplateGenerate(int i)
        {
            return "generate(Width" + i + ", Height" + i + ", Field" + i + ")";
        }

        private static string TemplateFigs(int i, List<int> figInd, List<string> figNames)
        {
            string queryStr = "";
            foreach (int ind in figInd)
            {
                figNames.Add("Fig" + ind + i);
                queryStr += "fig" + ind + "(Fig" + ind + i + ", Scale" + i + "),";
            }
            return queryStr;
        }

        private static string TemplateKscale(int i)
        {
            return "Kscale" + i + " is Scale" + i + "/Scale" + (i - 1);
        }

        private static string TemplateGetNextApproc(int i, List<string> figNames)
        {
            return "get_next_approc_coords(Kscale" + i + ",[" + String.Join(",", figNames) + "], Ans" + (i - 1) + ", ApprocFigNewScale" + i + ")";
        }

        private static string TemplatePlace(int i)
        {
            return "place_figures_in_range(ApprocFigNewScale" + i + ",Field" + i + ",Ans" + i + ",_)";
        }


        public static List<string> CreateListOfResulVars(int n)
        {
            List<string> args = new List<string>();
            for (int i = 1; i <= n; i++)
                args.Add("Fig" + i + "pos");
            return args;
        }

        public static List<string> CreateListOfArgs(int n)
        {
            List<string> args = new List<string>();
            for (int i = 1; i <= n; i++)
                args.Add("(X" + i + ",Y" + i + ")");
            return args;
        }

        public static string CreateArgs(int n)
        {
            return "(" + String.Join(",", CreateListOfArgs(n)) + ")";
        }
    }
}
