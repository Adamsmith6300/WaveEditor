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
        ComplexNum[][] dftThreadSamples;
        Chart2 fourierChart;
        long posXStart;
        long posXFinish;

        private string filename;
        private RWWaveFile waveFile;
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


        [DllImport("as3.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool OpenDialog();
        [DllImport("as3.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CloseDialog();
        [DllImport("as3.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RecordData StopRec();
        [DllImport("as3.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StartRec(int d, int r);
        [DllImport("as3.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetWaveform();

        [DllImport("as3.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayPause();

        [DllImport("as3.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayStart(IntPtr p, int size, int d, int r, int c);
        [DllImport("as3.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayStop();


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
            series2.Color = Color.Red;
            series.ChartType = SeriesChartType.Spline;
            series2.ChartType = SeriesChartType.Spline;
            //series.XValueType = ChartValueType.Double;
            var chartArea = chart1.ChartAreas[series.ChartArea];
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
            selEnd = dataSize;
            sel = dataSize;
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
            
            this.hScrollBar1.Minimum = 0;
            this.hScrollBar1.SmallChange = sel == dataSize ? sel : sel/dataSize;
            this.hScrollBar1.LargeChange = sel == dataSize ? sel : sel/dataSize;
            this.hScrollBar1.Maximum = dataSize - this.hScrollBar1.SmallChange;
            this.hScrollBar1.Visible = true;
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
            int threadCount = 2;
            N = (int)Math.Round((N / (double)threadCount),
             MidpointRounding.AwayFromZero) * threadCount;
            ThreadStart[] childRefs = new ThreadStart[threadCount];
            Thread[] childThreads = new Thread[threadCount];
            dftThreadSamples = new ComplexNum[threadCount][];
            childRefs[0] = new ThreadStart(() => dftThread(0, start, N, threadCount));
            childThreads[0] = new Thread(childRefs[0]);
            childThreads[0].Start();
            childRefs[1] = new ThreadStart(() => dftThread(1, start, N, threadCount));
            childThreads[1] = new Thread(childRefs[1]);
            childThreads[1].Start();
            //childRefs[2] = new Threa dStart(() => dftThread(2, start, N, threadCount));
            //childThreads[2] = new Thread(childRefs[2]);
            //childThreads[2].Start();
            //childRefs[3] = new ThreadStart(() => dftThread(3, start, N, threadCount));
            //childThreads[3] = new Thread(childRefs[3]);
            //childThreads[3].Start();
            foreach (Thread thread in childThreads)
            {
                thread.Join();
            }
            ComplexNum[] e = genComplexE(N);
            combineDfts(e);
        }

        private void dftThread(int startN, long start, long totalN, int threadCount)
        {
            ComplexNum[] dftSamples = new ComplexNum[totalN];
            for (long f = 0; f < dftSamples.Length / threadCount; f++)
            {
                dftSamples[f] = new ComplexNum();
                for (long t = startN; t < totalN; t += threadCount)
                {
                    //ComplexNum getters/setters needed
                    dftSamples[f].Re += rawSamples[start + t] * Math.Cos((2 * Math.PI * (t/threadCount) * f) / (totalN/threadCount));
                    dftSamples[f].Im -= rawSamples[start + t] * Math.Sin((2 * Math.PI * (t/threadCount) * f) / (totalN/threadCount));

                }
            }
            ComplexNum[] newDftSamples = new ComplexNum[totalN];
            Array.Copy(dftSamples, 0, newDftSamples, 0, dftSamples.Length / threadCount);
            Array.Copy(dftSamples, 0, newDftSamples, dftSamples.Length / threadCount, dftSamples.Length / threadCount);
            //Array.Copy(dftSamples, 0, newDftSamples, 2 * (dftSamples.Length / threadCount), dftSamples.Length / threadCount);
            //Array.Copy(dftSamples, 0, newDftSamples, 3 * (dftSamples.Length / threadCount), dftSamples.Length / threadCount);
            dftThreadSamples[startN] = newDftSamples;
        }

        private ComplexNum[] genComplexE(long N)
        {
            ComplexNum[] e = new ComplexNum[N];
            for (int f = 0; f < N; f++)
            {
                e[f] = new ComplexNum();
                e[f].Re = Math.Cos((2 * Math.PI * f) / N);
                e[f].Im = Math.Sin((2 * Math.PI * f) / N);
            }
            return e;
        }

        private void combineDfts(ComplexNum[] e)
        {
            fourierSamples = new ComplexNum[e.Length];
            for(int i = 0; i < fourierSamples.Length; i++)
            {
                fourierSamples[i] = new ComplexNum();
                fourierSamples[i].Re = dftThreadSamples[0][i].Re;
                fourierSamples[i].Im = dftThreadSamples[0][i].Im;
                for (int j = 1; j < dftThreadSamples.Length; j++)
                {
                    fourierSamples[i].Re += (dftThreadSamples[j][i].Re * e[i].Re);
                    fourierSamples[i].Im += (dftThreadSamples[j][i].Im * e[i].Im);
                }
            }
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

        private void applyFilter(double[] fWeights)
        {
            for(int i = 0; i < rawSamples.Length; i++)
            {
                short temp = 0;
                for (int j = 0; j < fWeights.Length; j++)
                {
                    short rs = i + j >= rawSamples.Length ? (short)0 : rawSamples[i + j];
                    temp += (short)(rs * fWeights[j]);
                }
                rawSamples[i] = temp;
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
                WaveForm wf = (WaveForm)Marshal.PtrToStructure(GetWaveform(), typeof(WaveForm));
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
            //Debug.WriteLine(data.Length + " " + (int)waveFile.FmtChunk1.BitsPerSample + " " + (int)waveFile.FmtChunk1.SamplesPerSec, (int)waveFile.FmtChunk1.Channels);
            PlayStart(iptr, data.Length, (int)waveFile.FmtChunk1.BitsPerSample, (int)waveFile.FmtChunk1.SamplesPerSec, (int)waveFile.FmtChunk1.Channels);
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
            posXStart = (long)(pX * SR);
            //Debug.WriteLine(pX * SR);
        }
        private void Chart1_MouseUp(object sender, EventArgs e)
        {

            MouseEventArgs me = (MouseEventArgs)e;
            var chart = (Chart)sender;
            //var xAxis = chart.ChartAreas[0].AxisX;
            chart.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(me.X, me.Y), true);
            double pX = chart.ChartAreas[0].CursorX.Position;

            var posXBeg = posXStart;
            var posXEnd = (long)(pX * SR);
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
                if (e.Delta < 0) // Scrolled down.
                {
                    if (Math.Abs(selEnd - selStart) < dataSize)
                    {
                        selStart = Math.Max(0, selStart - (int)sampleRate);
                        selEnd = Math.Min(dataSize, selEnd + (int)sampleRate);
                        sel = Math.Abs(selEnd - selStart);
                        //Debug.WriteLine(selStart + ":" + selEnd + ":" + sel);
                        refreshChart();
                        this.hScrollBar1.Value = selStart;
                    }
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    if(Math.Abs(selEnd - selStart) > 10*numChannels)
                    {
                        //double xPos = xAxis.PixelPositionToValue(e.Location.X) * sampleRate;
                        double xPos = (selEnd - selStart) / 2;
                        selStart = Math.Max(0, selStart + (int)sampleRate);
                        selEnd = Math.Min(dataSize, selEnd - (int) sampleRate);
                        sel = Math.Abs(selEnd - selStart);
                        //Debug.WriteLine(selStart + ":" + selEnd + ":" + sel);
                        refreshChart();
                        this.hScrollBar1.Value = selStart;
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
            selEnd = Math.Min(dataSize, selStart + sel);
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
                ComplexNum[] filter = fourierChart.generateFilter();
                double[] filterWeights = idft(filter);
                //foreach (var item in filterWeights)
                //{
                //    Debug.Write(item.ToString() + " ");
                //}
                applyFilter(filterWeights);
                //Debug.WriteLine("\n^^^FilterWeights^^^");
            }
        }
    }
}
