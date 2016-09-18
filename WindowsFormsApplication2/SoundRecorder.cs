using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System.Windows.Forms;


namespace WindowsFormsApplication2
{
    class SoundRecorder
    {
        private Notify myNotify = null;
        private WaveFormat mWaveFormat;
        private int iNotifyNum = 3;//通知的个数
        private int iBufferOffset = 0;//本次数据起始点， 上一次数据的终点。
        private int iSampleSize = 0;//所采集到的数据大小
        private int iNotifySize = 0;//通知所在区域大小
        private int iBufferSize = 0;//缓冲区大小
        private Capture capture = null;//捕捉设备对象
        private CaptureBuffer capturebuffer = null;
        private AutoResetEvent notifyevent = null;
        private Thread notifythread = null;
        private Thread managethread = null;
        private StreamWriter mWriter;
        private string mFileName = string.Empty;
        private FileStream mWavFile = null;
        public  static int count = 0;
        private byte[] capturedata = null;
        private byte[] data = null;
        private TextBox tb;
        private TextBox tb2;
        private TextBox tb3;
        private bool g;
        private bool isHMM;
        FFTSensor newFFT = new FFTSensor();
        ImageForm form = null;
        public SoundRecorder(TextBox tb,TextBox tb2,TextBox tb3,bool g, bool isHMM,ImageForm form)
        {
            InitCaptureDevice();
            mWaveFormat = CreatWaveFormat();
            this.tb = tb;
            this.tb2 = tb2;
            this.tb3 = tb3;
            this.g = g;
            this.isHMM = isHMM;
            this.form = form;
        }
        public void setFileName(string filename)
        {
            mFileName = filename;
        }
        public void RecStart()
        {
            //CreatSoundFile1();
            if (capturebuffer == null)
            {
                CreatCaptureBuffer();
                CreateNotification();
                capturebuffer.Start(true);
            }
            else
            {
                MessageBox.Show("已经开始");
            }
            
           
        }
        public void RecStop()
        {
            capturebuffer.Stop();
         //   if (null == notifyevent)
           //     notifyevent.Set();
            //RecordCaptureData();
            if (notifythread.IsAlive == true)
            {
                notifythread.Abort();
            }
            capturebuffer = null;
            notifythread = null;
          //  Goertzel.wirteFile();
            //mWriter.Close();
           // mWavFile.Close();
           // mWriter = null;
          //  mWavFile = null;

       
        }
        private bool InitCaptureDevice() 
        {
            //获取默认音频捕捉设备
            CaptureDevicesCollection devices = new CaptureDevicesCollection();
            Guid deviceGuid = Guid.Empty;//音频捕捉设备的ID
            if (devices.Count > 0)
            {
                deviceGuid = devices[0].DriverGuid;
            }
            else
            {
                MessageBox.Show("无音频捕捉设备");
                return false;
            }
            try
            {
                capture = new Capture(deviceGuid);
            }
            catch(DirectXException e1)
            {
                MessageBox.Show(e1.ToString());
                return false;
            }
            return true;
        }
        //设置PCM格式
        private WaveFormat CreatWaveFormat() 
        {
            WaveFormat format = new WaveFormat();
            format.FormatTag = WaveFormatTag.Pcm;
            format.Channels = 1;//单声道
            format.BitsPerSample = 16;//16位
            format.SamplesPerSecond = 44100;//44.1khz
            format.BlockAlign = (short)(format.Channels * (format.BitsPerSample / 8));
            format.AverageBytesPerSecond = format.BlockAlign * format.SamplesPerSecond;
            return format;
        }
        //建立捕捉缓冲区，两个参数：缓冲区信息，缓冲设备
        //
        private void CreatCaptureBuffer()
        {
            CaptureBufferDescription bufferdescription = new CaptureBufferDescription();
            iNotifySize = 4096;
            iNotifySize -= iNotifySize % mWaveFormat.BlockAlign;//保证包含整数样本
            iBufferSize = iNotifyNum * iNotifySize;
            bufferdescription.BufferBytes = iBufferSize;
            bufferdescription.Format = mWaveFormat;
            capturebuffer = new CaptureBuffer(bufferdescription,capture);
            iBufferOffset = 0;
        }
        //设置通知
        private void CreateNotification()
        {
            BufferPositionNotify[] bpn = new BufferPositionNotify[iNotifyNum];//设置缓冲区通知个数
            notifyevent = new AutoResetEvent(false);
            for (int i = 0; i < iNotifyNum; i++)
            {
                bpn[i].Offset = iNotifySize + i * iNotifySize - 1;
                bpn[i].EventNotifyHandle = notifyevent.Handle;
            }
            myNotify = new Notify(capturebuffer);
            myNotify.SetNotificationPositions(bpn);
            notifythread = new Thread(RecoData);
            notifythread.Start();

        }
        private void RecoData()
        {
            while (true)
            {
                notifyevent.WaitOne(Timeout.Infinite, true);
                RecordCaptureData();
            }
        }
        private void RecordCaptureData()
        {
            capturedata = null;
            int readpos = 0;
            int capturepos = 0;
            int locksize = 0;
            capturebuffer.GetCurrentPosition(out capturepos, out readpos);
            //Console.WriteLine("capturepos = "+capturepos+",readpos="+readpos);
            //Console.WriteLine("offset = " + iBufferOffset);
            locksize = readpos - iBufferOffset;
            if (locksize == 0)
            {
                return;
            }
            if (locksize < 0)
            {
                locksize += iBufferSize;
            }
            capturedata = (byte[])capturebuffer.Read(iBufferOffset, typeof(byte), LockFlag.FromWriteCursor, 4096);
            
            //iSampleSize += capturedata.Length;
            iBufferOffset += capturedata.Length;
            iBufferOffset %= iBufferSize;
            newFFT.Manage(capturedata, tb,tb2,tb3, g,isHMM,form);
        }
    }
}
