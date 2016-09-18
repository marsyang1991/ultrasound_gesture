using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication2
{
    class HMM
    {
        public readonly Int32 N;//隐藏状态数目N
        public readonly Int32 M;//观察符号数目M
        public Double[,] A;//状态转移矩阵A
        public Double[,] B;//混淆矩阵B
        public Double[] PI;//初始概率向量PI

        //构造函数
        public HMM(Int32 StatesNum, Int32 ObservationSymbolsNum)
        {
            N = StatesNum;
            M = ObservationSymbolsNum;

            A = new Double[N, N];
            B = new Double[N,M];
            PI = new Double[N];
        }

        /*
         * 方法名：Viterbi
         * 功能：维特比算法
         * 描述：通过给定的观察序列，推算出最大可能的隐藏状态序列
         * 参数表：Int32[] OB 观察序列；out Double Probability 隐藏状态序列的概率
         * 返回：Int32[] 最大可能的隐藏状态序列
         */
        public Int32[] Viterbi(Int32[] OB, out Double Probability)
        {
            Double[,] DELTA;
            Int32[,] PSI;

            return Viterbi(OB,out DELTA,out PSI, out Probability);
        }

        private Int32[] Viterbi(int[] OB, out double[,] DELTA, out Int32[,] PSI, out double Probability)
        {
            DELTA = new Double[OB.Length, N];//局部概率
            PSI = new Int32[OB.Length, N];//反向指针

            //1.初始化
            for (int i = 0; i < N; i++)
            {
                DELTA[0, i] = PI[i]*B[i,OB[0]];
            }

            // 2. 递归  
            for (Int32 t = 1; t < OB.Length; t++)
            {
                for (Int32 j = 0; j < N; j++)
                {
                    Double MaxValue = DELTA[t - 1, 0] * A[0, j];
                    Int32 MaxValueIndex = 0;
                    for (Int32 i = 1; i < N; i++)
                    {
                        Double Value = DELTA[t - 1, i] * A[i, j];
                        if (Value > MaxValue)
                        {
                            MaxValue = Value;
                            MaxValueIndex = i;
                        }
                    }

                    DELTA[t, j] = MaxValue * B[j, OB[t]];
                    PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态  
                }
            }

            // 3. 终止  
            Int32[] Q = new Int32[OB.Length];   // 定义最佳路径  
            Probability = DELTA[OB.Length - 1, 0];
            Q[OB.Length - 1] = 0;
            for (Int32 i = 1; i < N; i++)
            {
                if (DELTA[OB.Length - 1, i] > Probability)
                {
                    Probability = DELTA[OB.Length - 1, i];
                    Q[OB.Length - 1] = i;
                }
            }

            // 4. 路径回溯  
            for (Int32 t = OB.Length - 2; t >= 0; t--)
            {
                Q[t] = PSI[t + 1, Q[t + 1]];
            }

            return Q;  
          
        }

        /// <summary>  
        /// 维特比算法：通过给定的观察序列，推算出可能性最大的隐藏状态序列  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <param name="Probability">可能性最大的隐藏状态序列的概率</param>  
        /// <returns>可能性最大的隐藏状态序列</returns>  
        /// <remarks>使用对数运算，不输出中间结果</remarks>  
        public Int32[] ViterbiLog(Int32[] OB, out Double Probability)  
        {  
            Double[,] DELTA;  
            Int32[,] PSI;    
  
            return ViterbiLog(OB, out DELTA, out PSI, out Probability);  
        }  
  
        /// <summary>  
        /// 维特比算法：通过给定的观察序列，推算出可能性最大的隐藏状态序列  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <param name="DELTA">输出中间结果：局部最大概率。结果为自然对数值</param>  
        /// <param name="PSI">输出中间结果：反向指针指示最可能路径</param>  
        /// <param name="Probability">可能性最大的隐藏状态序列的概率</param>  
        /// <returns>可能性最大的隐藏状态序列</returns>   
        /// <remarks>使用对数运算，且输出中间结果</remarks>  
        public Int32[] ViterbiLog(Int32[] OB, out Double[,] DELTA, out Int32[,] PSI, out Double Probability)  
        {              
            DELTA = new Double[OB.Length, N];   // 局部概率  
            PSI = new Int32[OB.Length, N];      // 反向指针  
  
            // 0. 预处理  
            Double[,] LogA = new Double[N, N];  
            for (Int32 i = 0; i < N; i++)  
            {  
                for (Int32 j = 0; j < N; j++)  
                {  
                    LogA[i, j] = Math.Log(A[i, j]);  
                }  
            }  
  
            Double[,] LogBIOT = new Double[N, OB.Length];  
            for (Int32 i = 0; i < N; i++)  
            {  
                for (Int32 t = 0; t < OB.Length; t++)  
                {  
                    LogBIOT[i, t] = Math.Log(B[i, OB[t]]);  
                }  
            }  
  
            Double[] LogPI = new Double[N];  
            for (Int32 i = 0; i < N; i++)  
            {  
                LogPI[i] = Math.Log(PI[i]);  
            }  
  
            // 1. 初始化  
            for (Int32 j = 0; j < N; j++)  
            {  
                DELTA[0, j] = LogPI[j] + LogBIOT[j, 0];  
            }  
  
            // 2. 递归  
            for (Int32 t = 1; t < OB.Length; t++)  
            {  
                for (Int32 j = 0; j < N; j++)  
                {  
                    Double MaxValue = DELTA[t - 1, 0] + LogA[0, j];  
                    Int32 MaxValueIndex = 0;  
                    for (Int32 i = 1; i < N; i++)  
                    {  
                        Double Value = DELTA[t - 1, i] + LogA[i, j];  
                        if (Value > MaxValue)  
                        {  
                            MaxValue = Value;  
                            MaxValueIndex = i;  
                        }  
                    }  
  
                    DELTA[t, j] = MaxValue + LogBIOT[j, t];  
                    PSI[t, j] = MaxValueIndex; // 记录下最有可能到达此状态的上一个状态  
                }  
            }  
  
            // 3. 终止  
            Int32[] Q = new Int32[OB.Length];   // 定义最佳路径  
            Probability = DELTA[OB.Length - 1, 0];  
            Q[OB.Length - 1] = 0;  
            for (Int32 i = 1; i < N; i++)  
            {  
                if (DELTA[OB.Length - 1, i] > Probability)  
                {  
                    Probability = DELTA[OB.Length - 1, i];  
                    Q[OB.Length - 1] = i;  
                }  
            }  
  
            // 4. 路径回溯  
            Probability = Math.Exp(Probability);  
            for (Int32 t = OB.Length - 2; t >= 0; t--)  
            {  
                Q[t] = PSI[t + 1, Q[t + 1]];  
            }  
  
            return Q;  
        }

        /// <summary>  
        /// 前向算法：计算观察序列的概率  
        /// Forward Algorithm: Finding the probability of an observed sequence  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <returns>观察序列的概率</returns>  
        /// <remarks>使用双精度运算，不输出中间结果</remarks>  
        public Double Forward(Int32[] OB)
        {
            Double[,] ALPHA;    // 只声明，不定义  

            return Forward(OB, out ALPHA);
        }

        /// <summary>  
        /// 前向算法：计算观察序列的概率  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <param name="ALPHA">输出中间结果：局部概率</param>  
        /// <returns>观察序列的概率</returns>  
        /// <remarks>使用双精度运算，输出中间结果</remarks>  
        public Double Forward(Int32[] OB, out Double[,] ALPHA)
        {
            ALPHA = new Double[OB.Length, N];   // 局部概率  

            // 1. 初始化：计算初始时刻所有状态的局部概率  
            for (Int32 j = 0; j < N; j++)
            {
                ALPHA[0, j] = PI[j] * B[j, OB[0]];
            }

            // 2. 归纳：递归计算每个时间点的局部概率  
            for (Int32 t = 1; t < OB.Length; t++)
            {
                for (Int32 j = 0; j < N; j++)
                {
                    Double Sum = 0;
                    for (Int32 i = 0; i < N; i++)
                    {
                        Sum += ALPHA[t - 1, i] * A[i, j];
                    }

                    ALPHA[t, j] = Sum * B[j, OB[t]];
                }
            }

            // 3. 终止：观察序列的概率等于最终时刻所有局部概率之和  
            Double Probability = 0;
            for (Int32 i = 0; i < N; i++)
            {
                Probability += ALPHA[OB.Length - 1, i];
            }

            return Probability;
        }

        /// <summary>  
        /// 带比例因子修正的前向算法：计算观察序列的概率  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <param name="ALPHA">中间结果：局部概率</param>  
        /// <param name="SCALE">中间结果：比例因子</param>  
        /// <returns>观察序列的概率（自然对数值）</returns>  
        private Double ForwardWithScale(Int32[] OB, ref Double[,] ALPHA, ref Double[] SCALE)
        {
            if (ALPHA == null) ALPHA = new Double[OB.Length, N];
            if (SCALE == null) SCALE = new Double[OB.Length];

            // 1. 初始化  
            SCALE[0] = 0;
            for (Int32 j = 0; j < N; j++)
            {
                ALPHA[0, j] = PI[j] * B[j, OB[0]];
                SCALE[0] += ALPHA[0, j];
            }

            for (Int32 j = 0; j < N; j++)
            {
                ALPHA[0, j] /= SCALE[0];
            }

            // 2. 归纳  
            for (Int32 t = 1; t < OB.Length; t++)
            {
                SCALE[t] = 0;
                for (Int32 j = 0; j < N; j++)
                {
                    Double Sum = 0;
                    for (Int32 i = 0; i < N; i++)
                    {
                        Sum += ALPHA[t - 1, i] * A[i, j];
                    }

                    ALPHA[t, j] = Sum * B[j, OB[t]];
                    SCALE[t] += ALPHA[t, j];
                }

                for (Int32 j = 0; j < N; j++)
                {
                    ALPHA[t, j] /= SCALE[t];
                }
            }

            // 3. 终止  
            Double Probability = 0;
            for (Int32 t = 0; t < OB.Length; t++)
            {
                Probability += Math.Log(SCALE[t]);
            }

            return Probability;     // 自然对数值  
        }

        /// <summary>  
        /// 后向算法：计算观察序列的概率  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <returns>观察序列的概率</returns>  
        public Double Backward(Int32[] OB)
        {
            Double[,] BETA;     // 只声明，不定义  

            return Backward(OB, out BETA);
        }

        /// <summary>  
        /// 后向算法：计算观察序列的概率  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <param name="BETA">中间结果</param>  
        /// <returns>观察序列的概率</returns>  
        public Double Backward(Int32[] OB, out Double[,] BETA)
        {
            BETA = new Double[OB.Length, N];

            // 初始化  
            for (Int32 j = 0; j < N; j++)
            {
                BETA[OB.Length - 1, j] = 1.0;
            }

            // 归纳  
            for (Int32 t = OB.Length - 2; t >= 0; t--)
            {
                for (Int32 j = 0; j < N; j++)
                {
                    Double Sum = 0;
                    for (Int32 i = 0; i < N; i++)
                    {
                        Sum += A[j, i] * B[i, OB[t + 1]] * BETA[t + 1, i];
                    }

                    BETA[t, j] = Sum;
                }
            }

            // 终止  
            Double Probability = 0;
            for (Int32 i = 0; i < N; i++)
            {
                Probability += BETA[0, i];
            }

            return Probability;
        }

        /// <summary>  
        /// 带比例因子修正的后向算法  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>  
        /// <param name="SCALE">用于修正的比例因子</param>  
        /// <param name="BETA">中间结果：局部概率</param>  
        private void BackwardWithScale(Int32[] OB, Double[] SCALE, ref Double[,] BETA)
        {
            if (BETA == null) BETA = new Double[OB.Length, N];

            // 初始化  
            for (Int32 j = 0; j < N; j++)
            {
                BETA[OB.Length - 1, j] = 1.0 / SCALE[OB.Length - 1];
            }

            // 归纳  
            for (Int32 t = OB.Length - 2; t >= 0; t--)
            {
                for (Int32 j = 0; j < N; j++)
                {
                    Double Sum = 0;
                    for (Int32 i = 0; i < N; i++)
                    {
                        Sum += A[j, i] * B[i, OB[t + 1]] * BETA[t + 1, i];
                    }

                    BETA[t, j] = Sum / SCALE[t];
                }
            }
        }

        /// <summary>  
        /// 前向-后向算法，用于参数学习  
        /// Forward-backward algorithm: Generating a HMM from a sequence of obersvations  
        /// </summary>  
        /// <param name="OB">已知的观察序列</param>          
        /// <param name="LogProbInit">初始自然对数概率</param>  
        /// <param name="LogProbFinal">最终自然对数概率</param>  
        /// <param name="ExitError">迭代中允许的自然对数概率误差，缺省0.001</param>  
        /// <param name="MSP">状态概率最小值，缺省0.001</param>  
        /// <param name="MOP">观察概率最小值，缺省0.001</param>  
        /// <returns>迭代次数</returns>  
        /// <remarks>修正UMDHMM在模型参数调整中的错误</remarks>          
        public Int32 BaumWelch(Int32[] OB, out Double LogProbInit, out Double LogProbFinal,
            Double ExitError = 0.001, Double MSP = 0.001, Double MOP = 0.001)
        {
            Double[,] ALPHA = null;
            Double[,] BETA = null;
            Double[] SCALE = null;
            Double[,] GAMMA = null;
            Double[, ,] XI = null;

            Double LogProbForward = LogProbInit = ForwardWithScale(OB, ref ALPHA, ref SCALE); // 前向算法  
            BackwardWithScale(OB, SCALE, ref BETA); // 后向算法  
            ComputeGamma(ALPHA, BETA, ref GAMMA);   // 求解各时刻位于各隐藏状态的概率矩阵  
            ComputeXI(OB, ALPHA, BETA, ref XI);     // 求解各时刻位于各隐藏状态及下一时刻位于各隐藏状态的关联概率矩阵  

            Int32 Iterations;
            Double LogProbPrev = LogProbForward;
            for (Iterations = 1; ; Iterations++)
            {   // 重新估计初始概率向量  
                for (Int32 i = 0; i < N; i++)
                {   // 注意：此处修正UMDHMM错误，以保证概率总和为1  
                    PI[i] = MSP + (1 - MSP * N) * GAMMA[0, i];
                }

                for (Int32 i = 0; i < N; i++)
                {   // 重新估计状态转移矩阵  
                    Double DenominatorA = 0;
                    for (Int32 t = 0; t < OB.Length - 1; t++)
                        DenominatorA += GAMMA[t, i];

                    for (Int32 j = 0; j < N; j++)
                    {
                        Double NumeratorA = 0;
                        for (Int32 t = 0; t < OB.Length - 1; t++)
                            NumeratorA += XI[t, i, j];

                        // 注意：此处修正UMDHMM错误，以保证概率总和为1  
                        A[i, j] = MSP + (1 - MSP * N) * NumeratorA / DenominatorA;
                    }

                    // 重新估计混淆矩阵  
                    Double DenominatorB = DenominatorA + GAMMA[OB.Length - 1, i];
                    for (Int32 k = 0; k < M; k++)
                    {
                        Double NumeratorB = 0;
                        for (Int32 t = 0; t < OB.Length; t++)
                        {
                            if (OB[t] == k) NumeratorB += GAMMA[t, i];
                        }

                        // 注意：此处修正UMDHMM错误，以保证概率总和为1  
                        B[i, k] = MOP + (1 - MOP * M) * NumeratorB / DenominatorB;
                    }
                } // End for i  

                // 计算概率差，决定是否停止迭代  
                LogProbForward = ForwardWithScale(OB, ref ALPHA, ref SCALE);
                if (LogProbForward - LogProbPrev <= ExitError) break;

                BackwardWithScale(OB, SCALE, ref BETA);
                ComputeGamma(ALPHA, BETA, ref GAMMA);
                ComputeXI(OB, ALPHA, BETA, ref XI);
                LogProbPrev = LogProbForward;
            } // End for Iterations  

            LogProbFinal = LogProbForward;  // 最终概率  

            // 返回迭代次数  
            return Iterations;
        }

        /// <summary>  
        /// 求解t时刻位于隐藏状态Si的概率矩阵  
        /// </summary>  
        /// <param name="ALPHA">前向算法局部概率</param>  
        /// <param name="BETA">后向算法局部概率</param>  
        /// <param name="GAMMA">输出：各时刻位于各隐藏状态的概率矩阵</param>  
        private void ComputeGamma(Double[,] ALPHA, Double[,] BETA, ref Double[,] GAMMA)
        {
            Int32 T = ALPHA.GetLength(0);
            if (GAMMA == null) GAMMA = new Double[T, N];

            for (Int32 t = 0; t < T; t++)
            {
                Double Denominator = 0;
                for (Int32 i = 0; i < N; i++)
                {
                    GAMMA[t, i] = ALPHA[t, i] * BETA[t, i];
                    Denominator += GAMMA[t, i];
                }

                for (Int32 i = 0; i < N; i++)
                {
                    GAMMA[t, i] /= Denominator; // 保证各时刻的概率总和等于1  
                }
            }
        }

        /// <summary>  
        /// 求解t时刻位于隐藏状态Si及t+1时刻位于隐藏状态Sj的概率矩阵  
        /// </summary>  
        /// <param name="OB">观察序列</param>  
        /// <param name="ALPHA">前向算法局部概率</param>  
        /// <param name="BETA">后向算法局部概率</param>  
        /// <param name="XI">输出：求解各时刻位于各隐藏状态及下一时刻位于各隐藏状态的关联概率矩阵</param>  
        private void ComputeXI(Int32[] OB, Double[,] ALPHA, Double[,] BETA, ref Double[, ,] XI)
        {
            Int32 T = OB.Length;
            if (XI == null) XI = new Double[T, N, N];

            for (Int32 t = 0; t < T - 1; t++)
            {
                Double Sum = 0;
                for (Int32 i = 0; i < N; i++)
                {
                    for (Int32 j = 0; j < N; j++)
                    {
                        XI[t, i, j] = ALPHA[t, i] * A[i, j] * B[j, OB[t + 1]] * BETA[t + 1, j];
                        Sum += XI[t, i, j];
                    }
                }

                // 保证各时刻的概率总和等于1  
                for (Int32 i = 0; i < N; i++)
                    for (Int32 j = 0; j < N; j++)
                        XI[t, i, j] /= Sum;
            }
        }  
    }  
}  

