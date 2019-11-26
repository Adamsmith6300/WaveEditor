using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;

namespace WaveVisualizer
{
    public partial class Form1 : Form
    {
        static int maxSamples = 596;
        short[] rawSamples;
        short[] cutSamples;
        uint sampleRate;
        uint bytesPerSec;
        uint bitsPerSample;
        int dataSize;
        ushort numChannels;

        //int maxChartValY = 100000;
        //int minChartValY = -100000;

        int selStart = 0;
        int selEnd = 0;
        int sel = 0;

        short[] testSamples = {1, 1, 0, 0, 0, 1, 1, 0};
        ComplexNum[] fourierSamples;
        short[] windowingSamples;
        ComplexNum[][] dftThreadSamples;
        Chart2 fourierChart;
        long posXStart;
        long posXFinish;

        private string filename;
        private RWWaveFile waveFile;
        private WaveForm wf;
        int bitDepth = 16;
        int SR = 22050;
        int multiplier = 1;
        bool dialogOpen = false;

        public struct RecordData
        {
            public uint len;
            public IntPtr ip;
        }

        public struct WaveForm
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }


        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool OpenDialog();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CloseDialog();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RecordData StopRec();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StartRec(int d, int r);
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetWaveform();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetWaveform(int bits, int rate, int blockAlign, int nChannels, int byteRate);
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayPause();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayStart(IntPtr p, int size, int d, int r, int blockAlign, int c, int byteRate);
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayStop();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RecordData mmxApplyFilter(double[] fWeights, int size, IntPtr pd, int dsize);


