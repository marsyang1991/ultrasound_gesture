namespace WindowsFormsApplication2
{
    partial class ImageForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.beforeImage = new System.Windows.Forms.PictureBox();
            this.behindImage = new System.Windows.Forms.PictureBox();
            this.mainImage = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.beforeImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.behindImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainImage)).BeginInit();
            this.SuspendLayout();
            // 
            // beforeImage
            // 
            this.beforeImage.Location = new System.Drawing.Point(11, 132);
            this.beforeImage.Margin = new System.Windows.Forms.Padding(2);
            this.beforeImage.Name = "beforeImage";
            this.beforeImage.Size = new System.Drawing.Size(111, 114);
            this.beforeImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.beforeImage.TabIndex = 2;
            this.beforeImage.TabStop = false;
            this.beforeImage.Click += new System.EventHandler(this.beforeImage_Click);
            // 
            // behindImage
            // 
            this.behindImage.Location = new System.Drawing.Point(603, 132);
            this.behindImage.Margin = new System.Windows.Forms.Padding(2);
            this.behindImage.Name = "behindImage";
            this.behindImage.Size = new System.Drawing.Size(104, 114);
            this.behindImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.behindImage.TabIndex = 1;
            this.behindImage.TabStop = false;
            // 
            // mainImage
            // 
            this.mainImage.Location = new System.Drawing.Point(184, 11);
            this.mainImage.Margin = new System.Windows.Forms.Padding(2);
            this.mainImage.Name = "mainImage";
            this.mainImage.Size = new System.Drawing.Size(355, 377);
            this.mainImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.mainImage.TabIndex = 0;
            this.mainImage.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(539, 181);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "单击-->";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(127, 181);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "<--双击";
            // 
            // ImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(718, 399);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.beforeImage);
            this.Controls.Add(this.behindImage);
            this.Controls.Add(this.mainImage);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ImageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ImageViewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form1_KeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.beforeImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.behindImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mainImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox mainImage;
        private System.Windows.Forms.PictureBox behindImage;
        private System.Windows.Forms.PictureBox beforeImage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}