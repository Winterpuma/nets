using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PictureWork
{
    static class QueryCreator
    {
        #region solution 1
        /// <summary>
        /// Формирует строку вида  [[(name1_angle1,[(X,Y),(X,Y)]),(name1_angle2,[(X,Y),(X,Y)])],
        ///                         [(name2_angle1,[(X,Y),(X,Y)]),(name2_angle2,[(X,Y),(X,Y)])]]
        /// Т.е у каждой фигуры есть массив повернутых представлений.
        /// </summary>
        public static string GetPrologAllRotatedFigureArrayRepresentationWithAngle(List<Figure> figs)
        {
            string[] figsPrologRepr = new string[figs.Count];
            for (int i = 0; i < figs.Count; i++)
                figsPrologRepr[i] = GetPrologAllRotatedFigureRepresentationWithAngle(figs[i]);

            return "[" + String.Join(",", figsPrologRepr) + "]";
        }

        /// <summary>
        /// Формирует строку вида  [(name1,[[(X,Y),(X,Y)],[(X,Y),(X,Y)]]),
        ///                         (name2,[[(X,Y),(X,Y)],[(X,Y),(X,Y)]])]
        /// Т.е у каждой фигуры есть массив повернутых представлений.
        /// </summary>
        public static string GetPrologAllRotatedFigureArrayRepresentation(List<Figure> figs)
        {
            string[] figsPrologRepr = new string[figs.Count];
            for (int i = 0; i < figs.Count; i++)
                figsPrologRepr[i] = GetPrologAllRotatedFigureRepresentation(figs[i]);

            return "[" + String.Join(",", figsPrologRepr) + "]";
        }

        /// <summary>
        /// Формирует строку вида [(name1,[(X,Y),(X,Y)]),(name2,[(X,Y),(X,Y)])]
        /// </summary>
        public static string GetPrologOriginalFigureArrayRepresentation(List<Figure> figs)
        {
            string[] figsPrologRepr = new string[figs.Count];
            for (int i = 0; i < figs.Count; i++)
                figsPrologRepr[i] = GetPrologOriginalFigureRepresentation(figs[i]);

            return "[" + String.Join(",", figsPrologRepr) + "]";
        }
        

        /// <summary>
        /// Формирует строку вида [(name_angle,[(X,Y),(X,Y)]),(name_angle,[(X,Y),(X,Y)])]
        /// Т.е у фигуры есть массив повернутых представлений.
        /// </summary>
        public static string GetPrologAllRotatedFigureRepresentationWithAngle(Figure fig)
        {
            string[] figPrologRepr = new string[fig.rotated.Count];
            for (int i = 0; i < fig.rotated.Count; i++)
                figPrologRepr[i] = GetPrologDeltaRepresentationWithAngle(fig[i], fig.name, fig.rotated[i].angle);

            return "[" + String.Join(",", figPrologRepr) + "]";
        }


        /// <summary>
        /// Формирует строку вида (name,[[(X,Y),(X,Y)],[(X,Y),(X,Y)]])
        /// Т.е у фигуры есть массив повернутых представлений.
        /// </summary>
        public static string GetPrologAllRotatedFigureRepresentation(Figure fig)
        {
            string[] figPrologRepr = new string[fig.rotated.Count];
            for (int i = 0; i < fig.rotated.Count; i++)
                figPrologRepr[i] = GetPrologDeltaRepresentation(fig[i]);

            return "(" + fig.name + ",[" + String.Join(",", figPrologRepr) + "])";
        }


        /// <summary>
        /// Формирует строку вида (name,[(X,Y),(X,Y)])
        /// </summary>
        public static string GetPrologOriginalFigureRepresentation(Figure fig)
        {
            return "(" + fig.name + "," + GetPrologDeltaRepresentation(fig[0]) + ")";
        }


        /// <summary>
        /// Формирует строку вида (name_angle,[(X,Y),(X,Y)])
        /// </summary>
        public static string GetPrologDeltaRepresentationWithAngle(List<Point> delta, string figName, double angle)
        {
            string[] deltaPrologRepr = new string[delta.Count];
            for (int i = 0; i < delta.Count; i++)
                deltaPrologRepr[i] = GetPrologPointRepresentation(delta[i]);

            return "(" + figName + "_" + angle.ToString() + "," +
                "[" + String.Join(",", deltaPrologRepr) + "])";
        }


        /// <summary>
        /// Формирует строку вида [(X,Y),(X,Y)]
        /// </summary>
        public static string GetPrologDeltaRepresentation(List<Point> delta)
        {
            string[] deltaPrologRepr = new string[delta.Count];
            for (int i = 0; i < delta.Count; i++)
                deltaPrologRepr[i] = GetPrologPointRepresentation(delta[i]);

            return "[" + String.Join(",", deltaPrologRepr) + "]";
        }

        /// <summary>
        /// Формирует строку вида (X,Y)
        /// </summary>
        public static string GetPrologPointRepresentation(Point point)
        {
            return "(" + point.X.ToString() + "," + point.Y.ToString() + ")";
        }
        #endregion

        /// <summary>
        /// Формирует строку вида [(0,[0,1,2,3]),(1,[0,1,2,3])]
        /// При x = 4, y = 2
        /// </summary>
        public static string CreateLst(int x, int y)
        {
            string res = "[";
            string xValues = "[" + String.Join(",", Enumerable.Range(0, x).ToArray()) + "]";

            for (int yCur = 0; yCur < y - 1; yCur++)
            {
                res += "(" + yCur + "," + xValues + "),";
            }
            res += "(" + (y - 1) + "," + xValues + ")]";

            return res;
        }
        

        /// <returns>[(0,[1,0,-1]),(1,[0]),(-1,[0])]</returns>
        public static string CreateFigFromDict(Dictionary<int, List<int>> figure)
        {
            List<string> distinctY = new List<string>();

            foreach (KeyValuePair<int, List<int>> kvp in figure)
            {
                distinctY.Add("(" + kvp.Key + ",[" + String.Join(",", kvp.Value) + "])");
            }

            return "[" + String.Join(",", distinctY) + "]";
        }

        public static string CreateFigFromDict(SortedDictionary<int, List<int>> figure)
        {
            List<string> distinctY = new List<string>();

            foreach (KeyValuePair<int, List<int>> kvp in figure)
            {
                distinctY.Add("(" + kvp.Key + ",[" + String.Join(",", kvp.Value) + "])");
            }

            return "[" + String.Join(",", distinctY) + "]";
        }

        public static string CreateSimpleTest(int sizeX, int sizeY, Figure fig1, Figure fig2)
        {
            string res = "test1((X,Y),(X2,Y2)):-";
            res += "Lst = " + CreateLst(sizeX, sizeY) + ",";
            var transformed1 = fig1.rotated[0].TransformDeltaToDict();
            var transformed2 = fig2.rotated[0].TransformDeltaToDict();
            res += "Fig1 = (X,Y," + CreateFigFromDict(transformed1) + "),";
            res += "Fig2 = (X2,Y2," + CreateFigFromDict(transformed2) + "),";
            res += "place_it([Fig1,Fig2],Lst).";
            return res;
        }

    }
}
