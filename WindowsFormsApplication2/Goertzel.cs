using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication2
{
    class Goertzel
    {

        /*
         * 处理块的大小==音频数据的数组大小
         * 频率分辨率为fs/N=21.5hz
         * 响应时间为N/fs =46.4ms
         * **/
        static int fs = 44100;//采样频率hz
        static int N = 2048;//处理块的大小
        static double resolution = fs / N;//分辨率大小
        static int basicFreq = 20000;//基本频率值
        static int n = 15;//basicFreq 前后计算的数量
        static double percent = 0.1;//基础值谱宽
        static int num = 2;//判断是否手势的阈值
        double[] dataDouble = null;
        //手势识别过程中，记录手势的方向
        static int one = 0;
        static int two = 0;
        static int three = 0;
        static int count = 0;//记录手势方向的次数
        //记录双击的方向
        static int a = 0;
        static int b = 0;
        //记录连续的手势方向，判断手势结束，若r=q1 = q2 = 0，则手势结束
        static int q1 = 3;
        static int q2 = 3;
        static int q3 = 3;
        static bool ingesture = false;//判断是否在手势识别中
        static List<int> sequence = new List<int>();//记录手势方向序列
        //输出手势方向序列
        static double[] temp = null;
        public static List<int> record = new List<int>();
        public static List<double[]> none = new List<double[]>();
        public static List<double[]> qian = new List<double[]>();
        public static List<double[]> hou = new List<double[]>();
        //HMM参数
        //HMM1参数
        static Double[,] A1 = {{1.0000,0.0000,0.0000},
                            {0.0000,0.6136,0.3864},
                            {0.2373,0.0146,0.7481}};
        static Double[,] B1 = {{ 0.9575,0.0425,0},
                            {0.0015,0.9985,0},
                            {0.1657,0.8343,0}};

        static Double[] PI1 = { 0, 1, 0 };

        //HMM2参数
        static Double[,] A2 = {{0.4645,0.2076 , 0.3279},
                            {0.0079,0.9396 ,0.0525},
                            {0.5342,0.4142, 0.0516}};
        static Double[,] B2 = {{ 0.0021, 0 ,0.9979},
                            {0.7529 ,0 , 0.2471},
                            {0.0952 , 0  ,0.9048}};
        static Double[] PI2 = { 0.9316, 0.0000, 0.0684 };

        //HMM3参数 
        static Double[,] A3 = {{  0.7235  ,  0.0000   , 0.2765},
      {0.0688  ,  0.8154  ,  0.1158},
      {0.3227   , 0.0000  ,  0.6773}};
        static Double[,] B3 = {{0.0371  ,  0.0188  ,  0.9440},
    {0.1466 ,   0.8534   , 0.0000},
    {0.9793   , 0.0002    ,0.0205}};
        static Double[] PI3 = { 0.0000, 1.0000, 0.0000 };
        //HMM4参数
        static Double[,] A4 = {{ 0.4575  ,  0.0000   , 0.5425},
    {0.5266,    0.4733   , 0.0002},
    {0.0000   , 0.0942   , 0.9058}};
        static Double[,] B4 = {{ 0.1302  ,  0.8698  ,  0.0000},
    {0.1349 ,   0.8651  ,  0.0000},
    {0.4198  ,  0.0000   , 0.5802}};
        static Double[] PI4 = { 0.0000, 1.0000, 0.0000 };

        static HMM[] hmms = new HMM[3];

        HMM hmm1 = new HMM(A1.GetLength(0), B1.GetLength(1));
        HMM hmm2 = new HMM(A2.GetLength(0), B2.GetLength(1));
        HMM hmm3 = new HMM(A3.GetLength(0), B3.GetLength(1));
        HMM hmm4 = new HMM(A4.GetLength(0), B3.GetLength(1));
        public ImageForm form = null;
        public Goertzel()
        {
            setHMM();
        }
        private void setHMM()
        {
            hmm1.A = A1;
            hmm1.B = B1;
            hmm1.PI = PI1;
            hmms[0] = hmm1;

            hmm2.A = A2;
            hmm2.B = B2;
            hmm2.PI = PI2;
            hmms[1] = hmm2;

            hmm3.A = A3;
            hmm3.B = B3;
            hmm3.PI = PI3;
            hmms[2] = hmm3;

            hmm4.A = A4;
            hmm4.B = B4;
            hmm4.PI = PI4;
            // hmms[3] = hmm4;
        }
        /*
      * 方法名：HammingWindow
      * 功能：海明窗
      * 参数：Complex[]
      * 返回：COmplex[]
      */

        public double[] HammingWindow(double[] data)
        {
            int N = data.Length;
            for (int n = 0; n < N; n++)
            {
                data[n] *= 0.54f - 0.46f * Math.Cos((2 * Math.PI * (n + 1)) / (N - 1));
            }
            return data;
        }
        /*
         * 方法名：getMagnitude
         * 功能：根据一段音频数据，得到确定频率处的幅度值
         * 参数：
         * double[] data,音频数据
         * int tf,频率值
         * 返回值：
         * double result，幅度值
         * **/
        private double getMagnitude(double[] data, double tf)
        {
            double result = 0.0;
            double k = (N * tf) / fs;
            double w = (2 * Math.PI / N) * k;
            double cosine = Math.Cos(w);
            double sine = Math.Sin(w);
            double coeff = 2 * cosine;
            double q0 = 0, q1 = 0, q2 = 0;
            for (int i = 0; i < data.Length; i++)
            {
                q0 = coeff * q1 - q2 + data[i];
                q2 = q1;
                q1 = q0;
            }
            double real = (q1 - q2 * cosine);
            double image = q2 * sine;
            result = Math.Sqrt(real * real + image * image);
            //result = q1 * q1 + q2 * q2 - q1 * q2 * coeff;
            return result;
        }

        /*
         * 方法名：detect
         * 功能：根据音频数据，计算特定频率的幅度值，判断是否发生手势
         * 参数：
         * double[] data，音频数据
         * 返回值：
         * int r，1代表向前，2代表向后,0代表none
         * **/
        private int detect(double[] data, TextBox tb)
        {
            int r = 0;
            temp = new double[2 * n + 1];
            double[] zeros = new double[2 * n + 1];
            for (int i = 0; i < zeros.Length; i++)
            {
                zeros[i] = 0.003;
            }
            zeros[14] = 0.45;
            zeros[15] = 1;
            zeros[16] = 0.45;
            data = HammingWindow(data);
            double[] p = new double[n];
            double[] t = new double[n];
            double basicR = getMagnitude(data, (basicFreq/resolution)*resolution);
            temp[n] = 1;
            for (int i = 0; i < n; i++)
            {
                p[i] = getMagnitude(data, basicFreq - (i + 1) * resolution) / basicR;
                temp[n - 1 - i] = p[i];
                t[i] = getMagnitude(data, basicFreq + (i + 1) * resolution) / basicR;
                temp[i + 1 + n] = t[i];
            }

            try
            {
                double max = double.MinValue;
                int index = 15;
                for (int i = 0; i < 2*n+1; i++)
                {
                    double d = temp[i] - zeros[i];
                    //Console.Write(d+"\t");
                    if (d > max)
                    {
                        max = d;
                        index = i;
                    }
                }
                if (max < 0.01)
                    r = 0;
                else if (index >17)
                {
                    r = 1;
                }
                else if (index < 13)
                {
                    r = 2;
                }
                /*int j = 0;
                while (p[j++] > percent) ;
                int g = 0;
                while (t[g++] > percent) ;
                int tj = j;
                int tg = g;

                if (j > num)
                {
                    r = 2;
                }

                else
                {
                    if (g > num)
                    {
                        r = 1;
                    }

                }*/
                record.Add(r);
                switch (r)
                {
                    case 1:
                       // tb.AppendText("after:" + g + ",before:" + j + "向前-------\n");
                        tb.AppendText(index+"向前-------\t"+"\n");
                        qian.Add(temp);
                        //save(dataDouble, 1);
                        break;
                    case 2:
                        //tb.AppendText("after:" + g + ",before:" + j + "--------向后\n");
                        tb.AppendText(index+"--------向后\t"+"\n");
                        hou.Add(temp);
                        //save(dataDouble, 2);
                        break;
                    default:
                       // tb.AppendText("after:" + g + ",before:" + j + "\n");
                       tb.AppendText(index+"\n");
                        none.Add(temp);
                        //save(dataDouble, 0);
                        break;
                }
           //  save2(temp, index);

            }
            catch (Exception)
            {
                r = 0;
            }

            return r;
        }

        /*
         * 方法名：PcmToDouble
         * 功能：将Pcm音频数据转化为double类型
         * 参数：byte[] data，音频Pcm数据
         * 返回：double[]
         * **/
        private double[] PcmToDouble(byte[] data)
        {
            double[] waveDouble = new double[data.Length / 2];
            int h = 0;
            for (int i = 0; i < data.Length; i += 2)
            {
                waveDouble[h] = (double)BitConverter.ToInt16(data, i);
                h++;
            }
            return waveDouble;
        }

        /*
         * 方法名：show
         * 功能：根据音频数据，在结果栏里展示手势识别结果
         * 参数：
         * byte[] data,音频数据
         * TextBox tb，展示结果区
         * 返回：void
         * **/
        public void show(byte[] data, TextBox tb, TextBox tb2, bool isHMM,ImageForm form)
        {

            dataDouble = PcmToDouble(data);
            int r = detect(dataDouble, tb);
            secondDectect(r, tb2, isHMM,form);
        }

        /*
     * 方法名：secondDectect
     * 功能：
     * 参数：int，detect中每个缓冲区的结果
     * 返回：void
    */
        private void secondDectect(int result, TextBox tb, bool isHMM,ImageForm form)
        {
            if (ingesture == false && result == 0)//无移动，不进行识别，当识别到有移动方向时开始进行手势识别
                return;
            ingesture = true;
            if (result != 0)
                sequence.Add(result);
            if (q3==q2&&q2 == q1 && q1 == result && result == 0)//判断手势结束
            {
                int[] OB = sequence.ToArray();//将手势序列转化为数组
                List<int> temp = sequence;
                //wirteFile();
                sequence.Clear();
                if (OB.Length >= 3)
                {
                    ingesture = false;
                    int index = -1;
                    if (isHMM)
                    {

                        double[] rr = new double[hmms.Length];
                        double max_pro = Double.MinValue;

                        for (int i = 0; i < hmms.Length; i++)
                        {
                            rr[i] = hmms[i].Forward(OB);
                            if (max_pro < rr[i])
                            {
                                max_pro = rr[i];
                                index = i;
                            }

                        }

                    }
                    else//规则方法
                    {
                        List<State> states = new List<State>();
                        for (int i = 0; i < OB.Length; i++)
                        {
                            if (states.Count == 0 || OB[i] != states[states.Count - 1].kind)
                            {
                                State s = new State();
                                s.kind = OB[i];
                                s.duration = 1;
                                states.Add(s);
                            }
                            else
                            {
                                states[states.Count - 1].duration++;
                            }
                        }
                        for (int i = 0; i < states.Count; i++)
                        {
                            if (states[i].duration <2)
                            {
                                states.RemoveAt(i);
                                i--;
                            }
                        }
                        for (int i = 1; i < states.Count; i++)
                        {
                            if (states[i].kind == states[i - 1].kind)
                            {
                                states[i - 1].duration += states[i].duration;
                                states.RemoveAt(i);
                                i--;
                            }
                        }
                        if (states.Count == 1)
                            index = states[0].kind-1;
                        if (states.Count == 2||states.Count==3)
                            index = 2;
                        if (states.Count == 4)
                            index = 3;
                        foreach(State s in states)
                        {
                            Console.Write(s.kind +":"+s.duration+ " ");
                            
                        }
                        Console.WriteLine();
                    }
                    switch (index)
                    {
                        case 0:
                            tb.AppendText("向前移动\n");
                           // SendKeys.Send("RIGHT");
                            
                            break;
                        case 1:
                            tb.AppendText("向后移动\n");
                           // SendKeys.Send("LEFT");
                            break;
                        case 2:
                            tb.AppendText("单击\n");
                            break;
                        case 3:
                            tb.AppendText("双击\n");
                            break;
                    }
                    form.BeginInvoke(new ImageForm.MyInvoke(UpdateForm), new object[] { index });
                    OB = null;
                }
            }

            //记录前两个手势方向
            q3 = q2;
            q2 = q1;
            q1 = result;
        }
        public void UpdateForm(int result)
        {
            if (result == 2)
            {
                form.left();
                Application.DoEvents();
            }
            if (result == 3)
            {
                form.right();
                Application.DoEvents();
            }
            else {
                return;
            }
        }
        private void save(double[] data, int i)
        {
            FileStream fs = new FileStream("D:/audioData/" + i + ".txt", FileMode.Append);
            StreamWriter writer = new StreamWriter(fs);
            foreach (double d in data)
            {
                writer.Write(d + " ");
            }
            writer.Write("\n");
            writer.Flush();
            writer.Dispose();
            writer = null;
            fs.Dispose();
            fs = null;
        }

        private void save2(double[] data, int i)
        {
            FileStream fs = new FileStream("C:/data/full.txt", FileMode.Append);
            StreamWriter writer = new StreamWriter(fs);
            writer.Write(i + " ");
            foreach (double d in data)
            {
                writer.Write(d + " ");
            }
            writer.Write("\n");
            Console.WriteLine("done");
            writer.Flush();
            writer.Dispose();
            writer = null;
            fs.Dispose();
            fs = null;
        }
        class State 
        {
            public int kind;
            public int duration;
        }

    }
}