        public Form1()
        {
            InitializeComponent();
            //DoubleBuffered = true;
            //   System.Reflection.PropertyInfo aProp =
            //typeof(System.Windows.Forms.Control).GetProperty(
            //      "DoubleBuffered",
            //      System.Reflection.BindingFlags.NonPublic |
            //      System.Reflection.BindingFlags.Instance);
            //   aProp.SetValue(this.Controls[0], true, null);
            this.DoubleBuffered = true;
            this.hScrollBar1.Visible = false;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Chart1_MouseDown);
            this.chart1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Chart1_MouseUp);
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
        }


        private void newChart()
        {
            /*********************
             * START Initial setup
             ********************/
            rawSamples = waveFile.DataChunk1.Data;
            sampleRate = waveFile.FmtChunk1.SamplesPerSec;
            bytesPerSec = waveFile.FmtChunk1.AverageBytesPerSec;
            bitsPerSample = waveFile.FmtChunk1.BitsPerSample;
            numChannels = waveFile.FmtChunk1.Channels;
            dataSize = rawSamples.Length / numChannels;
            // clear the chart series points
            chart1.Series.Clear();
            //Debug.WriteLine(rawSamples.Length);
            var series = chart1.Series.Add("My Series");
            var series2 = chart1.Series.Add("My Series2");
            series.Color = Color.Purple;
            series.ChartType = SeriesChartType.Spline;
            series2.ChartType = SeriesChartType.Spline;
            //series.XValueType = ChartValueType.Double;
            var chartArea = chart1.ChartAreas[series.ChartArea];
            chartArea.BackColor = Color.Black;
            chartArea.AxisX.LabelStyle.Format = "#.###";
            //remove grid lines
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            //axis labels
            //chartArea.AxisX.Title = "Time (s)";
            //chartArea.AxisY.Title = "Frequency";
            chartArea.BorderWidth = 1;
            chartArea.AxisX.ScrollBar.Enabled = false; 
            //allow zooming
            chartArea.AxisX.ScaleView.Zoomable = false;
            //chartArea.AxisY.ScaleView.Zoomable = true;
            chart1.MouseWheel += chart1_MouseWheel;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            //minmum zoom to 10
            chartArea.AxisX.ScaleView.MinSize = 0.00;
            // set view range to [0,max]
            chartArea.AxisX.ScaleView.Zoom(0.00, (double)dataSize/sampleRate);
            chartArea.AxisX.ScaleView.Position = 0.00;
            chartArea.AxisX.Minimum = 0.00;
            chartArea.AxisX.Maximum = Math.Ceiling((double)dataSize/sampleRate);
            //chartArea.AxisY.Maximum = 100000;
            //chartArea.AxisY.Minimum = -100000;
            //reset zoom button
            //chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            // enable autoscroll
            //chartArea.CursorX.AutoScroll = true;
            chartArea.CursorX.Interval = 0.001;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.CursorX.IsUserEnabled = true;
            selStart = 0;
            selEnd = dataSize/numChannels;
            sel = dataSize/numChannels;
            maxSamples = chart1.Size.Width;
            this.hScrollBar1.Value = selStart;
            this.hScrollBar1.SmallChange = maxSamples;
            this.hScrollBar1.LargeChange = maxSamples;
            this.hScrollBar1.Maximum = (dataSize) - maxSamples;
            this.hScrollBar1.Minimum = 0;
            this.hScrollBar1.Visible = true;
            refreshChart();
            /*******************
             * END Initial setup
             ******************/
        }
        private void refreshChart()
        {
            foreach (var ser in chart1.Series)
            {
                ser.Points.Clear();
            }
            //int seconds = (int)rawSamples.Length / (int)sampleRate;
            
            maxSamples = chart1.Size.Width;

            //plot chart points
            if (numChannels == 2)
            {
                int i;
                int jump = sel / maxSamples;
                int start = Math.Max(0, selStart - sel);
                int end = (selEnd + sel) > dataSize ? dataSize : selEnd + sel;

                for (i = start; i < end - 1; i += jump) 
                {
                    double sample = rawSamples[i];
                    double sample2 = rawSamples[i + 1];
                    chart1.Series["My Series"].Points.AddXY((double)i /sampleRate, sample);
                    chart1.Series["My Series2"].Points.AddXY((double)i /sampleRate, sample2);
                }
            }
            else
            {
                int i;
                int jump = sel / maxSamples;
                int start = Math.Max(0,selStart - sel);
                int end = (selEnd + sel) > dataSize ? dataSize : selEnd + sel;
                for (i = start; i < end; i += jump)
                {
                    double sample = rawSamples[i];
                    chart1.Series["My Series"].Points.AddXY((double)i / sampleRate, sample);
                }
            }
            //chart1.ChartAreas[0].AxisX.ScaleView.Position = selStart/sampleRate;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum =  ((dataSize)/ sampleRate) + 1;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom((double)selStart / sampleRate, (double)selEnd / sampleRate);
            
            //this.hScrollBar1.Minimum = 0;
            //this.hScrollBar1.SmallChange = sel == dataSize ? sel : sel/dataSize;
            //this.hScrollBar1.LargeChange = sel == dataSize ? sel : sel/dataSize;
            //this.hScrollBar1.Maximum = dataSize - this.hScrollBar1.SmallChange;
            //this.hScrollBar1.SmallChange = maxSamples;
            //this.hScrollBar1.LargeChange = maxSamples;
            //this.hScrollBar1.Maximum = dataSize - maxSamples;
            //this.hScrollBar1.Visible = true;
        }
      
        
        private void dft(long start, long end)
        {
            long N = end - start;
            /*
             * for testSamples:
             * uncomment N = testSamples length below
             * replace rawSamples in dftThread with testSamples and remove 'start'
             */
            //N = testSamples.Length;
            int threadCount = 4;
            N = (int)Math.Round((N / (double)threadCount),
             MidpointRounding.AwayFromZero) * threadCount;
            applyWindowing(start, N);
            ThreadStart[] childRefs = new ThreadStart[threadCount];
            Thread[] childThreads = new Thread[threadCount];
            dftThreadSamples = new ComplexNum[threadCount][];
            long numberOfBins = N / threadCount;
            childRefs[0] = new ThreadStart(() => dftThread(0, start, N, numberOfBins));
            childThreads[0] = new Thread(childRefs[0]);
            childThreads[0].Start();
            childRefs[1] = new ThreadStart(() => dftThread(1, start, N, numberOfBins));
            childThreads[1] = new Thread(childRefs[1]);
            childThreads[1].Start();
            childRefs[2] = new ThreadStart(() => dftThread(2, start, N, numberOfBins));
            childThreads[2] = new Thread(childRefs[2]);
            childThreads[2].Start();
            childRefs[3] = new ThreadStart(() => dftThread(3, start, N, numberOfBins));
            childThreads[3] = new Thread(childRefs[3]);
            childThreads[3].Start();
            foreach (Thread thread in childThreads)
            {
                thread.Join();
            }
            //ComplexNum[] e = genComplexE(N);
            combineDfts(N, numberOfBins);
        }

        private void dftThread(int threadIndex, long startRawSamples, long N, long chunkSize)
        {
            ComplexNum[] dftSamples = new ComplexNum[chunkSize];
            //Debug.WriteLine("Started thread" + threadIndex);
            for (long f = 0; f < chunkSize; f++)
            {
                dftSamples[f] = new ComplexNum();
                for (int t = 0; t < N; t++)
                {
                    //ComplexNum getters/setters needed
                    dftSamples[f].Re += windowingSamples[t] * Math.Cos((2 * Math.PI * t * (f+(threadIndex*chunkSize))) / N);
                    dftSamples[f].Im -= windowingSamples[t] * Math.Sin((2 * Math.PI * t * (f+(threadIndex*chunkSize))) / N);

                }
            }
            //Debug.WriteLine("DONE thread" + threadIndex);
            dftThreadSamples[threadIndex] = dftSamples;
        }

        private void combineDfts(long N, long numberOfBins)
        {
            fourierSamples = new ComplexNum[N];
            //Debug.WriteLine("Started copying threads");
            Array.Copy(dftThreadSamples[0], 0, fourierSamples, 0, numberOfBins);
            Array.Copy(dftThreadSamples[1], 0, fourierSamples, numberOfBins, numberOfBins);
            Array.Copy(dftThreadSamples[2], 0, fourierSamples, numberOfBins * 2, numberOfBins);
            Array.Copy(dftThreadSamples[3], 0, fourierSamples, numberOfBins * 3, numberOfBins);
            //Debug.WriteLine("Done copying threads");
        }

        private double[] idft(ComplexNum[] A)
        {
            int n = A.Length;
            double[] newSamples = new double[n];
            var zeroThreshold = 1e-10;
            for (int t = 0; t < n; t++)
            {
                double re = 0;
                double im = 0;
                for (int f = 0; f < n; f++)
                {
                    re += A[f].Re * Math.Cos((2 * Math.PI * t * f) / n);
                    im += A[f].Im * Math.Sin((2 * Math.PI * t * f) / n);
                }
                if (Math.Abs(re) < zeroThreshold )//|| re < 0)
                {
                    re = 0;
                }

                if (Math.Abs(im) < zeroThreshold )//|| im < 0)
                {
                    im = 0;
                }
                newSamples[t] = (re + im)/n;
            }
            return newSamples;
        }

        private void applyWindowing(long start, long N)
        {
            windowingSamples = new short[N];
            double[] w = new double[N];
            Array.Copy(rawSamples, start, windowingSamples, 0, N);
            int selectedIndex = comboBox2.SelectedIndex;
            long m = N / 2;
            double r;
            double pi = Math.PI;
            switch (selectedIndex)
            {
                case 0:
                    //rectangular
                    break;
                case 1:
                    //hamming
                    r = pi / m;
                    for (long n = -m; n < m; n++)
                    {
                        w[m + n] = 0.54f + 0.46f * Math.Cos(n * r);
                        windowingSamples[m + n] = (short)(w[m + n] * windowingSamples[m + n]);
                    }
                    break;
                case 2:
                    //hanning
                    r = pi / (m + 1);
                    for (long n = -m; n < m; n++)
                    {
                        w[m + n] = 0.5f + 0.5f * Math.Cos(n * r);
                        windowingSamples[m + n] = (short)(w[m + n] * windowingSamples[m + n]);
                    }
                    break;
                default:
                    //rectangular
                    break;
            }
        }

        private void applyFilter(double[] fWeights)
        {
            for(int i = 0; i < rawSamples.Length; i++)
            {
                double temp = 0;
                for (int j = 0; j < fWeights.Length; j++)
                {
                    short rs = i + j >= rawSamples.Length ? (short)0 : rawSamples[i + j];
                    temp += (rs * fWeights[j]);
                }
                rawSamples[i] = (short)temp;
            }
            refreshChart();
        }

        //event Handlers
        private void sRec_Click_1(object sender, EventArgs e)
        {
            if (OpenDialog())
            {
                dialogOpen = true;
                StartRec(bitDepth, SR);
            } else
            {
                dialogOpen = false;
            }
        }
        private void stRec_Click(object sender, EventArgs e)
        {
            if (!dialogOpen)
            {
                OpenDialog();
            }
            RecordData rd = StopRec();
                byte[] data = new byte[rd.len];
                //gets recorded data (samples)
                Marshal.Copy(rd.ip, data, 0, (int)rd.len);
                //gets wave fmt/riff data
                wf = (WaveForm)Marshal.PtrToStructure(GetWaveform(), typeof(WaveForm));
                waveFile = new RWWaveFile(Encoding.ASCII.GetBytes("RIFF"), rd.len + 36, Encoding.ASCII.GetBytes("WAVE"),
                    Encoding.ASCII.GetBytes("fmt"), 16, (ushort)1, wf.nChannels, wf.nSamplesPerSec,
                    wf.nAvgBytesPerSec, wf.nBlockAlign, wf.wBitsPerSample,
                    Encoding.ASCII.GetBytes("data"), rd.len);
                //sets up samples
                if (waveFile.FmtChunk1.BitsPerSample == 16)
                {
                    short[] temp = new short[rd.len / (int)waveFile.FmtChunk1.BlockAlign];
                    for (int i = 0; i < temp.Length - 1; i++)
                        temp[i] = BitConverter.ToInt16(data, i * (int)waveFile.FmtChunk1.BlockAlign);
                    rawSamples = temp.Select(x => (x)).ToArray();
                    waveFile.DataChunk1.Data = rawSamples;
                    if (waveFile.DataChunk1.Data != null)
                    {
                        sel = (int)waveFile.DataChunk1.DataSize;
                        selEnd = (int)waveFile.DataChunk1.DataSize;
                        newChart();
                    }
                }
            
            
        }
        private void sPlay_Click_1(object sender, EventArgs e)
        {
            if (!dialogOpen)
            {
                OpenDialog();
            }
            if (waveFile == null)
                    return;
            byte[] data = null;
            //preps samples for dll
            if (waveFile.FmtChunk1.BitsPerSample == 16)
            {
                data = rawSamples.Select(x => (short)(x * multiplier)).ToArray()
                    .Select(x => Convert.ToInt16(x))
                    .SelectMany(x => BitConverter.GetBytes(x)).ToArray();
            }
            if (waveFile.FmtChunk1.BitsPerSample == 8)
            {
                data = rawSamples.Select(x => (short)(x * multiplier)).ToArray()
                        .SelectMany(x => BitConverter.GetBytes(x)).ToArray();
            }
            //gets pointer to array, to be sent to dll
            IntPtr iptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * data.Length);
            Marshal.Copy(data, 0, iptr, data.Length);
            //calls play in the dll, passing the pointer and play details
            if (wf.Equals(default(WaveForm)))
            {
                bool isSet = SetWaveform((int)waveFile.FmtChunk1.BitsPerSample, (int)waveFile.FmtChunk1.SamplesPerSec, (int)waveFile.FmtChunk1.BlockAlign, (int)waveFile.FmtChunk1.Channels, (int)waveFile.FmtChunk1.AverageBytesPerSec);
            }
            PlayStart(iptr, data.Length, (int)waveFile.FmtChunk1.BitsPerSample, (int)waveFile.FmtChunk1.SamplesPerSec, (int)waveFile.FmtChunk1.BlockAlign, (int)waveFile.FmtChunk1.Channels, (int)waveFile.FmtChunk1.AverageBytesPerSec);
            Marshal.FreeHGlobal(iptr);


        }
        private void stPlay_Click_1(object sender, EventArgs e)
        {
            if (dialogOpen)
            {
              PlayStop();
            }
        }
        private void Button1_Click(object sender, EventArgs e)
        {
        }
        private void Chart1_MouseDown(object sender, EventArgs e)
        {

            MouseEventArgs me = (MouseEventArgs)e;
            var chart = (Chart)sender;
            //var xAxis = chart.ChartAreas[0].AxisX;
            chart.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(me.X, me.Y), true);
            //chartArea1.CursorY.SetCursorPixelPosition(new Point(me.X, me.Y), true);
            double pX = chart.ChartAreas[0].CursorX.Position;
            
            posXStart = (long)(pX * sampleRate);
            //Debug.WriteLine("ONCLICK-"+ pX + ":" + sampleRate);
        }
        private void Chart1_MouseUp(object sender, EventArgs e)
        {

            MouseEventArgs me = (MouseEventArgs)e;
            var chart = (Chart)sender;
            //var xAxis = chart.ChartAreas[0].AxisX;
            chart.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(me.X, me.Y), true);
            double pX = chart.ChartAreas[0].CursorX.Position;

            var posXBeg = posXStart;
            var posXEnd = (long)(pX * sampleRate);
            //Debug.WriteLine(posXBeg + ":" + posXEnd);
            //Debug.WriteLine(rawSamples.Length);
            if (posXBeg > posXEnd)
            {
                posXStart = posXEnd;
                posXFinish = posXBeg;
                //  dft(posXFinish,posXStart);
            }
            else
            {
                posXStart = posXBeg;
                posXFinish = posXEnd;
                //dft(posXStart,posXFinish);
            }

            //Debug.WriteLine("positions- "+posXStart + ":" + posXFinish);
        }
        private void Chart1_Click(object sender, EventArgs e)
        {
        }
        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;

            try
            {
                if (e.Delta < 0) // zoom out
                {
                    if (Math.Abs(selEnd - selStart) < dataSize)
                    {
                        selStart = Math.Max(0, selStart - 1000);
                        selEnd = Math.Min(dataSize, selEnd + 1000);
                        sel = Math.Abs(selEnd - selStart);
                        //Debug.WriteLine(selStart + ":" + selEnd + ":" + sel);
                        this.hScrollBar1.Value = selStart;
                        refreshChart();
                    }
                }
                else if (e.Delta > 0) // zoom in
                {
                    if (Math.Abs(selEnd - selStart) > 5*maxSamples)
                    {
                        //double xPos = xAxis.PixelPositionToValue(e.Location.X) * sampleRate;
                        //double xPos = (selEnd - selStart) / 2;
                        selStart = Math.Max(0, selStart + 1000);
                        selEnd = Math.Min(dataSize, selEnd - 1000);
                        sel = Math.Abs(selEnd - selStart);
                        //Debug.WriteLine(selStart + ":" + selEnd + ":" + sel);
                        this.hScrollBar1.Value = selStart;
                        refreshChart();
                    }
                   
                }
            }
            catch { }
        }

        private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.X)
            {
                int posXStart = (int)(chart1.ChartAreas[0].CursorX.SelectionStart * sampleRate * numChannels);
                int posXFinish = (int)(chart1.ChartAreas[0].CursorX.SelectionEnd * sampleRate * numChannels);
                //Debug.WriteLine(" Press " + (posXFinish - posXStart));
                cutRawSamples(posXStart, posXFinish);
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                int posX = (int)(chart1.ChartAreas[0].CursorX.Position * sampleRate * numChannels);
                pasteRawSamples(posX);
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                int posXStart = (int)(chart1.ChartAreas[0].CursorX.SelectionStart * sampleRate * numChannels);
                int posXFinish = (int)(chart1.ChartAreas[0].CursorX.SelectionEnd * sampleRate * numChannels);
                copyRawSamples(posXStart, posXFinish);
            }
        }

        private void cutRawSamples(int posXStart, int posXFinish)
        {
            //cut & save raw samples
            int length = (int)Math.Abs(posXFinish - posXStart);
            cutSamples = new short[length];
            Array.Copy(rawSamples, posXStart, cutSamples, 0, length);
            //make new arr for raw samples minus the cut
            short[] newRawSamples = new short[rawSamples.Length - length];
            //copy left side of cut into new arr
            Array.Copy(rawSamples, 0, newRawSamples, 0, posXStart);
            //copy right side of cut int arr
            Array.Copy(rawSamples, posXFinish, newRawSamples, posXStart, (rawSamples.Length-(posXStart + length)));
            //reinitialize rawsamples
            Debug.WriteLine(rawSamples[rawSamples.Length - 1]);
            rawSamples = new short[newRawSamples.Length];
            //copy new raw samples into old raw samples variable
            Array.Copy(newRawSamples, 0, rawSamples, 0, newRawSamples.Length);
            dataSize = rawSamples.Length / numChannels;
            //selEnd = selEnd - length;
            refreshChart();
        }
        private void copyRawSamples(int posXStart, int posXFinish)
        {
            //cut & save raw samples
            int length = (int)Math.Abs(posXFinish - posXStart);
            cutSamples = new short[length];
            Array.Copy(rawSamples, posXStart, cutSamples, 0, length);
            //selEnd = selEnd - length;
            //refreshChart();
        }
        private void pasteRawSamples(int posX)
        {
            int length = cutSamples.Length;
            short[] newRawSamples = new short[rawSamples.Length + length];
            //paste left side of cut into new arr
            Array.Copy(rawSamples, 0, newRawSamples, 0, posX);
            //paste cutout section
            Array.Copy(cutSamples, 0, newRawSamples, posX+1, cutSamples.Length);
            //paste right side of cut int arr
            Array.Copy(rawSamples, posX, newRawSamples, (posX+length), rawSamples.Length-posX);
            //reinitialize rawsamples
            rawSamples = new short[newRawSamples.Length];
            //copy new raw samples into old raw samples variable
            Array.Copy(newRawSamples, 0, rawSamples, 0, newRawSamples.Length);

            Debug.WriteLine(rawSamples[rawSamples.Length-1]);
            dataSize = rawSamples.Length / numChannels;
            //selEnd = selEnd - length;
            refreshChart();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();

            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                filename = fileDlg.FileName;
                waveFile = new RWWaveFile(filename);
                sel = (int) waveFile.DataChunk1.DataSize;
                selEnd = (int) waveFile.DataChunk1.DataSize;
                newChart();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "wav (*.wav)|*.wav|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Wav File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                filename = saveFileDialog1.FileName;
                waveFile.DataChunk1.Data = rawSamples;
                waveFile.Write(filename);
            }
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            selStart = Math.Max(0, this.hScrollBar1.Value);
            selEnd = selStart + sel;
            //selEnd = Math.Min(dataSize, selStart + sel);
            //Debug.WriteLine(this.hScrollBar1.Value);
            //sel = Math.Abs(selEnd - selStart);
            refreshChart();
        }

        private void dftButton_Click(object sender, EventArgs e)
        {
            dft(posXStart, posXFinish);
            ///setup chart with fourier samples
            fourierChart = new Chart2(this.fourierSamples, this.chart2);
            fourierChart.setupChart();
        }

        private void filterButton_Click(object sender, EventArgs e)
        {
            if(fourierChart!= null)
            {
                int selectedIndex = comboBox1.SelectedIndex;
                ComplexNum[] filter = fourierChart.generateFilter();
                double[] filterWeights = idft(filter);
                switch (selectedIndex)
                {
                    case 0:
                        {
                            applyFilter(filterWeights);
                            break;
                        }
                    case 1:
                        {
                            IntPtr diptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(short)) * rawSamples.Length);
                            Marshal.Copy(rawSamples, 0, diptr, rawSamples.Length);
                            RecordData rd = mmxApplyFilter(filterWeights, filterWeights.Length, diptr, rawSamples.Length);
                            Marshal.Copy(rd.ip, rawSamples, 0, (int)rd.len);
                            waveFile.DataChunk1.Data = rawSamples;
                            refreshChart();
                            Marshal.FreeHGlobal(diptr);
                            break;
                        }
                    case 2:
                        {

                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            applyFilter(filterWeights);
                            watch.Stop();
                            var elapsedBasic = watch.ElapsedMilliseconds;

                            IntPtr diptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(short)) * rawSamples.Length);
                            Marshal.Copy(rawSamples, 0, diptr, rawSamples.Length);
                            var watchMMX = System.Diagnostics.Stopwatch.StartNew();
                            RecordData rd = mmxApplyFilter(filterWeights, filterWeights.Length, diptr, rawSamples.Length);
                            Marshal.Copy(rd.ip, rawSamples, 0, (int)rd.len);
                            waveFile.DataChunk1.Data = rawSamples;
                            refreshChart();
                            watchMMX.Stop();
                            var elapsedMMX = watchMMX.ElapsedMilliseconds;
                            Debug.WriteLine(elapsedBasic + ":" + elapsedMMX);
                            double diff = (double)elapsedBasic / (double)elapsedMMX;
                            diff = Math.Round(diff, 3);
                            string box_msg = "MMX is " + diff.ToString() + " times faster than basic C#";
                            string box_title = "MMX vs C# Convolution Performance";
                            MessageBox.Show(box_msg, box_title);
                            Marshal.FreeHGlobal(diptr);
                            break;
                        }
                    default:
                        {
                            applyFilter(filterWeights);
                            break;
                        }
                }
                
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
