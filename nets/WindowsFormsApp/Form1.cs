using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using PictureWork;
using DataClassLibrary;
using IO;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        List<string> choseenFigsPath = new List<string>();
        List<Figure> data = new List<Figure>();


        // Параметры
        static string pathPrologCode; // Путь к директории с кодом пролога

        static string pathTmp; // Путь для сохранения первично отмасштабированных фигур
        static string pathRes; // Путь для сохранения результата и лога

        static Color srcFigColor; // Цвет фигур на загружаемой картинке
        static int figAmount; // Кол-во каждой из фигур

        static Size lstSize; // Размер листа
        static double scale; // Коэф-т первоначального масштабирования

        static int angleStep = 1; // Шаг поворотов фигур
        static int borderDistance; // Отступ от границы фигур (чтобы не слипались)
        static List<double> scaleCoefs;



        public Form1()
        {
            //Environment.SetEnvironmentVariable("Path", @"D:\\Program Files (x86)\\swipl\\bin");
            InitializeComponent();
            imageList1.ImageSize = new Size(100, 50);
            this.AcceptButton = button1;
        }

        /// <summary>
        /// Инициализация параметров на основе файла конфигурации
        /// </summary>
        void InitConfiguration()
        {
            pathPrologCode = @"..\..\..\PrologCode\"; // Путь к директории с кодом пролога

            pathTmp = @"tmp\"; // Путь для сохранения первично отмасштабированных фигур
            pathRes = @"res\"; // Путь для сохранения результата и лога

            srcFigColor = ColorTranslator.FromHtml("#9B9B9B"); ; // Цвет фигур на загружаемой картинке
            figAmount = 1; // Кол-во каждой из фигур

            lstSize = new Size(Convert.ToInt32(textBox_w.Text), Convert.ToInt32(textBox_h.Text)); // Размер листа
            scale = 0.1; // Коэф-т первоначального масштабирования

            angleStep = 1; // Шаг поворотов фигур
            borderDistance = Convert.ToInt32(textBox_distance.Text); // Отступ от границы фигур (чтобы не слипались)
            scaleCoefs = new List<double>() { 0.5, 1 };

            
        }

        private void buttonAddFig_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true }; // Filter = "PNG|*.png"

            //int distance = Convert.ToInt32(textBox_distance.Text);

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach(string filename in ofd.FileNames)
                {
                    choseenFigsPath.Add(filename);
                    //Figure loadedFig = new Figure(filename, data.Count, figColor, angleStep, distance);
                    //data.Add(loadedFig);

                    //FileInfo fi = new FileInfo(filename);
                    listViewPreview.Items.Add(Path.GetFileNameWithoutExtension(filename), imageList1.Images.Count);
                    imageList1.Images.Add(Image.FromFile(filename));
                }

            }
        }

        private void button_FindAnswer_Click(object sender, EventArgs e)
        {
            if (choseenFigsPath.Count == 0)
            {
                MessageBox.Show("Не выбраны детали");
                return;
            }

            InitConfiguration();

            CleanDir(pathTmp);
            CleanDir(pathRes);
            

            // Масштабирование
            Size scaledLstSize = new Size((int)(lstSize.Width * scale), (int)(lstSize.Height * scale));
            InputHandling.ScaleAllFigs(choseenFigsPath, pathTmp, scale);


            // Загрузка фигур
            Console.WriteLine("Starting process. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            List<Figure> data = Figure.LoadFigures(pathTmp, srcFigColor, angleStep, borderDistance, figAmount);
            data.Sort(Figure.CompareFiguresBySize);
            Figure.UpdIndexes(data);
            Figure.DeleteWrongAngles(scaledLstSize.Width, scaledLstSize.Height, data);
            SolutionChecker.LoadFigures(data, pathPrologCode, scaleCoefs);

            Console.WriteLine("Figure loading finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);


            // Поиск решения
            Console.WriteLine("Starting result finding. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            var preDefArr = SolutionChecker.FindAnAnswer(data, scaledLstSize.Width, scaledLstSize.Height, pathPrologCode, scaleCoefs);
            var result = SolutionChecker.PlacePreDefinedArrangement(preDefArr, scaledLstSize.Width, scaledLstSize.Height, scaleCoefs);
            if (result == null)
                MessageBox.Show("Prolog finished. No answer.");
            else
            {
                MessageBox.Show("Prolog finished. Answer was found.");
                // Отображение решения
                Console.WriteLine("Starting visualization. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                OutputImage.SaveResult(data, preDefArr, result, pathRes, scaledLstSize.Width, scaledLstSize.Height);
                OutputText.SaveResult(preDefArr, data, result, pathRes + "result.txt");
            }


            Console.WriteLine("Process finished. " + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            Console.ReadLine();

        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            choseenFigsPath.Clear();
            data.Clear();
            listViewPreview.Items.Clear();
            imageList1.Images.Clear();
            pictureBoxResult.Image = null;
        }

        private void textBox_distance_KeyUp(object sender, KeyEventArgs e)
        {
            MessageBox.Show("недоступно");
            return;

            if (e.KeyData == Keys.Enter)
            {
                int borderDst = Convert.ToInt32(textBox_distance.Text);
                foreach (Figure fig in data)
                    ;// fig.ChangeBorderDistance(borderDst);
            }
        }

        /// <summary>
        /// Создание новой директории или удаление содержимого существующей
        /// </summary>
        public static void CleanDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo curDir in dir.GetDirectories())
            {
                curDir.Delete(true);
            }
        }
    }
}
