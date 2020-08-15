using System;
using System.Collections.Generic;

namespace DataClassLibrary
{
    public class ResultFigPos
    {
        public string name = "noname";
        public int xCenter { get; set; }
        public int yCenter { get; set; }
        public double angle { get; set; }


        public ResultFigPos(string name, int xCenter, int yCenter, double angle = 0)
        {
            this.name = name;
            this.xCenter = xCenter;
            this.yCenter = yCenter;
            this.angle = angle;
        }

        public override string ToString()
        {
            return xCenter + " " + yCenter + " " + angle;
        }
    }


    public class ResultData
    {
        public List<ResultFigPos> answer { get; set; } = new List<ResultFigPos>(); 
        public double scale;
        public int lstWidth;
        public int lstHeight;

        public ResultData(){ }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positioningData">список из строк по 3: x, y, angle, x2, y2, angle</param>
        public ResultData(System.Collections.ObjectModel.Collection<string> positioningData)
        {
            int i = 1;
            foreach (string oneFigData in positioningData)
            {
                var oneFigDataSplitted = oneFigData.Split(','); 
                string name = i.ToString();
                int xCenter = Convert.ToInt32(oneFigDataSplitted[0]);
                int yCenter = Convert.ToInt32(oneFigDataSplitted[1]);
                double angle = Convert.ToDouble(oneFigDataSplitted[2]);
                answer.Add(new ResultFigPos(name, xCenter, yCenter, angle));
                i++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ResultData GetRes(string result)
        {
            var res = new ResultData();
            string name;
            int xCenter, yCenter;
            double angle = 0;
            int i = 0;
            foreach (string figure in result.Split(' '))
            {
                var tmp = figure.Split(',');
                name = i.ToString();
                xCenter = Convert.ToInt32(tmp[0]);
                yCenter = Convert.ToInt32(tmp[1]);
                angle = Convert.ToDouble(tmp[2]);

                res.answer.Add(new ResultFigPos(name, xCenter, yCenter, angle));
                i++;
            }
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result">f1,1,1 f2,3,1 f3,5,1 or f1_320,1,2 f2_15,0,5</param>
        /// <param name="nameWithAngle"></param>
        public ResultData(string result, bool nameWithAngle = false)
        {
            string name;
            int xCenter, yCenter;
            double angle = 0;
            foreach (string figure in result.Split(' '))
            {
                var tmp = figure.Split(',');
                if (nameWithAngle)
                {
                    var seperatorIndex = tmp[0].LastIndexOf('_');
                    name = tmp[0].Substring(0, seperatorIndex);
                    angle = Convert.ToDouble(tmp[0].Substring(seperatorIndex + 1));
                }
                else
                {
                    name = tmp[0];
                }

                xCenter = Convert.ToInt32(tmp[1]);
                yCenter = Convert.ToInt32(tmp[2]);

                answer.Add(new ResultFigPos(name, xCenter, yCenter, angle));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"> список строк вида Fig2pos,120,23</param>
        /// <param name="nameWithAngle"></param>
        public ResultData(List<string> result, bool nameWithAngle = false)
        {
            string name;
            int xCenter, yCenter;
            double angle = 0;
            foreach (string figure in result)
            {
                var tmp = figure.Split(',');
                if (nameWithAngle)
                {
                    var seperatorIndex = tmp[0].LastIndexOf('_');
                    name = tmp[0].Substring(0, seperatorIndex);
                    angle = Convert.ToDouble(tmp[0].Substring(seperatorIndex + 1));
                }
                else
                {
                    name = tmp[0];
                }

                xCenter = Convert.ToInt32(tmp[1]);
                yCenter = Convert.ToInt32(tmp[2]);

                answer.Add(new ResultFigPos(name, xCenter, yCenter, angle));
            }
        }

        public void SetLstInfo(int lstW, int lstH, double scale)
        {
            lstHeight = lstH;
            lstWidth = lstW;
            this.scale = scale;
        }


        /// <summary>
        /// Возвращает диапазоны
        /// </summary>
        /// <returns>(45,50),(55,65),(0,15)</returns>
        public string GetApproxLocationForNextFig(int indFig, double newScale, int newLstWidth, int newLstHeight)
        {
            ResultFigPos figRes = answer[indFig];

            double kScale = newScale / scale;
            int dMove = (kScale > 1) ? (int)Math.Ceiling(kScale) * 2 : 1;
            int space = newLstWidth / 20;
            space = Math.Max(dMove, space);

            int newXCenter = (int) (figRes.xCenter * kScale);
            int newYCenter = (int) (figRes.yCenter * kScale);


            int magicNum = 10;
            int xL = (int)((figRes.xCenter -            magicNum) * kScale);
            int xR = (int)((figRes.xCenter + dMove +    magicNum) * kScale);
            int yL = (int)((figRes.yCenter -            magicNum) * kScale);
            int yR = (int)((figRes.yCenter + dMove +    magicNum) * kScale);
            /*
            return "(" + xL + "," + xR + ")," +
                "(" + yL + "," + yR + ")," +
                GetFigRange((int)figRes.angle, 3, 360);
            *////*
            return GetFigRange(newXCenter + dMove, space, newLstWidth) + "," +
                GetFigRange(newYCenter + dMove, space, newLstHeight) + "," +
                GetFigRange((int)figRes.angle, 3, 360); // 359? а если угол отрицательный, то по хорошему тоже нужно проверить
        }

        public string GetApproxLocationForNextFig2(int indFig, double newScale, int newLstWidth, int newLstHeight)
        {
            ResultFigPos figRes = answer[indFig];

            double kScale = newScale / scale;
            int dMove = (kScale > 1) ? (int)Math.Ceiling(kScale) * 2 : 1;

            int newXCenter = (int)(figRes.xCenter * kScale); 
            int newYCenter = (int)(figRes.yCenter * kScale);

            int magicNum = 10;
            int xL = newXCenter - magicNum;
            int xR = newXCenter + magicNum;
            int yL = newYCenter - magicNum;
            int yR = newYCenter + magicNum;
            /*
            int xL = (int)((figRes.xCenter - magicNum) * kScale);
            int xR = (int)((figRes.xCenter + dMove + magicNum) * kScale);
            int yL = (int)((figRes.yCenter - magicNum) * kScale);
            int yR = (int)((figRes.yCenter + dMove + magicNum) * kScale);*/
            
            return "(" + xL + "," + xR + ")," +
                "(" + yL + "," + yR + ")," +
                GetFigRange((int)figRes.angle, 3, 360);
            
        }

        private string GetFigRange(int center, int dCenter, int maxSize)
        {
            int minCoord = center - dCenter;
            if (minCoord < 0)
                minCoord = 0;
            int maxCoord = center + dCenter;
            if (maxCoord > maxSize)
                maxCoord = maxSize;
            return "(" + minCoord + "," + maxCoord + ")";
        }

        public static List<ResultData> PackAllPossibleResults(IEnumerable<string> allResults, bool flagNameWithAngle)
        {
            List<ResultData> res = new List<ResultData>();
            foreach (string currentResult in allResults)
                res.Add(new ResultData(currentResult, flagNameWithAngle));
            return res;
        }

        public override string ToString()
        {
            return String.Join("\n", answer);
        }
    }
}
