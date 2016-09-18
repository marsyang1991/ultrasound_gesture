using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication2
{
    public partial class ImageForm : Form
    {
        int m = 0;
        int bf = 1;
        int bh = 3;
        List<Image> list;
        public delegate void MyInvoke(int result); 
        public ImageForm()
        {
            InitializeComponent();
            list = new List<Image>();
            loadImage(list);
            setImage();
            
        }

        public void loadImage(List<Image> list)
        {
            list.Add(Properties.Resources._1);
            list.Add(Properties.Resources._2);
            list.Add(Properties.Resources._3);
            list.Add(Properties.Resources._4);
            list.Add(Properties.Resources._5);
            list.Add(Properties.Resources._6);
            list.Add(Properties.Resources._7);
            list.Add(Properties.Resources._8);
            list.Add(Properties.Resources._9);
 
        }
        public void setImage()
        {
            bf = (m-1+list.Count)%list.Count;
            bh = (m+1)%list.Count;
            beforeImage.Image = list[bf];
            mainImage.Image = list[m];
            behindImage.Image = list[bh];
        }
        public void right()
        {
            m = (--m+list.Count) % list.Count;
            setImage();
        }
        public void left()
        {
            m = ++m % list.Count;
            setImage();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                left();
                return;
            }
            if (e.KeyCode == Keys.Right)
            {
                right();
                return;
            }
        }

        private void ImageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void beforeImage_Click(object sender, EventArgs e)
        {

        }
    }
}
