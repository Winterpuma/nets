﻿using System;
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
        Color figColor = Color.FromArgb(155, 155, 155);
        string pathRes = "../../../../result/";
        int angleStep = 10;

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
            //var result = PrologSolutionFinder.FindResultBeforeTimout(lst, width, height);
            lst.Sort(Figure.CompareFiguresBySize);
            var result = PrologSolutionFinder.GetAnyResult(lst, width, height);

            if (result == null)
            {
                MessageBox.Show("Решение не найдено");
            }
            else
            {
                Bitmap resultPicture = OutputHandling.SaveOneSingleListResult(lst, result, width, height, pathRes);
                pictureBoxResult.Size = resultPicture.Size;
                pictureBoxResult.Image = resultPicture;
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
