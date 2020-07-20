namespace WindowsFormsApp
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox_w = new System.Windows.Forms.TextBox();
            this.textBox_h = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonAddFig = new System.Windows.Forms.Button();
            this.button_FindAnswer = new System.Windows.Forms.Button();
            this.listViewPreview = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.pictureBoxResult = new System.Windows.Forms.PictureBox();
            this.button_Clear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxResult)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox_w
            // 
            this.textBox_w.Location = new System.Drawing.Point(59, 60);
            this.textBox_w.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_w.Name = "textBox_w";
            this.textBox_w.Size = new System.Drawing.Size(76, 26);
            this.textBox_w.TabIndex = 0;
            this.textBox_w.Text = "300";
            // 
            // textBox_h
            // 
            this.textBox_h.Location = new System.Drawing.Point(59, 96);
            this.textBox_h.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_h.Name = "textBox_h";
            this.textBox_h.Size = new System.Drawing.Size(76, 26);
            this.textBox_h.TabIndex = 1;
            this.textBox_h.Text = "100";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 66);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 102);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 26);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Размер листа";
            // 
            // buttonAddFig
            // 
            this.buttonAddFig.Location = new System.Drawing.Point(31, 152);
            this.buttonAddFig.Name = "buttonAddFig";
            this.buttonAddFig.Size = new System.Drawing.Size(171, 39);
            this.buttonAddFig.TabIndex = 5;
            this.buttonAddFig.Text = "Добавить деталь";
            this.buttonAddFig.UseVisualStyleBackColor = true;
            this.buttonAddFig.Click += new System.EventHandler(this.buttonAddFig_Click);
            // 
            // button_FindAnswer
            // 
            this.button_FindAnswer.Location = new System.Drawing.Point(31, 206);
            this.button_FindAnswer.Name = "button_FindAnswer";
            this.button_FindAnswer.Size = new System.Drawing.Size(171, 39);
            this.button_FindAnswer.TabIndex = 6;
            this.button_FindAnswer.Text = "Решить";
            this.button_FindAnswer.UseVisualStyleBackColor = true;
            this.button_FindAnswer.Click += new System.EventHandler(this.button_FindAnswer_Click);
            // 
            // listViewPreview
            // 
            this.listViewPreview.HideSelection = false;
            this.listViewPreview.LargeImageList = this.imageList1;
            this.listViewPreview.Location = new System.Drawing.Point(230, 36);
            this.listViewPreview.Name = "listViewPreview";
            this.listViewPreview.Size = new System.Drawing.Size(346, 477);
            this.listViewPreview.TabIndex = 7;
            this.listViewPreview.UseCompatibleStateImageBehavior = false;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(64, 64);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // pictureBoxResult
            // 
            this.pictureBoxResult.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBoxResult.Location = new System.Drawing.Point(611, 36);
            this.pictureBoxResult.Name = "pictureBoxResult";
            this.pictureBoxResult.Size = new System.Drawing.Size(574, 476);
            this.pictureBoxResult.TabIndex = 9;
            this.pictureBoxResult.TabStop = false;
            // 
            // button_Clear
            // 
            this.button_Clear.Location = new System.Drawing.Point(31, 262);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(171, 39);
            this.button_Clear.TabIndex = 10;
            this.button_Clear.Text = "Очистить";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1228, 565);
            this.Controls.Add(this.button_Clear);
            this.Controls.Add(this.pictureBoxResult);
            this.Controls.Add(this.listViewPreview);
            this.Controls.Add(this.button_FindAnswer);
            this.Controls.Add(this.buttonAddFig);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_h);
            this.Controls.Add(this.textBox_w);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxResult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_w;
        private System.Windows.Forms.TextBox textBox_h;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonAddFig;
        private System.Windows.Forms.Button button_FindAnswer;
        private System.Windows.Forms.ListView listViewPreview;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBoxResult;
        private System.Windows.Forms.Button button_Clear;
    }
}

