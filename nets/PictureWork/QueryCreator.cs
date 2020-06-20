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
        /// <summary>
        /// Формирует строку вида [("name1",[[(X,Y),(X,Y)],[(X,Y),(X,Y)]]),("name2",[[(X,Y),(X,Y)],[(X,Y),(X,Y)]])]
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
        /// Формирует строку вида [("name1",[(X,Y),(X,Y)]),("name2",[(X,Y),(X,Y)])]
        /// </summary>
        public static string GetPrologOriginalFigureArrayRepresentation(List<Figure> figs)
        {
            string[] figsPrologRepr = new string[figs.Count];
            for (int i = 0; i < figs.Count; i++)
                figsPrologRepr[i] = GetPrologOriginalFigureRepresentation(figs[i]);

            return "[" + String.Join(",", figsPrologRepr) + "]";
        }

        /// <summary>
        /// Формирует строку вида ("name",[[(X,Y),(X,Y)],[(X,Y),(X,Y)]])
        /// Т.е у фигуры есть массив повернутых представлений.
        /// </summary>
        public static string GetPrologAllRotatedFigureRepresentation(Figure fig)
        {
            string[] figPrologRepr = new string[fig.rotated.Count];
            for (int i = 0; i < fig.rotated.Count; i++)
                figPrologRepr[i] = GetPrologDeltaRepresentation(fig[i]);

            return "(\"" + fig.name + "\",[" + String.Join(",", figPrologRepr) + "])";
        }

        /// <summary>
        /// Формирует строку вида ("name",[(X,Y),(X,Y)])
        /// </summary>
        public static string GetPrologOriginalFigureRepresentation(Figure fig)
        {
            return "(\"" + fig.name + "\"," + GetPrologDeltaRepresentation(fig[0]) + ")";
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

    }
}
