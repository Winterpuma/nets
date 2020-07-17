using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PictureWork;
using System.Drawing;

namespace WindowsFormsApp
{
    public partial class Form1 : Form
    {
        List<Figure> lst = new List<Figure>();
        Color figColor = Color.FromArgb(155, 155, 155);
        int angleStep = 10;
        string pathRes = "../../../../result/";

        public Form1()
        {
            Environment.SetEnvironmentVariable("Path", @"D:\\Program Files (x86)\\swipl\\bin");
            InitializeComponent();
        }

        private void buttonAddFig_Click(object sender, EventArgs e)
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToOpen = FD.FileName;
                Figure loadedFig = new Figure(fileToOpen, lst.Count, figColor, angleStep);
                lst.Add(loadedFig);
            }
        }

        private void button_FindAnswer_Click(object sender, EventArgs e)
        {
            int width = Convert.ToInt32(textBox_w.Text);
            int height = Convert.ToInt32(textBox_h.Text);
            var result = PrologSolutionFinder.GetAnyResult(lst, width, height);

            if (result == null)
            {
                MessageBox.Show("Решение не найдено");
            }
            else
            {
                OutputHandling.SaveOneSingleListResult(lst, result, width, height, pathRes);
            }

        }
    }
}
