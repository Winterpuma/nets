using System;
using System.Collections.Generic;
using System.Globalization;
using DataClassLibrary;

namespace SolveTask.ServerCodeGenerators
{
    static class QueryCreator
    {
        private static readonly CultureInfo CultureInfo = CultureInfo.InvariantCulture;

        private static readonly string GenerateField = "generate";
        private static readonly string Query = "myQuery";
        private static readonly string PlaceFiguresInRange = "place_figures_in_range";
        private static readonly string GetNextApprocCoords = "get_next_approc_coords";

        private static string JoinArray(IEnumerable<string> array) =>
            "[" + String.Join(", ", array) + "]";

        private static string JoinArray(IEnumerable<Segment> array) =>
            "[" + String.Join(", ", array) + "]";

        private static string PredicateCall(string name, params string[] arguments) =>
            $"{name}({String.Join(", ", arguments)})";

        private static string PlaceFiguresInRangeLine(List<string> figLocationInfo, string index = "") =>
            PredicateCall(PlaceFiguresInRange, JoinArray(figLocationInfo), $"Field{index}", $"Ans{index}", "_");


        //private static string Generat

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
                distinctY.Add($"({kvp.Key}, {JoinArray(kvp.Value)})");
            }

            return JoinArray(distinctY);
        }
        
        /// <summary>
        /// Создание фигур одного размера
        /// </summary>
        /// <param name="oneSizeFig"></param>
        /// <returns></returns>
        public static string CreateFigOneSize(Figure oneSizeFig)
        {
            List<string> allAngles = new List<string>();

            int angleStep = oneSizeFig.angleStep;
            DeltaRepresentation deltaToTurn = oneSizeFig.withBorderDistance;

            for (int angle = 0; angle < 360; angle += angleStep)
            {
                DeltaRepresentation curDelta = deltaToTurn.GetTurnedDelta(angle); //TODO using?? // TODO WouldDeltaFit check
                {
                    SegmentRepresentation sr = new SegmentRepresentation(curDelta.GetDictRepresentation());
                    allAngles.Add($"({angle}, {JoinArray(sr.segments[0])}, {sr.GetMinMaxYLine()}, {CreateFigFromDict(sr.segments)})");
                }
            }

            string scaleStr = oneSizeFig.scaleCoef.ToString(CultureInfo);

            return $"fig{oneSizeFig.id}(Fig, {scaleStr}) :- Fig = {JoinArray(allAngles)}.";
        }

        public static string GetAnsQuery(int width, int height, double scale, List<int> figInd)
        {
            string queryStr = $"{GenerateField}(" + (width - 1) + "," + (height - 1) + ", Field),";
            List<string> figNames = new List<string>();
            string scaleStr = scale.ToString(CultureInfo);
            foreach (int i in figInd)
            {
                queryStr += "fig" + i + "(Fig" + i + "," + scaleStr + "),";
                figNames.Add("((0," + (width - 1) + "),(0," + (height - 1) + "), (0,359),Fig" + i + ")");
            }
            queryStr += PlaceFiguresInRangeLine(figNames) + ".";
            return queryStr;
        }

        public static string GetAnsQuery(int width, int height, double scale, ResultData prevScaleRes, List<int> figInd)
        {
            string queryStr = $"{GenerateField}(" + (width - 1) + "," + (height - 1) + ", Field),";
            List<string> figLocationInfo = new List<string>();
            string scaleStr = scale.ToString(CultureInfo);
            for (int j = 0; j < figInd.Count; j++)
            {
                queryStr += "fig" + figInd[j] + "(Fig" + figInd[j] + "," + scaleStr + "),";
                figLocationInfo.Add("(" + prevScaleRes.GetApproxLocationForNextFig(j, scale, width, height) + 
                    ", Fig" + figInd[j] + ")");
            }
            queryStr += PlaceFiguresInRangeLine(figLocationInfo) + ".";
            return queryStr;
        }

        public static string GetAnsQuery(List<int> width, List<int> height, List<double> scales, List<int> figInd)
        {
			List<string> queryForEachScale = new List<string>
			{
				GetFieldAndFigsFirstQuery(figInd, 0)
			};

			for (int iSize = 1; iSize < scales.Count; iSize++)
                queryForEachScale.Add(GetFieldAndFigsQueryWithPrevRes(iSize, figInd));

            return $"{Query}(Ans" + (width.Count - 1) + "):-\n\t" +
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
                queryStr += varName + i + " is " + constants[i].ToString(CultureInfo) + ",";
            return queryStr;
        }

        private static string GetFieldAndFigsFirstQuery(List<int> figInd, int i = 0)
        {
            List<string> figNames = new List<string>();
            string queryStr = TemplateGenerate(i) + ",";

            for (int j = 0; j < figInd.Count; j++)
            {
                queryStr += $"fig{figInd[j]}(Fig{figInd[j]}{i}, Scale{i}),";
                figNames.Add($"((0,Width{i}), (0,Height{i}), (0,359), Fig{figInd[j]}{i})");
            }

            queryStr += PlaceFiguresInRangeLine(figNames, i.ToString());
            return queryStr;
        }

        private static string GetFieldAndFigsQueryWithPrevRes(int i, List<int> figInd)
        {
            List<string> figNames = new List<string>();
            string queryStr =
                TemplateGenerate(i) + ",\n\t" +
                TemplateFigs(i, figInd, figNames) + "\n\t" +
                TemplateKscale(i) + ",\n\t" +
                TemplateGetNextApprocCoords(i, figNames) + ",\n\t" +
                TemplatePlace(i);
            return queryStr;
        }

        private static string TemplateGenerate(int i)
        {
            return PredicateCall(GenerateField, $"Width{i}", $"Height{i}", $"Field{i}");
        }

        private static string TemplateFigs(int i, List<int> figInd, List<string> figNames)
        {
            string queryStr = "";
            foreach (int ind in figInd)
            {
                figNames.Add($"Fig{ind}{i}");
                queryStr += $"fig{ind}(Fig{ind}{i}, Scale{i}),";
            }
            return queryStr;
        }

        private static string TemplateKscale(int i)
        {
            return $"Kscale{i} is Scale{i}/Scale{i - 1}";
        }

        private static string TemplateGetNextApprocCoords(int i, List<string> figNames)
        {
            return PredicateCall(GetNextApprocCoords, $"Kscale{i}", JoinArray(figNames), $"Ans{i - 1}", $"ApprocFigNewScale{i}");
        }

        private static string TemplatePlace(int i)
        {
            return PredicateCall(PlaceFiguresInRange, $"ApprocFigNewScale{i}", $"Field{i}", $"Ans{i}", "_");
        }
    }
}
