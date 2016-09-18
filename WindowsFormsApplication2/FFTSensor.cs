using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication2
{
    class FFTSensor
    {
        const int UNITCOUNT = 3;
        static int count = 0;
        static int one = 0;
        static int two = 0;
        static int three = 0;
        static int a = 0;
        static int b = 0;
        static int[] signals = new int[10];
        static int q1 = 3;
        static int q2 = 3;
        static bool ingesture = false;//判断是否在手势识别中
        static List<int> sequence = new List<int>();//记录手势方向序列
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
         Goertzel newg = new Goertzel();
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
        public void Manage(byte[] data, TextBox tb,TextBox tb2,TextBox tb3,bool g,bool isHMM,ImageForm form)
        {
            setHMM();
            if (g)
            {
                newg.form = form;
                newg.show(data, tb3,tb2,isHMM,form);
            }
            else
            {
                Complex[] wave = PcmToComplex(data);
                Complex[] wave1 = HammingWindow(wave);
                Complex[] resultFFT = FFT(wave1);
                double[] result = Cmp2Mdl(resultFFT);
                int r = detect(result, tb);
                secondDectect(r, tb2,isHMM);
            }

        }
        /*
       * 方法名：PcmToComplex
       * 功能：将Pcm数据转化为Complex数组
       * 参数：byte[] 录制的音频
       * 返回：Complex[]
       */
        public static Complex[] PcmToComplex(byte[] data)
        {
            Complex[] wave = new Complex[data.Length / 2];

            int h = 0;
            for (int i = 0; i < data.Length; i += 2)
            {
                wave[h] = new Complex((double)BitConverter.ToInt16(data, i));
                ++h;
            }
            return wave;
        }

        /*
       * 方法名：HammingWindow
       * 功能：海明窗
       * 参数：Complex[]
       * 返回：COmplex[]
       */

        public static Complex[] HammingWindow(Complex[] data)
        {
            int N = data.Length;
            for (int n = 0; n < N; n++)
            {
                data[n].Real *= 0.54f - 0.46f * Math.Cos((2 * Math.PI * (n + 1)) / (N - 1));
            }
            return data;
        }

        //FFT
        /*
         * 方法名：FFT
         * 功能：实现快速傅里叶变换
         * 参数：Complex[]，经过Hamming窗的Complex[]类型数据
         * 返回：Complex[]，FFT后的结果
        */
        public static Complex[] FFT(Complex[] input)
        {
            if (input.Length == 1)
            {
                return new Complex[] { input[0] };
            }
            int length = input.Length;
            int half = length / 2;
            Complex[] output = new Complex[length];
            double fac = -2.0 * Math.PI / length;//转换因子的基数
            ///序列中下标为偶数的点
            Complex[] evens = new Complex[half];

            for (int i = 0; i < half; i++)
            {
                evens[i] = input[2 * i];
            }

            ///求偶数点FFT或IFFT的结果，递归实现多级蝶形运算
            Complex[] evenResult = FFT(evens);

            ///序列中下标为奇数的点
            Complex[] odds = new Complex[half];

            for (int i = 0; i < half; i++)
            {
                odds[i] = input[2 * i + 1];
            }

            ///求偶数点FFT或IFFT的结果，递归实现多级蝶形运算
            Complex[] oddResult = FFT(odds);

            for (int k = 0; k < half; k++)
            {
                double fack = fac * k;//旋转因子
                Complex oddPart = oddResult[k] * new Complex(Math.Cos(fack), Math.Sin(fack));
                output[k] = evenResult[k] + oddPart;
                output[k + half] = evenResult[k] - oddPart;
            }
            return output;
        }
 
        /*
         * 方法名：Cmp2Mdl
         * 功能：将结果有复数形式转变为double类型
         * 参数：Complex[]
         * 返回：double[]
        */
        public static double[] Cmp2Mdl(Complex[] input)
        {
            double[] output = new double[20];
            for (int i = 0; i < output.Length; i++)
            {
               // output[i] = 10 * Math.Log10(input[i+900].ToModul());
                output[i] = input[i + 919].ToModul();
            }

            return output;
        }
        /*
         * 方法名：detect
         * 功能：对结果进行分析，寻找谱峰宽度，正常情况下是前后各2，前段超过3则表示向后移动，后段超过3则表示向前移动
         * 参数：double[]，结果的double[]
         * 返回：int，向前返回1，向后返回2
        */
        private static int detect(double[] data, TextBox tb)
        {
            try
            {
                int peak = 1;
                double energy = data[peak];
                for (int i = peak; i < data.Length; i++)
                {
                    if (data[i] > energy)
                    {
                        peak = i;
                        energy = data[peak];
                    }

                }
                int p = peak - 1;
                int t = peak + 1;
                double ee = energy * 0.1;
                double ff = energy * 0.2;
                while (data[t] > ee)
                    t++;

                while (data[p] > ee)
                    p--;
                int secondt = t + 1;
                // int temp = secondt;

                // int secondp = p - 1;

                int after = t - peak;
                int before = peak - p;
               /* if (after >= 3 && before >= 3)
                {
                   // tb.AppendText("after:" + after + ",before:" + before + "---剪刀手---\n");
                    return 3;
                }*/
                if (after >= 3)
                {
                   tb.AppendText( "after:" + after + ",before:" + before + "向前------\n");
                    return 1;
                }
                if (before >= 3)
                {
                    tb.AppendText( "after:" + after + ",before:" + before + "-------向后\n");
                    return 2;
                }

                while (t < data.Length) 
                {
                    if (data[t] > ff)
                        break;
                    t++;
                }
                if (t < data.Length)
                {
                    tb.AppendText("t:" + t + "快向前------\n");
                    return 1;
                }

                while (p > 0)
                {
                    if (data[p] > ff)
                        break;
                    p--;
                }
                if (p > 0) 
                {
                    tb.AppendText("p:" + p + "------快向后\n");
                    return 2;
                }
                tb.AppendText("after:" + after + ",before:" + before+"\n");
                return 0;

            }
            catch (Exception)
            {
                return 0;
            }
            
        }
        /*
        * 方法名：secondDectect
        * 功能：
        * 参数：int，detect中每个缓冲区的结果
        * 返回：void
       */
        private static void secondDectect(int result, TextBox tb,bool isHMM)
        {
            if (ingesture == false && result == 0)//无移动，不进行识别，当识别到有移动方向时开始进行手势识别
                return;
            ingesture = true;
            sequence.Add(result);
            if (q2 == q1 && q1 == result && result == 0)//判断手势结束
            {
                int[] OB = sequence.ToArray();//将手势序列转化为数组
                sequence.Clear();
                if (OB.Length > 6)
                {
                    ingesture = false;
                    int index = 0;
                    if (isHMM)
                    {
                        //  wirteFile();
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
                        for (int i = 0; i < OB.Length; i++)
                        {
                            switch (OB[i])
                            {
                                case 1:
                                    one++;
                                    break;
                                case 2:
                                    two++;
                                    break;
                                case 0:
                                    three++;
                                    break;
                            }
                            if (one >= 3 && two >= 3)
                            {
                                index = 2;

                            }
                            else
                            {
                                if (one >= 5)
                                {
                                    index = 0;
                                }
                                if (two >= 5)
                                {
                                    index = 1;
                                }
                            }
                        }
                        one = 0;
                        two = 0;
                        three = 0;
                    }

                    switch (index)
                    {
                        case 0:
                            tb.AppendText("向前移动\n");
                            break;
                        case 1:
                            tb.AppendText("向后移动\n");
                            break;
                        case 2:
                            tb.AppendText("单击\n");
                            break;
                        case 3:
                            tb.AppendText("双击\n");
                            break;
                    }
                    OB = null;
                }
            }

            //记录前两个手势方向
            q2 = q1;
            q1 = result;
        }


        


    }
}
