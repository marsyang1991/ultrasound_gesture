using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Threading;


namespace WindowsFormsApplication2
{
    public partial class MainForm : Form
    {
        private SoundRecorder recorder = null;
        private bool g = true;
        bool isHMM = false;
        int minute = 0;
        SoundPlayer player = null;
        int second = 0;
        ImageForm form2;
        public MainForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
           // button1.PerformClick();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            form2 = new ImageForm();
           // this.Hide();
            form2.Show();
            timer1.Enabled = true;
            timer1.Start();
            player = new SoundPlayer(WindowsFormsApplication2.Properties.Resources._20khz);
            player.Load();
            player.PlayLooping();
            timer1.Interval = 1000;
            Thread.Sleep(100);
            recorder = new SoundRecorder(textBox1, textBox2, textBox3,g,isHMM,form2);
            recorder.RecStart();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (recorder != null)
            {
                recorder.RecStop();
                timer1.Enabled = false;
                player.Stop();
                player.Dispose();
                player = null;
                recorder = null;
                button1.Enabled = true;
                button2.Enabled = false;
                
            }
            else
            {
                MessageBox.Show("录音未开始，请点击开始按钮");
            }
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            second++;
            if (second == 60)
            {
                minute++;
                second = 0;
            }
            string secondstr = string.Format("{0:D2}", second);
            string minutestr = string.Format("{0:D2}", minute);
            label1.Text = minutestr + ":" + secondstr;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) 
            {
                button1.PerformClick();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            minute = 0;
            second = 0;
            string secondstr = string.Format("{0:D2}", second);
            string minutestr = string.Format("{0:D2}", minute);
            label1.Text = minutestr + ":" + secondstr;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (g)
            {
                g = false;
                label5.Text = "FFT is chosen";
                this.Text = "SoundWaveSensor(FFT)";

            }
            else
            {
                g = true;
                label5.Text = "Goertzel is chosen";
                this.Text = "SoundWaveSensor(Goertzel)";
            }
            if (recorder != null)
            {
                recorder.RecStop();
                recorder = new SoundRecorder(textBox1, textBox2, textBox3, g, isHMM,form2);
                recorder.RecStart();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (isHMM)
            {
               isHMM = false;
                label6.Text = "规则 is running";

            }
            else
            {
                isHMM = true;
                label6.Text = "HMM is running";
               
            }
            if (recorder != null)
            {
                recorder.RecStop();
                recorder = new SoundRecorder(textBox1, textBox2, textBox3, g, isHMM,form2);
                recorder.RecStart();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
