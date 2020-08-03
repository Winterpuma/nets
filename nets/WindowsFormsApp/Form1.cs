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
        List<Figure> lst = new List<Figure>();
        Color figColor = Color.FromArgb(155, 155, 155);//Color.FromArgb(0, 0, 0);//
        string pathRes = "../../../../result/";
        string pathProlog = @"D:\GitHub\nets\nets\PictureWork\";
        int angleStep = 1;

        public Form1()
        {
            Environment.SetEnvironmentVariable("Path", @"D:\\Program Files (x86)\\swipl\\bin");
            InitializeComponent();
            imageList1.ImageSize = new Size(100, 50);
            this.AcceptButton = button1;
        }

        private void buttonAddFig_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true }; // Filter = "PNG|*.png"

            int distance = Convert.ToInt32(textBox_distance.Text);

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach(string filename in ofd.FileNames)
                {
                    Figure loadedFig = new Figure(filename, lst.Count, figColor, angleStep, distance);
                    lst.Add(loadedFig);

                    //FileInfo fi = new FileInfo(filename);
                    listViewPreview.Items.Add(Path.GetFileNameWithoutExtension(filename), imageList1.Images.Count);
                    imageList1.Images.Add(Image.FromFile(filename));
                }

            }
        }

        private void button_FindAnswer_Click(object sender, EventArgs e)
        {
            if (lst.Count == 0)
            {
                MessageBox.Show("Не выбраны детали");
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(pathRes);
            foreach (FileInfo file in dir.GetFiles())
                file.Delete();

            int width = Convert.ToInt32(textBox_w.Text);
            int height = Convert.ToInt32(textBox_h.Text);
            lst.Sort(Figure.CompareFiguresBySize);
            FigureFileOperations.CreateNewFigFile(pathProlog + "figInfo.pl");
            FigureFileOperations.AddManyFigs(lst, 1);

            int[] figInd = new int[lst.Count];
            for (int i = 0; i < figInd.Length; i++)
                figInd[i] = i;

            var result = PrologSolutionFinder.GetAnyResult(width, height, 1, figInd);

            if (result == null)
            {
                MessageBox.Show("Решение не найдено");
            }
            else
            {
                Bitmap resultPicture = OutputImage.SaveOneSingleListResult(lst, result, width, height, pathRes);
                pictureBoxResult.Size = resultPicture.Size;
                pictureBoxResult.Image = resultPicture;
                OutputText.SaveOneSingleListResult(lst, result, "result.txt");
            }

        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            lst.Clear();
            listViewPreview.Items.Clear();
            imageList1.Images.Clear();
            pictureBoxResult.Image = null;
        }

        private void textBox_distance_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                int borderDst = Convert.ToInt32(textBox_distance.Text);
                foreach (Figure fig in lst)
                    fig.ChangeBorderDistance(borderDst);
            }
        }
    }
}
